namespace WebApi.Infrastructure.Models.Storage;

public class TaskForTest : BaseEntity
{
    public string Text { get; set; }
    public string CorrectAnswer { get; set; }

    public int DifficultyLevel { get; set; }
    
    public byte[]? ImageData { get; set; }
    public byte[]? FileData { get; set; }
    
    public int ThemeId { get; set; }
    public Theme Theme { get; set; }
}