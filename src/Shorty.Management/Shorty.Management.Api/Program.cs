using Microsoft.EntityFrameworkCore;
using Shorty.Management.Api.Extensions;
using Shorty.Management.Api.Handlers;
using Shorty.Management.Api.Services;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddSecurity();
builder.Services.AddFeatures();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddJsonOptions();

var app = builder.Build();

app.InitDb();
app.UseForwardedHeaders();
app.UseExceptionHandler();
//app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapFeatures();
app.Run();