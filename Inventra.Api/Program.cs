using Inventra.Api;
using Inventra.Api.Hubs;
using Inventra.Api.Middleware;
using Inventra.Application;
using Inventra.Infrastructure.Data;
using Inventra.Infrastructure.Identity;
using Inventra.Infrastructure.Cloudinary;
using Inventra.Infrastructure.Caching;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddFrontendCors(configuration);
builder.Services.AddApplicationServices()
    .AddDataServices(configuration)
    .AddIdentityServices(configuration)
    .AddCachingServices(configuration)
    .AddCloudinaryServices(configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("OpenApi:Enabled"))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InventoryDiscussionHub>("/hubs/inventory-discussion");

app.Run();
