using Microsoft.EntityFrameworkCore;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Service;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Book Consert ticket API",
        Version = "v1"
    });
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.AllowAnyOrigin() // <-- origin front-end
              .AllowAnyHeader()
              .AllowAnyMethod();
        // .AllowCredentials(); // Bật nếu bạn gửi cookies/credentials. Nếu bật, không dùng AllowAnyOrigin.
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Concert ticket API v1");
    options.RoutePrefix = string.Empty; // Để truy cập Swagger UI tại gốc của ứng dụng
});

app.UseCors("AllowLocalDev");

app.MapGet("/", () => "Welcome to the Book Concert Ticket API!");
app.UseRouting();

app.MapGet("/swagger", () => Results.Redirect("/swagger"));

app.MapPost("/login", async (IAuthService authService, LoginDto loginDto) =>
{
    try
    {
        var userInfo = await authService.Login(loginDto);
        return Results.Ok(new 
        { 
            success = true,
            message = "Login successful",
            data = userInfo 
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new 
        { 
            success = false,
            message = "Login failed",
            details = ex.Message 
        });
    }
});

app.MapPost("/register", async (IAuthService authService, RegisterDto registerDto) =>
{
    try
    {
        await authService.Register(registerDto);
        return Results.Ok(new 
        { 
            success = true,
            message = "Registration successful" 
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new 
        { 
            success = false,
            message = "Registration failed",
            detail = ex.Message 
        });
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
