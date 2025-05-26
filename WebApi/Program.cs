using WebApi.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConnectionCreate();
builder.Services.AddJwtAuthentication();

builder.RegistrationEndpoints();

builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(5000));

var app = builder.Build();

app.MappingEndpoints();

app.Run();