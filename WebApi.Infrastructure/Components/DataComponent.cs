using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Components;

public class DataComponent(string connectionString)
{
    //TODO: Обновить после создания бд схемы
    
    public async Task<bool> Insert<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new DatabaseContext(connectionString);
            await context.AddAsync(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new DatabaseContext(connectionString);
            context.Entry(entityItem).State = EntityState.Modified;
            context.Update(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Delete<T>(int entityId) where T : class
    {
        try
        {
            await using var context = new DatabaseContext(connectionString);
            var entity = await context.Set<T>().FindAsync(entityId);

            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}