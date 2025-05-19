namespace WebApi.Infrastructure.Models.Storage;

public class TestTask : BaseEntity
{
    public int TaskId { get; set; }
    public TaskForTest TaskForTest { get; set; }
    
    public int TestId { get; set; }
    public Test Test { get; set; }
}