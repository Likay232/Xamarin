using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AdminController(AdminService service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetSomeShit()
    {
        return StatusCode(200);
    }
}