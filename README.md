# 🎵 YC3_DAT_VE_CONCERT - Concert Ticket Booking API

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

> 🎫 Hệ thống API RESTful cho đặt vé concert được xây dựng với .NET 9 và MySQL, hỗ trợ đầy đủ chức năng quản lý sự kiện, đặt vé và thống kê.

---

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)
- [Tính năng chính](#-tính-năng-chính)
- [Kiến trúc hệ thống](#️-kiến-trúc-hệ-thống)
- [Cài đặt](#-cài-đặt)
- [Cấu hình](#️-cấu-hình)
- [API Endpoints](#-api-endpoints)
- [Database Schema](#️-database-schema)
- [Dependency Injection](#-dependency-injection)
- [Testing](#-testing)
- [Đóng góp](#-đóng-góp)
- [License](#-license)
- [Liên hệ](#-liên-hệ--hỗ-trợ)

---

## 🎯 Tổng quan

**YC3_DAT_VE_CONCERT** là một RESTful API hiện đại được xây dựng bằng .NET 9 và C# 13, sử dụng Entity Framework Core với MySQL. Hệ thống cung cấp giải pháp toàn diện cho việc quản lý và đặt vé concert, bao gồm:

- ✅ Xác thực và phân quyền người dùng
- 🎪 Quản lý sự kiện và địa điểm tổ chức
- 🎫 Quản lý vé và đặt chỗ
- 👥 Quản lý khách hàng
- 📦 Xử lý đơn hàng
- 📊 Thống kê doanh thu và vé bán

---

## 🛠️ Công nghệ sử dụng

### **Backend Framework**
- ![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet) **.NET 9** với **C# 13**
- ![ASP.NET](https://img.shields.io/badge/ASP.NET-Core-512BD4?logo=dotnet) **ASP.NET Core Web API**
- ![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?logo=dotnet) **Entity Framework Core**

### **Database**
- ![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white) **Microsoft SQL Server 2022** (hoặc 2019/2017)

### **Tools & Libraries**
- 📝 **Swagger/OpenAPI** - API Documentation
- 🔐 **JWT Authentication** - Security
- 💉 **Dependency Injection** - ASP.NET Core DI Container
- 🔄 **AutoMapper** - Object mapping
- ✅ **Data Annotations** - Model validation

---

## 🎨 Tính năng chính

### 🔐 **Authentication (Auth)**
- Đăng ký tài khoản mới
- Đăng nhập và xác thực
- Quản lý phiên đăng nhập

### 🎪 **Event Management**
- ✨ Tạo và quản lý sự kiện concert
- 📅 Lên lịch sự kiện theo thời gian
- 🏟️ Liên kết với địa điểm tổ chức (Venue)
- 🔍 Tìm kiếm và lọc sự kiện

### 🎫 **Ticket Management**
- 🎟️ Tạo và quản lý vé cho từng sự kiện
- 💺 Quản lý số ghế và vị trí ngồi (Mã vé: OrderId)
- ✅ Kiểm tra trạng thái vé
- 🔒 Đặt vé theo sự kiện

### 👥 **Customer Management**
- 📝 Đăng ký thông tin khách hàng
- 📧 Quản lý email và số điện thoại
- 📊 Lịch sử mua vé của khách hàng
- 🔐 Bảo mật thông tin cá nhân

### 📦 **Order Management**
- 🛒 Tạo đơn hàng đặt vé (OrderId)
- 💳 Xử lý thanh toán (pending/completed/cancelled)
- 📋 Quản lý nhiều vé trong một đơn hàng
- 📧 Xác nhận đơn hàng

### 📊 **Statistical**
- 💰 Thống kê tổng doanh thu
- 🎫 Tổng số vé đã bán
- 👥 Tổng số khách hàng
- 📈 Báo cáo theo thời gian
- 🏆 Top sự kiện bán chạy nhất

---

## 🏗️ Kiến trúc hệ thống
```
YC3_DAT_VE_CONCERT/
│
├── 📁 Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── EventController.cs
│   ├── TicketController.cs
│   ├── CustomerController.cs
│   ├── OrderController.cs
│   └── StatisticalController.cs
│
├── 📁 Services/            # Business Logic Layer
│   ├── IStatisticalService.cs
│   ├── StatisticalService.cs
│   ├── IEventService.cs
│   ├── EventService.cs
│   └── ...
│
├── 📁 Models/              # Entity Models
│   ├── Customer.cs
│   ├── Event.cs
│   ├── Venue.cs
│   ├── Ticket.cs
│   └── Order.cs
│
├── 📁 DTOs/                # Data Transfer Objects
│   ├── CreateEventDto.cs
│   ├── UpdateEventDto.cs
│   ├── EventResponseDto.cs
│   └── ...
│
├── 📁 Data/                # Database Context
│   └── ApplicationDbContext.cs
│
├── 📁 Migrations/          # EF Core Migrations
│   └── InitialMigration.cs
│
├── 📁 Validations/         # Custom Validators
│   └── FutureDateAttribute.cs
│
├── 📄 Program.cs           # Application Entry Point
└── 📄 appsettings.json     # Configuration
```

---

## 🚀 Cài đặt

### **Yêu cầu hệ thống**

- ✅ [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- ✅ [SQL Server 2022](https://www.microsoft.com/sql-server/sql-server-downloads) (Express/Developer/Standard/Enterprise)
  - Hoặc SQL Server 2019/2017
  - Hoặc [LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (cho development)
- ✅ [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) - Optional
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/)

---

### **Bước 1: Clone Repository**
```bash
git clone https://github.com/NguyenDinhNam28803/YC3_DAT_VE_CONCERT.git
cd YC3_DAT_VE_CONCERT
```

---

### **Bước 2: Cài đặt Dependencies**
```bash
dotnet restore
```

**Packages cần thiết:**
```bash
# Entity Framework Core cho SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# EF Core Tools cho migrations
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Design package cho migrations
dotnet add package Microsoft.EntityFrameworkCore.Design
```

---

### **Bước 3: Cấu hình SQL Server**

#### **Option 1: Sử dụng SQL Server LocalDB (Khuyên dùng cho Development)**

LocalDB đã được cài sẵn với Visual Studio, không cần cài đặt thêm.

**Connection String:**
```json
"Server=(localdb)\\mssqllocaldb;Database=ConcertDB;Trusted_Connection=true;MultipleActiveResultSets=true"
```

#### **Option 2: Sử dụng SQL Server Express/Developer**

**Cài đặt SQL Server:**
1. Tải [SQL Server 2022 Express](https://www.microsoft.com/sql-server/sql-server-downloads)
2. Chọn "Basic" installation
3. Lưu lại tên instance (thường là `SQLEXPRESS`)

**Tạo Database:**
```sql
-- Mở SQL Server Management Studio (SSMS)
-- Hoặc dùng sqlcmd

-- Tạo database
CREATE DATABASE ConcertDB;
GO

-- Sử dụng database
USE ConcertDB;
GO

-- Kiểm tra
SELECT DB_NAME();
```

**Connection String:**
```json
"Server=localhost\\SQLEXPRESS;Database=ConcertDB;Trusted_Connection=true;TrustServerCertificate=true"
```

#### **Option 3: SQL Server với SQL Authentication**
```sql
-- Tạo login
CREATE LOGIN concert_user WITH PASSWORD = 'YourStrongPassword123!';
GO

-- Tạo database
CREATE DATABASE ConcertDB;
GO

-- Gán quyền
USE ConcertDB;
GO
CREATE USER concert_user FOR LOGIN concert_user;
GO
ALTER ROLE db_owner ADD MEMBER concert_user;
GO
```

**Connection String:**
```json
"Server=localhost\\SQLEXPRESS;Database=ConcertDB;User Id=concert_user;Password=YourStrongPassword123!;TrustServerCertificate=true"
```

---

### **Bước 4: Cấu hình Connection String**

Mở file `appsettings.json` và cập nhật:
```json
{
  "ConnectionStrings": {
    // Option 1: LocalDB (Development)
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ConcertDB;Trusted_Connection=true;MultipleActiveResultSets=true",
    
    // Option 2: SQL Server Express với Windows Authentication
    // "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ConcertDB;Trusted_Connection=true;TrustServerCertificate=true",
    
    // Option 3: SQL Server với SQL Authentication
    // "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ConcertDB;User Id=concert_user;Password=YourStrongPassword123!;TrustServerCertificate=true",
    
    // Option 4: Remote SQL Server
    // "DefaultConnection": "Server=your-server.database.windows.net;Database=ConcertDB;User Id=your-username;Password=your-password;Encrypt=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

**Giải thích các tham số:**
- `Server`: Tên server hoặc địa chỉ IP
- `Database`: Tên database
- `Trusted_Connection=true`: Dùng Windows Authentication
- `User Id` & `Password`: Dùng SQL Authentication
- `TrustServerCertificate=true`: Bỏ qua SSL certificate validation (development)
- `MultipleActiveResultSets=true`: Cho phép nhiều query đồng thời

---

### **Bước 5: Cấu hình DbContext trong Program.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext với SQL Server
builder.Services.AddDbContext(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
    ));

// ... rest of configuration
```

---

### **Bước 6: Chạy Migrations**

**Visual Studio:**
```powershell
# Mở Package Manager Console
Tools > NuGet Package Manager > Package Manager Console

# Tạo migration đầu tiên (nếu chưa có)
Add-Migration InitialCreate

# Apply migration lên database
Update-Database

# Xem các migrations
Get-Migration

# Rollback migration (nếu cần)
Update-Database -Migration PreviousMigrationName
```

**Command Line:**
```bash
# Cài đặt EF Core tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Kiểm tra version
dotnet ef --version

# Tạo migration
dotnet ef migrations add InitialCreate

# Xem script SQL sẽ được chạy
dotnet ef migrations script

# Apply migration
dotnet ef database update

# Xem danh sách migrations
dotnet ef migrations list

# Remove migration cuối cùng (chưa apply)
dotnet ef migrations remove

# Drop database (cẩn thận!)
dotnet ef database drop
```

**Kiểm tra trong SSMS:**
```sql
USE ConcertDB;
GO

-- Xem các bảng
SELECT * FROM INFORMATION_SCHEMA.TABLES;

-- Xem migration history
SELECT * FROM __EFMigrationsHistory;

-- Xem cấu trúc bảng
sp_help 'customers';
```

---

### **Bước 7: Chạy ứng dụng**

**Visual Studio:**
- Nhấn `F5` (Debug) hoặc `Ctrl+F5` (Run without debugging)

**Command Line:**
```bash
dotnet build
dotnet run
```

**Truy cập Swagger UI:**
```
https://localhost:7153/
hoặc
http://localhost:5015/
```

---

## ⚙️ Cấu hình

### **Program.cs - Dependency Injection**
```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Add Controllers
builder.Services.AddControllers();

// Register Services
builder.Services.AddScoped<IStatisticalService, StatisticalService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Concert Ticket Booking API", 
        Version = "v1",
        Description = "API for managing concert ticket bookings"
    });
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Concert API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 📡 API Endpoints

### **🔐 Authentication**

| Method | Endpoint | Description | Body |
|--------|----------|-------------|------|
| `POST` | `/api/Auth/register` | Đăng ký tài khoản mới | `{ email, password, name }` |
| `POST` | `/api/Auth/login` | Đăng nhập | `{ email, password }` |

---

### **🎪 Events**

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/api/Event` | Lấy danh sách sự kiện | `?page=1&limit=10` |
| `GET` | `/api/Event/{id}` | Lấy chi tiết sự kiện | `id` |
| `POST` | `/api/Event` | Tạo sự kiện mới | Body: Event object |
| `PUT` | `/api/Event/{id}` | Cập nhật sự kiện | `id` + Body |
| `DELETE` | `/api/Event/{id}` | Xóa sự kiện | `id` |
| `GET` | `/api/Event/search` | Tìm kiếm sự kiện | `?q=keyword` |

**Example Request - Tạo sự kiện:**
```json
POST /api/Event
Content-Type: application/json

{
  "name": "Rock Concert 2024",
  "date": "2024-12-31T20:00:00",
  "venueId": 1,
  "description": "Amazing rock concert with international artists"
}
```

**Example Response:**
```json
{
  "id": 1,
  "name": "Rock Concert 2024",
  "date": "2024-12-31T20:00:00",
  "venueName": "Nhà Hát Lớn Hà Nội",
  "venueLocation": "1 Tràng Tiền, Hoàn Kiếm, Hà Nội",
  "description": "Amazing rock concert with international artists",
  "totalTicketsSold": 0,
  "availableSeats": 1200
}
```

---

### **🎫 Tickets**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Ticket` | Lấy danh sách vé |
| `GET` | `/api/Ticket/{id}` | Lấy chi tiết vé |
| `POST` | `/api/Ticket` | Tạo vé mới |
| `GET` | `/api/Ticket/event/{eventId}` | Lấy vé theo sự kiện |
| `GET` | `/api/Ticket/available/{eventId}` | Lấy vé còn trống |

**Example - Lấy vé theo sự kiện:**
```bash
GET /api/Ticket/event/1
```

---

### **👥 Customers**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Customer` | Lấy danh sách khách hàng |
| `GET` | `/api/Customer/{id}` | Lấy chi tiết khách hàng |
| `POST` | `/api/Customer` | Tạo khách hàng mới |
| `PUT` | `/api/Customer/{id}` | Cập nhật thông tin |
| `DELETE` | `/api/Customer/{id}` | Xóa khách hàng |
| `GET` | `/api/Customer/search` | Tìm kiếm khách hàng |

**Example Request:**
```json
POST /api/Customer
Content-Type: application/json

{
  "name": "Nguyen Van A",
  "email": "nguyenvana@example.com",
  "phone": "0123456789",
  "password": "password123"
}
```

---

### **📦 Orders**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Order` | Lấy danh sách đơn hàng |
| `GET` | `/api/Order/{id}` | Lấy chi tiết đơn hàng |
| `POST` | `/api/Order` | Tạo đơn hàng mới |
| `PUT` | `/api/Order/{id}/status` | Cập nhật trạng thái |
| `GET` | `/api/Order/customer/{customerId}` | Lấy đơn hàng theo khách |

**Example Request - Tạo đơn hàng:**
```json
POST /api/Order
Content-Type: application/json

{
  "customerId": 1,
  "tickets": [
    {
      "eventId": 1,
      "seatNumber": "A12"
    },
    {
      "eventId": 1,
      "seatNumber": "A13"
    }
  ]
}
```

**Example Response:**
```json
{
  "id": 1,
  "customerId": 1,
  "customerName": "Nguyen Van A",
  "orderDate": "2024-01-10T10:30:00",
  "status": "pending",
  "amount": 1000000,
  "tickets": [
    {
      "id": 1,
      "eventName": "Rock Concert 2024",
      "eventDate": "2024-12-31T20:00:00",
      "seatNumber": "A12"
    },
    {
      "id": 2,
      "eventName": "Rock Concert 2024",
      "eventDate": "2024-12-31T20:00:00",
      "seatNumber": "A13"
    }
  ]
}
```

---

### **📊 Statistics**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Statistical/overall` | Thống kê tổng quan |
| `GET` | `/api/Statistical/events` | Thống kê theo sự kiện |
| `GET` | `/api/Statistical/revenue` | Thống kê doanh thu |
| `GET` | `/api/Statistical/top-events` | Top sự kiện bán chạy |

**Example Response - Overall:**
```json
{
  "totalCustomers": 1250,
  "totalEvents": 45,
  "totalOrders": 3890,
  "totalRevenue": 1250000000,
  "activeEvents": 12,
  "averageTicketPrice": 500000
}
```

---

## 🗄️ Database Schema

### **ERD Diagram**
```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   Customers     │       │     Orders      │       │    Tickets      │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ id (PK)         │◄─────┤│ id (PK)         │       │ id (PK)         │
│ name            │   1:N │ customer_id (FK)│       │ event_id (FK)   │
│ email (unique)  │       │ order_date      │◄─────┤│ customer_id(FK) │
│ phone           │       │ status          │   1:N │ order_id (FK)   │
│ password        │       │ amount          │       │ seat_number     │
│ created_at      │       │ created_at      │       │ purchase_date   │
└─────────────────┘       └─────────────────┘       └─────────────────┘
                                                              │
                                                              │ N:1
                          ┌─────────────────┐                │
                          │     Events      │◄───────────────┘
                          ├─────────────────┤
                          │ id (PK)         │
                          │ name            │
                          │ date            │
                          │ venue_id (FK)   │
                          │ description     │
                          │ created_at      │
                          └─────────────────┘
                                   │
                                   │ N:1
                          ┌─────────────────┐
                          │     Venues      │
                          ├─────────────────┤
                          │ id (PK)         │
                          │ name            │
                          │ location        │
                          │ capacity        │
                          └─────────────────┘
```

### **Bảng thông tin chi tiết**

#### **customers**
| Column | Type | Description |
|--------|------|-------------|
| id | string(26) | ULID - Primary Key |
| name | varchar(100) | Tên khách hàng |
| email | varchar(255) | Email (unique) |
| phone | varchar(20) | Số điện thoại |
| password | varchar(255) | Mật khẩu (hashed) |
| created_at | datetime | Ngày tạo |

#### **events**
| Column | Type | Description |
|--------|------|-------------|
| id | int | Auto increment PK |
| name | varchar(200) | Tên sự kiện |
| date | datetime | Ngày diễn ra |
| venue_id | int | FK to venues |
| description | text | Mô tả sự kiện |

#### **orders**
| Column | Type | Description |
|--------|------|-------------|
| id | string(20) | OrderId: ORD-20240110-001 |
| customer_id | string(26) | FK to customers |
| order_date | datetime | Ngày đặt hàng |
| status | varchar(50) | pending/completed/cancelled |
| amount | decimal(18,2) | Tổng tiền |

#### **tickets**
| Column | Type | Description |
|--------|------|-------------|
| id | string(30) | TicketId: TKT-EV001-A12-001 |
| event_id | int | FK to events |
| customer_id | string(26) | FK to customers |
| order_id | string(20) | FK to orders |
| seat_number | varchar(10) | Số ghế (A1, B12...) |
| purchase_date | datetime | Ngày mua |

---

## 💉 Dependency Injection

### **Service Lifetime**

| Lifetime | Khi nào dùng | Ví dụ |
|----------|--------------|-------|
| **Transient** | Mỗi lần inject tạo instance mới | Logger, Helper |
| **Scoped** | 1 instance per HTTP request | Services, Repositories |
| **Singleton** | 1 instance cho toàn app | Cache, Configuration |

### **Service Registration trong Program.cs**
```csharp
// Scoped - Khuyên dùng cho Services
builder.Services.AddScoped<IStatisticalService, StatisticalService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// Singleton - Cho ID Generator
builder.Services.AddSingleton<IIdGeneratorService, IdGeneratorService>();
```

### **Controller Injection Pattern**
```csharp
[ApiController]
[Route("api/[controller]")]
public class StatisticalController : ControllerBase
{
    private readonly IStatisticalService _statisticalService;
    private readonly ILogger<StatisticalController> _logger;
    
    public StatisticalController(
        IStatisticalService statisticalService,
        ILogger<StatisticalController> logger)
    {
        _statisticalService = statisticalService;
        _logger = logger;
    }
    
    [HttpGet("overall")]
    public async Task<IActionResult> GetOverallStatistics()
    {
        try
        {
            var result = await _statisticalService.GetOverallStatisticsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics");
            return StatusCode(500, "Internal server error");
        }
    }
}
```

**⚠️ Lưu ý quan trọng:** 
- Luôn inject **Interface** (`IStatisticalService`) thay vì concrete class
- Tránh lỗi: `Unable to resolve service for type 'ConcreteClass'`
- Service phải được đăng ký trong `Program.cs` trước khi sử dụng

---

## 🧪 Testing

### **1. Swagger UI Testing**

1. Chạy ứng dụng: `dotnet run`
2. Mở trình duyệt: `https://localhost:7153/`
3. Thử nghiệm các endpoint trực tiếp trên Swagger UI
4. Xem request/response examples

### **2. cURL Examples**
```bash
# 1. Lấy danh sách sự kiện
curl -X GET "https://localhost:7153/api/Event" \
  -H "accept: application/json"

# 2. Tạo customer mới
curl -X POST "https://localhost:7153/api/Customer" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "0123456789",
    "password": "password123"
  }'

# 3. Tạo đơn hàng
curl -X POST "https://localhost:7153/api/Order" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "tickets": [
      {"eventId": 1, "seatNumber": "A12"}
    ]
  }'

# 4. Lấy thống kê tổng quan
curl -X GET "https://localhost:7153/api/Statistical/overall" \
  -H "accept: application/json"

# 5. Tìm kiếm sự kiện
curl -X GET "https://localhost:7153/api/Event/search?q=rock" \
  -H "accept: application/json"
```

### **3. Postman Collection**

Import Postman collection từ Swagger:
1. Mở Swagger UI: `https://localhost:7153/`
2. Copy URL: `https://localhost:7153/swagger/v1/swagger.json`
3. Postman > Import > Link > Paste URL
4. Import và test

### **4. Unit Testing (Nếu có)**
```bash
# Chạy tests
dotnet test

# Với coverage
dotnet test /p:CollectCoverage=true
```

---

## 🤝 Đóng góp

Chúng tôi rất hoan nghênh mọi đóng góp! Để đóng góp vào dự án:

### **Quy trình đóng góp**

1. 🍴 **Fork repository**
```bash
   # Click nút Fork trên GitHub
```

2. 🌿 **Tạo branch mới**
```bash
   git checkout -b feature/AmazingFeature
```

3. ✍️ **Commit changes**
```bash
   git add .
   git commit -m 'Add some AmazingFeature'
```

4. 📤 **Push to branch**
```bash
   git push origin feature/AmazingFeature
```

5. 🔃 **Mở Pull Request**
   - Vào repository gốc trên GitHub
   - Click "New Pull Request"
   - Mô tả chi tiết những thay đổi

### **Coding Guidelines**

- ✅ Tuân thủ C# coding conventions
- ✅ Viết code rõ ràng, dễ hiểu
- ✅ Thêm comments cho logic phức tạp
- ✅ Viết unit tests cho features mới
- ✅ Update README nếu thay đổi API
- ✅ Đảm bảo code build thành công
- ✅ Format code trước khi commit

### **Commit Message Format**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance

**Example:**
```
feat(order): add bulk order creation

- Add endpoint POST /api/Order/bulk
- Support creating multiple orders at once
- Add validation for bulk requests

Closes #123
```

---

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

---

## 👨‍💻 Tác giả

**Nguyễn Đình Nam**

- 🌐 GitHub: [@NguyenDinhNam28803](https://github.com/NguyenDinhNam28803)
- 📧 Email: nguyendinhnam241209@gmail.com

---

## 🙏 Acknowledgments

Cảm ơn các công nghệ và tài liệu tham khảo:

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core) - Official ASP.NET docs
- [Entity Framework Core](https://docs.microsoft.com/ef/core) - EF
