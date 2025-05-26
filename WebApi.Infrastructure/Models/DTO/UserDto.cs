namespace WebApi.Infrastructure.Models.DTO;

public class UserDto
{
    public int Id { get; set; }
    
    public string LastName { get; set; }
    
    public string FirstName { get; set; }
    
    public bool IsBlocked { get; set; }
}