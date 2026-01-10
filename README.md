# YC3_DAT_VE_CONCERT

Ứng dụng API cho hệ thống đặt vé concert (Book Concert Ticket API).

## Tổng quan
YC3_DAT_VE_CONCERT là một RESTful API xây dựng bằng .NET 9 và C# 13, sử dụng Entity Framework Core với SQL Server. Ứng dụng cung cấp các chức năng chính: xác thực, quản lý sự kiện, quản lý vé, quản lý khách hàng, đơn hàng và thống kê doanh thu / vé bán.

## Công nghệ chính
- .NET 9 (C# 13)
- ASP.NET Core Web API
- Entity Framework Core (EF Core) với SQL Server
- Swagger / OpenAPI cho tài liệu API
- Dependency Injection theo chuẩn ASP.NET Core

## Tính năng
- Xác thực (Auth)
- Quản lý sự kiện (Event)
- Quản lý vé (Ticket)
- Quản lý khách hàng (Customer)
- Quản lý đơn hàng (Order)
- Thống kê: tổng vé bán, tổng doanh thu, tổng khách hàng (Statistical)
- API documentation qua Swagger UI

## Yêu cầu (Prerequisites)
- .NET 9 SDK
- Visual Studio 2022 (hoặc VS Code)
- SQL Server / SQL Server Express
- (Tùy chọn) dotnet-ef tools nếu bạn dùng CLI migrations

## Cài đặt & chạy nhanh
1. Clone repository:
   - git clone https://github.com/NguyenDinhNam28803/YC3_DAT_VE_CONCERT

2. Cấu hình chuỗi kết nối:
   - Mở `appsettings.json` và chỉnh `ConnectionStrings:DefaultConnection` trỏ tới SQL Server của bạn.

3. Tạo/áp dụng migration (nếu cần):
   - Visual Studio: mở __Package Manager Console__ và chạy:
     - `Update-Database`
   - Hoặc CLI:
     - `dotnet ef database update`

   (Nếu chưa cài: `dotnet tool install --global dotnet-ef` và thêm package `Microsoft.EntityFrameworkCore.Design`)

4. Chạy ứng dụng:
   - Visual Studio: mở solution, chọn project `YC3_DAT_VE_CONCERT` làm Startup và nhấn __F5__ (Debug) hoặc __Ctrl+F5__.
   - CLI:
     - `dotnet restore`
     - `dotnet build`
     - `dotnet run`

5. Mở Swagger UI:
   - Vì Swagger được cấu hình với RoutePrefix rỗng, truy cập root ứng dụng https://localhost:7153/ hoặc https://localhost:5015/ để xem giao diện Swagger và thử các endpoint.

## Endpoints chính (tổng quan)
- `POST /api/Auth` — đăng nhập/đăng ký (tùy cấu trúc)
- `GET/POST/PUT/DELETE /api/Event` — quản lý sự kiện
- `GET/POST/PUT/DELETE /api/Ticket` — quản lý vé
- `GET/POST/PUT/DELETE /api/Customer` — quản lý khách hàng
- `GET/POST /api/Order` — quản lý đơn hàng
- `GET /api/Statitiscal` — lấy dữ liệu thống kê (lưu ý tên controller là `StatitiscalController` trong mã nguồn)

(Tài liệu chi tiết và body của từng endpoint có trong Swagger UI.)

## Lưu ý về Dependency Injection
Các service đã được đăng ký trong `Program.cs` theo mẫu DI:
- `builder.Services.AddScoped<IStatisticalService, StatisticalService>();`
Vì vậy các Controller nên phụ thuộc vào interface (ví dụ `IStatisticalService`) thay vì concrete class để tránh lỗi khi khởi tạo.

## Migrations & Database
- Thư mục `Migrations` đã chứa migration khởi tạo (`Initial Migration`).
- Nếu muốn tạo migration mới:
  - `Add-Migration YourMigrationName` (Package Manager Console)
  - Hoặc CLI: `dotnet ef migrations add YourMigrationName`

## Test & Debug
- Dùng Swagger UI để kiểm thử nhanh các endpoint.
- Dùng Postman / Insomnia để test API với JSON payloads.
- Kiểm tra logs / exceptions trong Visual Studio Output khi debug.

## Đóng góp
- Fork repository, tạo branch feature, commit, mở Pull Request.
- Viết unit tests nếu có thay đổi logic quan trọng.

## License
- Đề xuất: MIT (chỉnh sửa theo nhu cầu).

## Liên hệ
- Người duy trì repository: NguyenDinhNam... (xem thông tin liên hệ trong repo)
