using Application.DTO.User;
using Application.Interfaces;
using Application.Services;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Middlewares;
using Serilog;
using Serilog.Events;
using System.Runtime.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


// Serilog initialization
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CourseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

try
{
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<IAnswerService, AnswerService>();
    builder.Services.AddScoped<ICourseService, CourseService>();
    builder.Services.AddScoped<IChapterService, ChapterService>();
    builder.Services.AddScoped<IEnrolledCourseService, EnrolledCourseService>();
    builder.Services.AddScoped<IExamAttemptService, ExamAttemptService>();
    builder.Services.AddScoped<IExamService, ExamService>();
    builder.Services.AddScoped<IExamQuestionAnswerService, ExamQuestionAnswerService>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IQuestionService, QuestionService>();
    builder.Services.AddScoped<IUserRoleService, UserRoleService>();
    builder.Services.AddScoped<IAuthService, AuthService>();


    // Serilog configuration
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
            options.SwaggerEndpoint("/openapi/v1.json", "API v1")
        );
    }

    // Create default admin
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CourseContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var role = context.Roles.Find(1);
        if (role == null) throw new NotFoundException("Specified role not found");

        var user = context.Users.FirstOrDefault(u => u.Email == "admin@mail.com");
        if (user == null)
        {
            var admin = new User
            {
                Name = "admin",
                Surname = "admin",
                Email = "admin@mail.com",
                DateOfRegistration = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "admin123");

            await context.Users.AddAsync(admin);
            await context.UserRoles.AddAsync(new UserRole { Role = role, User = admin });
            await context.SaveChangesAsync();
        }
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandler>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
