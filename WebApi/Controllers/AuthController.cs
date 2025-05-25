using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> LoginAdmin(Login request)
    {
        try
        {
            return StatusCode(200, await service.LoginAdmin(request));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<string?>> LoginClient(Login request)
    {
        try
        {
            return StatusCode(200, await service.Login(request));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

}