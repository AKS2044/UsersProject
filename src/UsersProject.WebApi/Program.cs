using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using UsersProject.Data.Contexts;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Managers;
using UsersProject.Logic.Services;
using UsersProject.WebApi.Middlewares;
using UsersProject.WebApi.Settings;

//Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Information()
//            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//            .Enrich.FromLogContext()
//            .WriteTo.Console()
//            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
//            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => options.LoginPath = "/account/login");

builder.Services.AddScoped(typeof(IRepositoryManager<>), typeof(RepositoryManager<>));
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API",
        Description = "User API",
        Contact = new OpenApiContact() { }
    });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    config.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
                    {
                    {
                        securitySchema, new[] { "Bearer" }
                    }
                    };
    config.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHsts();
app.UseMiddleware<JwtMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();