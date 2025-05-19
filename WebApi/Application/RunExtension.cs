using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Infrastructure.Components;

namespace WebApi.Application;

public static class RunExtension
{
    public static void ConnectionCreate(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => $"{type.Namespace}.{type.Name}");
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
        });

        services.AddTransient<DatabaseContext>(_ => new DatabaseContext(connectionString));
        services.AddScoped(_ => new DataComponent(connectionString));
    }

    public static void MappingEndpoints(this WebApplication app)
    {
        app.MigrateDatabase();
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseCors("AllowedOrigins");
        
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi");
            options.RoutePrefix = "swagger";
        });
        
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    public static void RegistrationEndpoints(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
    }

    
    private static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}