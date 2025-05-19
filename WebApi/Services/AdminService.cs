using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Components;
using WebApi.Infrastructure.Models.DTO;
using WebApi.Infrastructure.Models.Requests;

namespace WebApi.Services;

public class AdminService(DataComponent component)
{
    public async Task<List<UserDto>> GetUsers()
    {
        return await component.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
            })
            .ToListAsync();
    }

    public async Task<bool> ChangeUserPassword(ChangePassword request)
    {
        var userEntry = await component.Users.FirstOrDefaultAsync(u => u.Id == request.userId);
        
        if (userEntry == null) return false;
        
        userEntry.Password = request.newPassword;
        
        return await component.Update(userEntry);
    }
}