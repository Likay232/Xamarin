using WebApi.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConnectionCreate();

builder.RegistrationEndpoints();

var app = builder.Build();

app.MappingEndpoints();

app.Run();