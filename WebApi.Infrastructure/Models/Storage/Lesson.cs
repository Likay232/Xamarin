namespace WebApi.Infrastructure.Models.Storage;

public class Lesson : BaseEntity
{
    public string Text { get; set; }

    public string? Link { get; set; }

    public int ThemeId { get; set; }
    public Theme Theme { get; set; }
}