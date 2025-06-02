﻿using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Components;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Infrastructure.Models.Storage;

namespace WebApi.Services;

public class ClientService(DataComponent component, IWebHostEnvironment env)
{
    public async Task<bool> RegisterDevice(RegisterDevice request)
    {
        if (!await component.Users.AnyAsync(u => u.Id == request.UserId))
            throw new Exception("User not found");

        if (await component.UserDevices.AnyAsync(u =>
                u.UserId == request.UserId && u.DeviceToken == request.DeviceToken))
            return true;

        var newUserDevice = new UserDevice
        {
            UserId = request.UserId,
            DeviceToken = request.DeviceToken,
        };

        return await component.Insert(newUserDevice);
    }

    public async Task<List<ThemeDto>> GetThemes()
    {
        return await component.Themes.Select(t => new ThemeDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
        }).ToListAsync();
    }

    public async Task<List<TaskForClientDto>> GetTasksForTheme(GetTasks request)
    {
        if (!await component.Themes.AnyAsync(t => t.Id == request.ThemeId))
            throw new Exception("Тема не найдена.");

        var completed = await component.CompletedTasks
            .Where(ct => ct.UserId == request.UserId)
            .ToListAsync();

        var tasks = await component.Tasks
            .Where(t => t.ThemeId == request.ThemeId)
            .ToListAsync();

        return tasks.Select(t =>
        {
            var completedTask = completed.FirstOrDefault(ct => ct.TaskForTestId == t.Id);

            return new TaskForClientDto
            {
                Id = t.Id,
                Text = t.Text,
                CorrectAnswer = t.CorrectAnswer,
                DifficultyLevel = t.DifficultyLevel,
                File = t.FilePath,
                Image = t.ImageData,
                IsCorrect = completedTask?.IsCorrect ?? null
            };
        }).ToList();
    }

    public async Task<List<LessonDto>> GetLessonsForTheme(int themeId)
    {
        return await component.Lessons
            .Where(l => l.ThemeId == themeId)
            .Select(l => new LessonDto
            {
                Text = l.Text,
                Link = l.Link,
            })
            .ToListAsync();
    }

    public async Task<List<TestDto>> GetTests()
    {
        return await component.Tests
            .Select(t => new TestDto
            {
                Id = t.Id,
                Title = t.Title,
            })
            .ToListAsync();
    }

    public async Task<List<TaskForClientDto>> GetTest(int testId)
    {
        if (!await component.TestTasks.AnyAsync(t => t.TestId == testId))
            throw new Exception("Вопросы для теста с заданным Id не найден.");

        return await component.TestTasks
            .Where(t => t.TestId == testId)
            .Select(t => new TaskForClientDto()
            {
                Id = t.TaskForTestId,
                Text = t.TaskForTest.Text,
                CorrectAnswer = "",
                DifficultyLevel = t.TaskForTest.DifficultyLevel,
                File = t.TaskForTest.FilePath,
                Image = t.TaskForTest.ImageData,
            })
            .ToListAsync();
    }

    public async Task<CheckedTest> CheckTest(TestForCheck test)
    {
        var wrongTasks = new List<WrongTask>();

        foreach (var userAnswer in test.Answers)
        {
            var isCorrect = await CheckTask(new CheckTask
            {
                TaskId = userAnswer.TaskId,
                UserId = test.UserId,
                Answer = userAnswer.Answer
            });

            if (!isCorrect)
            {
                var task = await component.Tasks.FirstOrDefaultAsync(t => t.Id == userAnswer.TaskId);

                wrongTasks.Add(new WrongTask
                {
                    Text = task != null ? task.Text : "",
                    ImageData = task?.ImageData,
                    FilePath = task?.FilePath,
                    Answer = userAnswer.Answer,
                });
            }
        }

        return new CheckedTest
        {
            WrongTasks = wrongTasks,
            Score = $"{test.Answers.Count - wrongTasks.Count} / {test.Answers.Count}"
        };
    }

    public async Task<TaskForClientDto> GetTaskById(int taskId)
    {
        var taskFromDb = await component.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

        if (taskFromDb == null)
            throw new Exception("Задание с заданным id не найдено.");

        var task = new TaskForClientDto
        {
            Id = taskId,
            Text = taskFromDb.Text,
            DifficultyLevel = taskFromDb.DifficultyLevel,
            File = taskFromDb.FilePath,
            Image = taskFromDb.ImageData,
            IsCorrect = false
        };

        return task;
    }

    public async Task<TaskDto> GetRandomTask()
    {
        var tasks = await component.Tasks.ToListAsync();

        if (tasks.Count == 0)
            throw new Exception("Задания не найдены в базе данных.");

        var random = new Random();

        var randomTask = tasks[random.Next(tasks.Count)];

        return new TaskDto
        {
            Id = randomTask.Id,
            ThemeId = randomTask.ThemeId,
            Text = randomTask.Text,
            DifficultyLevel = randomTask.DifficultyLevel,
            FilePath = randomTask.FilePath,
            ImageData = randomTask.ImageData,
        };
    }

    public async Task<bool> CheckTask(CheckTask answer)
    {
        var task = await component.Tasks.FirstOrDefaultAsync(t => t.Id == answer.TaskId);

        if (task == null) return false;

        var existing = await component.CompletedTasks
            .FirstOrDefaultAsync(ct => ct.UserId == answer.UserId && ct.TaskForTestId == answer.TaskId);

        var isCorrect = task.CorrectAnswer == answer.Answer;

        if (existing != null)
        {
            existing.IsCorrect = isCorrect;
            await component.Update(existing);
        }
        else
        {
            var completedTaskToAdd = new CompletedTask
            {
                TaskForTestId = answer.TaskId,
                UserId = answer.UserId,
                IsCorrect = isCorrect
            };

            await component.Insert(completedTaskToAdd);
        }

        if (isCorrect) await UpdateProgress(answer.UserId, task.ThemeId, task.DifficultyLevel);

        return task.CorrectAnswer == answer.Answer;
    }

    public async Task<bool> ChangePassword(ChangePasswordClient request)
    {
        var user = await component.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null) return false;

        if (user.Password != request.OldPassword) return false;

        user.Password = request.NewPassword;

        return await component.Update(user);
    }

    public async Task<List<TaskForClientDto>> GenerateTest(GenerateTest request)
    {
        var test = new List<TaskForClientDto>();
        var random = new Random();

        foreach (var themeId in request.DesiredTasksAmount.Keys)
        {
            var userProgress = await component.Progresses
                .FirstOrDefaultAsync(p => p.ThemeId == themeId && p.UserId == request.UserId);

            if (userProgress == null)
            {
                userProgress = new Progress
                {
                    UserId = request.UserId,
                    ThemeId = themeId,
                    Level = 1,
                    AmountToLevelUp = 5
                };

                await component.Insert(userProgress);
            }

            var tasks = await component.Tasks
                .Where(t => t.ThemeId == themeId)
                .ToListAsync();
            
            var completedTasksIds = await component.CompletedTasks
                .Where(t => t.UserId == request.UserId && t.IsCorrect == true)
                .Select(t => t.TaskForTestId)
                .ToListAsync();

            var desiredTaskAmount = Math.Min(request.DesiredTasksAmount[themeId], tasks.Count);

            var selectedTasks = tasks
                .Select(task => new
                {
                    task.Id,
                    task.Text,
                    task.DifficultyLevel,
                    task.FilePath,
                    task.ImageData,
                    WasSolvedCorrectly = completedTasksIds.Contains(task.Id),
                })
                .OrderBy(t => t.WasSolvedCorrectly ? 1 : 0)
                .ThenBy(t => t.DifficultyLevel <= userProgress.Level ? 0 : 1)
                .ThenBy(_ => random.Next())
                .Take(desiredTaskAmount)
                .Select(t => new TaskForClientDto
                {
                    Id = t.Id,
                    Text = t.Text,
                    DifficultyLevel = t.DifficultyLevel,
                    File = t.FilePath,
                    Image = t.ImageData,
                });

            test.AddRange(selectedTasks);
        }

        return test;
    }

    public async Task<byte[]?> GetFileBytes(string fileName)
    {
        var filePath = Path.Combine(env.ContentRootPath, "FileRepository", fileName);

        if (!File.Exists(filePath))
            return null;

        var fileBytes = await File.ReadAllBytesAsync(filePath);

        return fileBytes;
    }

    private async Task UpdateProgress(int userId, int themeId, int taskDifficulty)
    {
        var existingEntry = await component.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ThemeId == themeId);

        const int maxLevel = 5;
        int currentLevel = existingEntry?.Level ?? 1;
        int currentAmount = existingEntry?.AmountToLevelUp ?? 5;

        if (currentLevel >= maxLevel)
            return;

        int decrement = CalculateDecrement(taskDifficulty, currentLevel);

        int updatedAmount = currentAmount - decrement;

        int updatedLevel = currentLevel;

        if (updatedAmount <= 0)
        {
            updatedLevel = Math.Min(currentLevel + 1, maxLevel);
            updatedAmount = 5;
        }

        if (existingEntry == null)
        {
            var newProgress = new Progress
            {
                UserId = userId,
                ThemeId = themeId,
                Level = updatedLevel,
                AmountToLevelUp = updatedAmount
            };
            await component.Insert(newProgress);
        }
        else
        {
            existingEntry.Level = updatedLevel;
            existingEntry.AmountToLevelUp = updatedAmount;
            await component.Update(existingEntry);
        }
    }

    private int CalculateDecrement(int taskDifficulty, int currentLevel)
    {
        int diff = taskDifficulty - currentLevel;
        return diff >= 0 ? diff + 1 : 1;
    }

}