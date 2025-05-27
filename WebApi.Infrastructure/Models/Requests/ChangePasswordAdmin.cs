namespace WebApi.Infrastructure.Models.Requests;

public class ChangePasswordAdmin
{
    public int userId { get; set; }
    public string newPassword { get; set; }
}