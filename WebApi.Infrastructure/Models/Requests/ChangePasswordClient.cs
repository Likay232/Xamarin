namespace WebApi.Infrastructure.Models.Requests;

public class ChangePasswordClient
{
    public int UserId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}