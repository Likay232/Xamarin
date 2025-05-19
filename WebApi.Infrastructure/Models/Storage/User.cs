namespace WebApi.Infrastructure.Models.Storage;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
    
    public DateTime LastLogin { get; set; }

    public bool isBlocked { get; set; }
}