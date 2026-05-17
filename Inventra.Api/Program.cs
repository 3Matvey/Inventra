using Inventra.Api;
using Inventra.Api.Hubs;
using Inventra.Api.Middleware;
using Inventra.Application;
using Inventra.Infrastructure.Data;
using Inventra.Infrastructure.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddPortableSecrets();
var configuration = builder.Configuration;


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddApplicationServices()
    .AddDataServices(configuration)
    .AddIdentityServices(configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InventoryDiscussionHub>("/hubs/inventory-discussion");

app.Run();
