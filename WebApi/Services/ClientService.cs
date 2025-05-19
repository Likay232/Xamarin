using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Components;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Storage;

namespace WebApi.Services;

public class ClientService(DataComponent component)
{
    public async Task<List<ThemeDto>> GetThemes()
    {
        return await component.Themes.Select(t => new ThemeDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
        }).ToListAsync();
    }

    //TODO: вернуть правильность
    public async Task<List<TaskForClientDto>> GetTasksForTheme(int themeId)
    {
        if (!await component.Themes.AnyAsync(t => t.Id == themeId))
            throw new Exception("Тема не найдена.");

        return await component.Tasks
            .Where(t => t.ThemeId == themeId)
            .Select(t => new TaskForClientDto()
            {
                Id = t.Id,
                Text = t.Text,
                CorrectAnswer = t.CorrectAnswer,
                DifficultyLevel = t.DifficultyLevel,
                File = t.FileData,
                Image = t.ImageData
            })
            .ToListAsync();
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

    public async Task<List<TaskDto>> GetTest(int testId)
    {
        if (!await component.TestTasks.AnyAsync(t => t.Id == testId))
            throw new Exception("Вопросы для теста с заданным Id не найден.");

        return await component.TestTasks
            .Where(t => t.TestId == testId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Text = t.TaskForTest.Text,
                CorrectAnswer = "",
                DifficultyLevel = t.TaskForTest.DifficultyLevel,
                FileData = t.TaskForTest.FileData,
                ImageData = t.TaskForTest.ImageData
            })
            .ToListAsync();
    }

    public async Task<CheckedTest> CheckTest(TestForCheck test)
    {
        List<UserAnswer> wrongAnswers = new List<UserAnswer>();
        
        foreach (var userAnswer in test.Answers)
        {
            var task = await component.Tasks.FirstOrDefaultAsync(t => t.Id == userAnswer.TaskId);
            var completedTask = new CompletedTask();

            if (task == null || task.CorrectAnswer != userAnswer.Answer)
            {
                wrongAnswers.Add(userAnswer);
            }
            
            if (task != null)
            {
                completedTask.TaskId = userAnswer.TaskId;
                completedTask.UserId = test.UserId;
                completedTask.IsCorrect = task.CorrectAnswer == userAnswer.Answer;
            }

            await component.Insert(completedTask);
        }

        return new CheckedTest
        {
            WrongAnswers = wrongAnswers,
            Score = $"{test.Answers.Count - wrongAnswers.Count} / {test.Answers.Count}"
        }; 
    }
}