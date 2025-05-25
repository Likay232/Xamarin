namespace WebApi.Infrastructure.Models.Storage;

public class CompletedTask : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int TaskForTestId { get; set; }
    public TaskForTest TaskForTest { get; set; }
    
    public bool IsCorrect { get; set; }
}