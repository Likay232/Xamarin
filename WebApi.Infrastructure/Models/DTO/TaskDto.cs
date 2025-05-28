﻿namespace WebApi.Infrastructure.Models.DTO;

public class TaskDto
{
    public int Id { get; set; }
    
    public int ThemeId { get; set; }
    
    public string Text { get; set; }
    public string CorrectAnswer { get; set; }
    
    public int DifficultyLevel { get; set; }
    
    public byte[]? ImageData { get; set; }
    public string? FilePath { get; set; }
}