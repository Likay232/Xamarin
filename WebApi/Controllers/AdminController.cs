﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize(Roles = "Admin")]
[Route("[controller]/[action]")]
public class AdminController(AdminService service) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        return View(await service.GetUsers());
    }

    [HttpGet]
    public async Task<IActionResult> SwitchBlockState(int userId)
    {
        await service.SwitchBlockState(userId);
        
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> ChangePasswordForUser(int userId)
    {
        var user = await service.GetUser(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePasswordForUser([FromForm] ChangePasswordAdmin request)
    {
        try
        {
            var result = await service.ChangeUserPassword(request);

            if (!result)
            {
                TempData["Error"] = "Ошибка при обновлении пароля";
            }
            else
            {
                TempData["Message"] = "Пароль успешно изменён";
            }

            return RedirectToAction("ChangePasswordForUser", new { request.userId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("ChangePasswordForUser", new { request.userId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Themes()
    {
        return View(await service.GetThemes());
    }
    
    [HttpGet]
    public IActionResult AddTheme()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddTheme([FromForm] CreateTheme request)
    {
        try
        {
            await service.CreateNewTheme(request);
            return RedirectToAction(nameof(Themes));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка при создании темы: {e.Message}");
            return View();
        }
    }

    [HttpGet]
    public IActionResult AddLesson(int themeId)
    {
        var model = new LessonDto {ThemeId = themeId};
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddLesson([FromForm] LessonDto lessonToAdd)
    {
        try
        {
            await service.AddLessonForTheme(lessonToAdd);
            return RedirectToAction(nameof(Themes));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка при добавлении материала: {e.Message}");
            return View(lessonToAdd);
        }
    }
    
    [HttpGet]
    public IActionResult AddTask(int themeId)
    {
        var model = new TaskDto
        {
            ThemeId = themeId
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask([FromForm] TaskDto model, IFormFile? Image, IFormFile? File)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (Image is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await Image.CopyToAsync(ms);
                model.ImageData = ms.ToArray();
            }

            if (File is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await File.CopyToAsync(ms);
                model.FileData = ms.ToArray();
            }

            var success = await service.AddTaskForTheme(model);

            if (success)
                return RedirectToAction(nameof(Tasks), new { themeId = model.ThemeId });

            ModelState.AddModelError("", "Не удалось добавить задание");
            return View(model);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка: {e.Message}");
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Tasks(int themeId)
    {
        return View(await service.GetTasksForTheme(themeId));
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteTask(int taskId, int themeId)
    {
        try
        {
            var result = await service.DeleteTaskForTheme(taskId);
            if (result)
            {
                return RedirectToAction(nameof(Tasks), new { themeId });
            }

            TempData["Error"] = "Не удалось удалить задание.";
            return RedirectToAction(nameof(Tasks), new { themeId });
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Ошибка при удалении задания: {e.Message}";
            return RedirectToAction(nameof(Tasks), new { themeId });
        }
    }

    
    [HttpGet]
    public async Task<IActionResult> EditTask(int taskId)
    {
        var task = await service.GetTaskById(taskId);
        if (task == null)
            return NotFound();

        return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask([FromForm] TaskDto model, IFormFile? Image, IFormFile? File)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (Image is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await Image.CopyToAsync(ms);
                model.ImageData = ms.ToArray();
            }

            if (File is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await File.CopyToAsync(ms);
                model.FileData = ms.ToArray();
            }
            
            var result = await service.EditTaskForTheme(model);
            if (result)
                return RedirectToAction(nameof(Tasks), new { themeId = model.ThemeId });

            ModelState.AddModelError("", "Не удалось обновить задание.");
            return View(model);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка: {e.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateTest()
    {
        var tasks = await service.GetTasks();
        ViewBag.Tasks = tasks;
        return View(new CreateTest());
    }

    [HttpPost]
    public async Task<IActionResult> CreateTest([FromForm] CreateTest request, string taskIdsStr)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskIdsStr))
            {
                ModelState.AddModelError("", "Не выбрано ни одного задания.");
                var tasks = await service.GetTasks();
                ViewBag.Tasks = tasks;
                return View(request);
            }

            request.TaskIds = taskIdsStr
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            var result = await service.CreateTest(request);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", $"Ошибка при создании теста: {e.Message}");
            var tasks = await service.GetTasks();
            ViewBag.Tasks = tasks;
            return View(request);
        }
    }
}