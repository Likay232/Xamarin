namespace WebApi.Infrastructure.Models.Requests;

public class CreateTest
{
    public string Title { get; set; }

    public List<int> TaskIds { get; set; }
}