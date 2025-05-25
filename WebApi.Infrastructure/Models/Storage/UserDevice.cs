namespace WebApi.Infrastructure.Models.Storage;

public class UserDevice : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string DeviceToken { get; set; }
}