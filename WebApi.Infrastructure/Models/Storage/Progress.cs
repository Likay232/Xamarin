namespace WebApi.Infrastructure.Models.Storage;

public class Progress : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int ThemeId { get; set; }
    public Theme Theme { get; set; }
    
    public int Level { get; set; }
}