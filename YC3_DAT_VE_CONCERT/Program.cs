using Microsoft.EntityFrameworkCore;
using PayOS;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Tài liệu về Swagger dự án : https://localhost:7153/swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Book Consert ticket API",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // enable [SwaggerOperation], [SwaggerResponse]
    options.EnableAnnotations();
});

// register PayOs options and http client
//builder.Services.Configure<PayOsOptions>(builder.Configuration.GetSection("PayOs"));
builder.Services.AddHttpClient<PayOsService>((sp, client) =>
{
    var cfg = builder.Configuration.GetSection("PayOs");
    client.BaseAddress = new Uri(cfg["BaseUrl"] ?? "https://api.payos.example");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5));
builder.Services.Configure<YC3_DAT_VE_CONCERT.Model.EmailSetting>(builder.Configuration.GetSection("EmailSettings"));

// Add Transient
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPayOSService, PayOsService>();

// Add Scopped
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStatisticalService, StatisticalService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IAppConfigService, AppConfigService>();

// Add Singleton
builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
builder.Services.AddSingleton<IPdfService, PdfService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.AllowAnyOrigin() // <-- Allow requests from any origin
              .AllowAnyHeader()
              .AllowAnyMethod();
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
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
