﻿using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Models.Storage;

namespace WebApi.Infrastructure.Components;

public class DataComponent(string connectionString)
{
    public IQueryable<User> Users => new DatabaseContext(connectionString).Users;
    public IQueryable<Lesson> Lessons => new DatabaseContext(connectionString).Lessons;
    public IQueryable<CompletedTask> CompletedTasks => new DatabaseContext(connectionString).CompletedTasks;
    public IQueryable<Progress> Progresses => new DatabaseContext(connectionString).Progresses;
    public IQueryable<TestTask> TestTasks => new DatabaseContext(connectionString).TestTasks;
    public IQueryable<Theme> Themes => new DatabaseContext(connectionString).Themes;
    public IQueryable<Test> Tests => new DatabaseContext(connectionString).Tests;
    public IQueryable<TaskForTest> Tasks => new DatabaseContext(connectionString).Tasks;
    public IQueryable<UserDevice> UserDevices => new DatabaseContext(connectionString).UserDevices;
    public async Task<bool> Insert<T>(T entityItem) where T : class
    {
        try
        {
            await using var context = new DatabaseContext(connectionString);
            await context.AddAsync(entityItem);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
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