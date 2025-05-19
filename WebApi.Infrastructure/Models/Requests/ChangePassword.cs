namespace WebApi.Infrastructure.Models.Requests;

public class ChangePassword
{
    public int userId { get; set; }
    public string newPassword { get; set; }
}