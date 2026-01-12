using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YC3_DAT_VE_CONCERT.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "venues",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venues", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                    table.ForeignKey(
                        name: "FK_customers_Roles_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    venue_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ticket_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total_seat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    purchase_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    seat_number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.id);
                    table.ForeignKey(
                        name: "FK_tickets_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "venues",
                columns: new[] { "id", "capacity", "location", "name" },
                values: new object[,]
                {
                    { 1, 1200, "1 Tràng Tiền, Hoàn Kiếm, Hà Nội", "Nhà Hát Lớn Hà Nội" },
                    { 2, 40000, "Mỹ Đình, Nam Từ Liêm, Hà Nội", "Sân vận động Mỹ Đình" },
                    { 3, 2000, "4 Phạm Ngọc Thạch, Quận 1, TP. Hồ Chí Minh", "Nhà Văn Hóa Thanh Niên" }
                });

            migrationBuilder.InsertData(
                table: "events",
                columns: new[] { "id", "date", "description", "name", "ticket_price", "total_seat", "venue_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 10, 19, 0, 0, 0, DateTimeKind.Utc), "Chuỗi ban nhạc và DJ biểu diễn tại Sân vận động Mỹ Đình.", "Lễ hội Âm nhạc Xuân", 500000.00m, 35000, 2 },
                    { 2, new DateTime(2026, 6, 12, 19, 30, 0, 0, DateTimeKind.Utc), "Buổi hòa nhạc cổ điển tại Nhà Hát Lớn Hà Nội.", "Đêm Nhạc Cổ Điển", 350000.00m, 1000, 1 },
                    { 3, new DateTime(2026, 7, 5, 20, 0, 0, 0, DateTimeKind.Utc), "Các ban nhạc indie và nghệ sĩ trẻ biểu diễn ở Nhà Văn Hóa Thanh Niên.", "Indie Showcase TP.HCM", 200000.00m, 800, 3 },
                    { 4, new DateTime(2026, 8, 20, 20, 0, 0, 0, DateTimeKind.Utc), "Đêm nhạc jazz lãng mạn tại Hà Nội.", "Đêm Jazz Mùa Hè", 300000.00m, 900, 1 }
                });

            migrationBuilder.InsertData(
                table: "tickets",
                columns: new[] { "id", "customer_id", "event_id", "order_id", "price", "purchase_date", "seat_number", "status" },
                values: new object[,]
                {
                    { 1, null, 1, null, 500000.00m, null, "A1", 0 },
                    { 2, null, 1, null, 500000.00m, null, "A2", 0 },
                    { 3, null, 1, null, 500000.00m, null, "A3", 0 },
                    { 4, null, 1, null, 500000.00m, null, "A4", 0 },
                    { 5, null, 1, null, 500000.00m, null, "A5", 0 },
                    { 6, null, 1, null, 500000.00m, null, "A6", 0 },
                    { 7, null, 1, null, 500000.00m, null, "A7", 0 },
                    { 8, null, 1, null, 500000.00m, null, "A8", 0 },
                    { 9, null, 1, null, 500000.00m, null, "A9", 0 },
                    { 10, null, 1, null, 500000.00m, null, "A10", 0 },
                    { 11, null, 1, null, 500000.00m, null, "A11", 0 },
                    { 12, null, 1, null, 500000.00m, null, "A12", 0 },
                    { 13, null, 1, null, 500000.00m, null, "A13", 0 },
                    { 14, null, 1, null, 500000.00m, null, "A14", 0 },
                    { 15, null, 1, null, 500000.00m, null, "A15", 0 },
                    { 16, null, 1, null, 500000.00m, null, "A16", 0 },
                    { 17, null, 1, null, 500000.00m, null, "A17", 0 },
                    { 18, null, 1, null, 500000.00m, null, "A18", 0 },
                    { 19, null, 1, null, 500000.00m, null, "A19", 0 },
                    { 20, null, 1, null, 500000.00m, null, "A20", 0 },
                    { 21, null, 1, null, 500000.00m, null, "A21", 0 },
                    { 22, null, 1, null, 500000.00m, null, "A22", 0 },
                    { 23, null, 1, null, 500000.00m, null, "A23", 0 },
                    { 24, null, 1, null, 500000.00m, null, "A24", 0 },
                    { 25, null, 1, null, 500000.00m, null, "A25", 0 },
                    { 26, null, 1, null, 500000.00m, null, "A26", 0 },
                    { 27, null, 1, null, 500000.00m, null, "A27", 0 },
                    { 28, null, 1, null, 500000.00m, null, "A28", 0 },
                    { 29, null, 1, null, 500000.00m, null, "A29", 0 },
                    { 30, null, 1, null, 500000.00m, null, "A30", 0 },
                    { 31, null, 1, null, 500000.00m, null, "A31", 0 },
                    { 32, null, 1, null, 500000.00m, null, "A32", 0 },
                    { 33, null, 1, null, 500000.00m, null, "A33", 0 },
                    { 34, null, 1, null, 500000.00m, null, "A34", 0 },
                    { 35, null, 1, null, 500000.00m, null, "A35", 0 },
                    { 36, null, 1, null, 500000.00m, null, "A36", 0 },
                    { 37, null, 1, null, 500000.00m, null, "A37", 0 },
                    { 38, null, 1, null, 500000.00m, null, "A38", 0 },
                    { 39, null, 1, null, 500000.00m, null, "A39", 0 },
                    { 40, null, 1, null, 500000.00m, null, "A40", 0 },
                    { 41, null, 1, null, 500000.00m, null, "A41", 0 },
                    { 42, null, 1, null, 500000.00m, null, "A42", 0 },
                    { 43, null, 1, null, 500000.00m, null, "A43", 0 },
                    { 44, null, 1, null, 500000.00m, null, "A44", 0 },
                    { 45, null, 1, null, 500000.00m, null, "A45", 0 },
                    { 46, null, 1, null, 500000.00m, null, "A46", 0 },
                    { 47, null, 1, null, 500000.00m, null, "A47", 0 },
                    { 48, null, 1, null, 500000.00m, null, "A48", 0 },
                    { 49, null, 1, null, 500000.00m, null, "A49", 0 },
                    { 50, null, 1, null, 500000.00m, null, "A50", 0 },
                    { 51, null, 1, null, 500000.00m, null, "A51", 0 },
                    { 52, null, 1, null, 500000.00m, null, "A52", 0 },
                    { 53, null, 1, null, 500000.00m, null, "A53", 0 },
                    { 54, null, 1, null, 500000.00m, null, "A54", 0 },
                    { 55, null, 1, null, 500000.00m, null, "A55", 0 },
                    { 56, null, 1, null, 500000.00m, null, "A56", 0 },
                    { 57, null, 1, null, 500000.00m, null, "A57", 0 },
                    { 58, null, 1, null, 500000.00m, null, "A58", 0 },
                    { 59, null, 1, null, 500000.00m, null, "A59", 0 },
                    { 60, null, 1, null, 500000.00m, null, "A60", 0 },
                    { 61, null, 1, null, 500000.00m, null, "A61", 0 },
                    { 62, null, 1, null, 500000.00m, null, "A62", 0 },
                    { 63, null, 1, null, 500000.00m, null, "A63", 0 },
                    { 64, null, 1, null, 500000.00m, null, "A64", 0 },
                    { 65, null, 1, null, 500000.00m, null, "A65", 0 },
                    { 66, null, 1, null, 500000.00m, null, "A66", 0 },
                    { 67, null, 1, null, 500000.00m, null, "A67", 0 },
                    { 68, null, 1, null, 500000.00m, null, "A68", 0 },
                    { 69, null, 1, null, 500000.00m, null, "A69", 0 },
                    { 70, null, 1, null, 500000.00m, null, "A70", 0 },
                    { 71, null, 1, null, 500000.00m, null, "A71", 0 },
                    { 72, null, 1, null, 500000.00m, null, "A72", 0 },
                    { 73, null, 1, null, 500000.00m, null, "A73", 0 },
                    { 74, null, 1, null, 500000.00m, null, "A74", 0 },
                    { 75, null, 1, null, 500000.00m, null, "A75", 0 },
                    { 76, null, 1, null, 500000.00m, null, "A76", 0 },
                    { 77, null, 1, null, 500000.00m, null, "A77", 0 },
                    { 78, null, 1, null, 500000.00m, null, "A78", 0 },
                    { 79, null, 1, null, 500000.00m, null, "A79", 0 },
                    { 80, null, 1, null, 500000.00m, null, "A80", 0 },
                    { 81, null, 1, null, 500000.00m, null, "A81", 0 },
                    { 82, null, 1, null, 500000.00m, null, "A82", 0 },
                    { 83, null, 1, null, 500000.00m, null, "A83", 0 },
                    { 84, null, 1, null, 500000.00m, null, "A84", 0 },
                    { 85, null, 1, null, 500000.00m, null, "A85", 0 },
                    { 86, null, 1, null, 500000.00m, null, "A86", 0 },
                    { 87, null, 1, null, 500000.00m, null, "A87", 0 },
                    { 88, null, 1, null, 500000.00m, null, "A88", 0 },
                    { 89, null, 1, null, 500000.00m, null, "A89", 0 },
                    { 90, null, 1, null, 500000.00m, null, "A90", 0 },
                    { 91, null, 1, null, 500000.00m, null, "A91", 0 },
                    { 92, null, 1, null, 500000.00m, null, "A92", 0 },
                    { 93, null, 1, null, 500000.00m, null, "A93", 0 },
                    { 94, null, 1, null, 500000.00m, null, "A94", 0 },
                    { 95, null, 1, null, 500000.00m, null, "A95", 0 },
                    { 96, null, 1, null, 500000.00m, null, "A96", 0 },
                    { 97, null, 1, null, 500000.00m, null, "A97", 0 },
                    { 98, null, 1, null, 500000.00m, null, "A98", 0 },
                    { 99, null, 1, null, 500000.00m, null, "A99", 0 },
                    { 100, null, 1, null, 500000.00m, null, "A100", 0 },
                    { 101, null, 2, null, 350000.00m, null, "B1", 0 },
                    { 102, null, 2, null, 350000.00m, null, "B2", 0 },
                    { 103, null, 2, null, 350000.00m, null, "B3", 0 },
                    { 104, null, 2, null, 350000.00m, null, "B4", 0 },
                    { 105, null, 2, null, 350000.00m, null, "B5", 0 },
                    { 106, null, 2, null, 350000.00m, null, "B6", 0 },
                    { 107, null, 2, null, 350000.00m, null, "B7", 0 },
                    { 108, null, 2, null, 350000.00m, null, "B8", 0 },
                    { 109, null, 2, null, 350000.00m, null, "B9", 0 },
                    { 110, null, 2, null, 350000.00m, null, "B10", 0 },
                    { 111, null, 2, null, 350000.00m, null, "B11", 0 },
                    { 112, null, 2, null, 350000.00m, null, "B12", 0 },
                    { 113, null, 2, null, 350000.00m, null, "B13", 0 },
                    { 114, null, 2, null, 350000.00m, null, "B14", 0 },
                    { 115, null, 2, null, 350000.00m, null, "B15", 0 },
                    { 116, null, 2, null, 350000.00m, null, "B16", 0 },
                    { 117, null, 2, null, 350000.00m, null, "B17", 0 },
                    { 118, null, 2, null, 350000.00m, null, "B18", 0 },
                    { 119, null, 2, null, 350000.00m, null, "B19", 0 },
                    { 120, null, 2, null, 350000.00m, null, "B20", 0 },
                    { 121, null, 2, null, 350000.00m, null, "B21", 0 },
                    { 122, null, 2, null, 350000.00m, null, "B22", 0 },
                    { 123, null, 2, null, 350000.00m, null, "B23", 0 },
                    { 124, null, 2, null, 350000.00m, null, "B24", 0 },
                    { 125, null, 2, null, 350000.00m, null, "B25", 0 },
                    { 126, null, 2, null, 350000.00m, null, "B26", 0 },
                    { 127, null, 2, null, 350000.00m, null, "B27", 0 },
                    { 128, null, 2, null, 350000.00m, null, "B28", 0 },
                    { 129, null, 2, null, 350000.00m, null, "B29", 0 },
                    { 130, null, 2, null, 350000.00m, null, "B30", 0 },
                    { 131, null, 2, null, 350000.00m, null, "B31", 0 },
                    { 132, null, 2, null, 350000.00m, null, "B32", 0 },
                    { 133, null, 2, null, 350000.00m, null, "B33", 0 },
                    { 134, null, 2, null, 350000.00m, null, "B34", 0 },
                    { 135, null, 2, null, 350000.00m, null, "B35", 0 },
                    { 136, null, 2, null, 350000.00m, null, "B36", 0 },
                    { 137, null, 2, null, 350000.00m, null, "B37", 0 },
                    { 138, null, 2, null, 350000.00m, null, "B38", 0 },
                    { 139, null, 2, null, 350000.00m, null, "B39", 0 },
                    { 140, null, 2, null, 350000.00m, null, "B40", 0 },
                    { 141, null, 2, null, 350000.00m, null, "B41", 0 },
                    { 142, null, 2, null, 350000.00m, null, "B42", 0 },
                    { 143, null, 2, null, 350000.00m, null, "B43", 0 },
                    { 144, null, 2, null, 350000.00m, null, "B44", 0 },
                    { 145, null, 2, null, 350000.00m, null, "B45", 0 },
                    { 146, null, 2, null, 350000.00m, null, "B46", 0 },
                    { 147, null, 2, null, 350000.00m, null, "B47", 0 },
                    { 148, null, 2, null, 350000.00m, null, "B48", 0 },
                    { 149, null, 2, null, 350000.00m, null, "B49", 0 },
                    { 150, null, 2, null, 350000.00m, null, "B50", 0 },
                    { 151, null, 2, null, 350000.00m, null, "B51", 0 },
                    { 152, null, 2, null, 350000.00m, null, "B52", 0 },
                    { 153, null, 2, null, 350000.00m, null, "B53", 0 },
                    { 154, null, 2, null, 350000.00m, null, "B54", 0 },
                    { 155, null, 2, null, 350000.00m, null, "B55", 0 },
                    { 156, null, 2, null, 350000.00m, null, "B56", 0 },
                    { 157, null, 2, null, 350000.00m, null, "B57", 0 },
                    { 158, null, 2, null, 350000.00m, null, "B58", 0 },
                    { 159, null, 2, null, 350000.00m, null, "B59", 0 },
                    { 160, null, 2, null, 350000.00m, null, "B60", 0 },
                    { 161, null, 2, null, 350000.00m, null, "B61", 0 },
                    { 162, null, 2, null, 350000.00m, null, "B62", 0 },
                    { 163, null, 2, null, 350000.00m, null, "B63", 0 },
                    { 164, null, 2, null, 350000.00m, null, "B64", 0 },
                    { 165, null, 2, null, 350000.00m, null, "B65", 0 },
                    { 166, null, 2, null, 350000.00m, null, "B66", 0 },
                    { 167, null, 2, null, 350000.00m, null, "B67", 0 },
                    { 168, null, 2, null, 350000.00m, null, "B68", 0 },
                    { 169, null, 2, null, 350000.00m, null, "B69", 0 },
                    { 170, null, 2, null, 350000.00m, null, "B70", 0 },
                    { 171, null, 2, null, 350000.00m, null, "B71", 0 },
                    { 172, null, 2, null, 350000.00m, null, "B72", 0 },
                    { 173, null, 2, null, 350000.00m, null, "B73", 0 },
                    { 174, null, 2, null, 350000.00m, null, "B74", 0 },
                    { 175, null, 2, null, 350000.00m, null, "B75", 0 },
                    { 176, null, 2, null, 350000.00m, null, "B76", 0 },
                    { 177, null, 2, null, 350000.00m, null, "B77", 0 },
                    { 178, null, 2, null, 350000.00m, null, "B78", 0 },
                    { 179, null, 2, null, 350000.00m, null, "B79", 0 },
                    { 180, null, 2, null, 350000.00m, null, "B80", 0 },
                    { 181, null, 2, null, 350000.00m, null, "B81", 0 },
                    { 182, null, 2, null, 350000.00m, null, "B82", 0 },
                    { 183, null, 2, null, 350000.00m, null, "B83", 0 },
                    { 184, null, 2, null, 350000.00m, null, "B84", 0 },
                    { 185, null, 2, null, 350000.00m, null, "B85", 0 },
                    { 186, null, 2, null, 350000.00m, null, "B86", 0 },
                    { 187, null, 2, null, 350000.00m, null, "B87", 0 },
                    { 188, null, 2, null, 350000.00m, null, "B88", 0 },
                    { 189, null, 2, null, 350000.00m, null, "B89", 0 },
                    { 190, null, 2, null, 350000.00m, null, "B90", 0 },
                    { 191, null, 2, null, 350000.00m, null, "B91", 0 },
                    { 192, null, 2, null, 350000.00m, null, "B92", 0 },
                    { 193, null, 2, null, 350000.00m, null, "B93", 0 },
                    { 194, null, 2, null, 350000.00m, null, "B94", 0 },
                    { 195, null, 2, null, 350000.00m, null, "B95", 0 },
                    { 196, null, 2, null, 350000.00m, null, "B96", 0 },
                    { 197, null, 2, null, 350000.00m, null, "B97", 0 },
                    { 198, null, 2, null, 350000.00m, null, "B98", 0 },
                    { 199, null, 2, null, 350000.00m, null, "B99", 0 },
                    { 200, null, 2, null, 350000.00m, null, "B100", 0 },
                    { 201, null, 3, null, 200000.00m, null, "C1", 0 },
                    { 202, null, 3, null, 200000.00m, null, "C2", 0 },
                    { 203, null, 3, null, 200000.00m, null, "C3", 0 },
                    { 204, null, 3, null, 200000.00m, null, "C4", 0 },
                    { 205, null, 3, null, 200000.00m, null, "C5", 0 },
                    { 206, null, 3, null, 200000.00m, null, "C6", 0 },
                    { 207, null, 3, null, 200000.00m, null, "C7", 0 },
                    { 208, null, 3, null, 200000.00m, null, "C8", 0 },
                    { 209, null, 3, null, 200000.00m, null, "C9", 0 },
                    { 210, null, 3, null, 200000.00m, null, "C10", 0 },
                    { 211, null, 3, null, 200000.00m, null, "C11", 0 },
                    { 212, null, 3, null, 200000.00m, null, "C12", 0 },
                    { 213, null, 3, null, 200000.00m, null, "C13", 0 },
                    { 214, null, 3, null, 200000.00m, null, "C14", 0 },
                    { 215, null, 3, null, 200000.00m, null, "C15", 0 },
                    { 216, null, 3, null, 200000.00m, null, "C16", 0 },
                    { 217, null, 3, null, 200000.00m, null, "C17", 0 },
                    { 218, null, 3, null, 200000.00m, null, "C18", 0 },
                    { 219, null, 3, null, 200000.00m, null, "C19", 0 },
                    { 220, null, 3, null, 200000.00m, null, "C20", 0 },
                    { 221, null, 3, null, 200000.00m, null, "C21", 0 },
                    { 222, null, 3, null, 200000.00m, null, "C22", 0 },
                    { 223, null, 3, null, 200000.00m, null, "C23", 0 },
                    { 224, null, 3, null, 200000.00m, null, "C24", 0 },
                    { 225, null, 3, null, 200000.00m, null, "C25", 0 },
                    { 226, null, 3, null, 200000.00m, null, "C26", 0 },
                    { 227, null, 3, null, 200000.00m, null, "C27", 0 },
                    { 228, null, 3, null, 200000.00m, null, "C28", 0 },
                    { 229, null, 3, null, 200000.00m, null, "C29", 0 },
                    { 230, null, 3, null, 200000.00m, null, "C30", 0 },
                    { 231, null, 3, null, 200000.00m, null, "C31", 0 },
                    { 232, null, 3, null, 200000.00m, null, "C32", 0 },
                    { 233, null, 3, null, 200000.00m, null, "C33", 0 },
                    { 234, null, 3, null, 200000.00m, null, "C34", 0 },
                    { 235, null, 3, null, 200000.00m, null, "C35", 0 },
                    { 236, null, 3, null, 200000.00m, null, "C36", 0 },
                    { 237, null, 3, null, 200000.00m, null, "C37", 0 },
                    { 238, null, 3, null, 200000.00m, null, "C38", 0 },
                    { 239, null, 3, null, 200000.00m, null, "C39", 0 },
                    { 240, null, 3, null, 200000.00m, null, "C40", 0 },
                    { 241, null, 3, null, 200000.00m, null, "C41", 0 },
                    { 242, null, 3, null, 200000.00m, null, "C42", 0 },
                    { 243, null, 3, null, 200000.00m, null, "C43", 0 },
                    { 244, null, 3, null, 200000.00m, null, "C44", 0 },
                    { 245, null, 3, null, 200000.00m, null, "C45", 0 },
                    { 246, null, 3, null, 200000.00m, null, "C46", 0 },
                    { 247, null, 3, null, 200000.00m, null, "C47", 0 },
                    { 248, null, 3, null, 200000.00m, null, "C48", 0 },
                    { 249, null, 3, null, 200000.00m, null, "C49", 0 },
                    { 250, null, 3, null, 200000.00m, null, "C50", 0 },
                    { 251, null, 3, null, 200000.00m, null, "C51", 0 },
                    { 252, null, 3, null, 200000.00m, null, "C52", 0 },
                    { 253, null, 3, null, 200000.00m, null, "C53", 0 },
                    { 254, null, 3, null, 200000.00m, null, "C54", 0 },
                    { 255, null, 3, null, 200000.00m, null, "C55", 0 },
                    { 256, null, 3, null, 200000.00m, null, "C56", 0 },
                    { 257, null, 3, null, 200000.00m, null, "C57", 0 },
                    { 258, null, 3, null, 200000.00m, null, "C58", 0 },
                    { 259, null, 3, null, 200000.00m, null, "C59", 0 },
                    { 260, null, 3, null, 200000.00m, null, "C60", 0 },
                    { 261, null, 3, null, 200000.00m, null, "C61", 0 },
                    { 262, null, 3, null, 200000.00m, null, "C62", 0 },
                    { 263, null, 3, null, 200000.00m, null, "C63", 0 },
                    { 264, null, 3, null, 200000.00m, null, "C64", 0 },
                    { 265, null, 3, null, 200000.00m, null, "C65", 0 },
                    { 266, null, 3, null, 200000.00m, null, "C66", 0 },
                    { 267, null, 3, null, 200000.00m, null, "C67", 0 },
                    { 268, null, 3, null, 200000.00m, null, "C68", 0 },
                    { 269, null, 3, null, 200000.00m, null, "C69", 0 },
                    { 270, null, 3, null, 200000.00m, null, "C70", 0 },
                    { 271, null, 3, null, 200000.00m, null, "C71", 0 },
                    { 272, null, 3, null, 200000.00m, null, "C72", 0 },
                    { 273, null, 3, null, 200000.00m, null, "C73", 0 },
                    { 274, null, 3, null, 200000.00m, null, "C74", 0 },
                    { 275, null, 3, null, 200000.00m, null, "C75", 0 },
                    { 276, null, 3, null, 200000.00m, null, "C76", 0 },
                    { 277, null, 3, null, 200000.00m, null, "C77", 0 },
                    { 278, null, 3, null, 200000.00m, null, "C78", 0 },
                    { 279, null, 3, null, 200000.00m, null, "C79", 0 },
                    { 280, null, 3, null, 200000.00m, null, "C80", 0 },
                    { 281, null, 3, null, 200000.00m, null, "C81", 0 },
                    { 282, null, 3, null, 200000.00m, null, "C82", 0 },
                    { 283, null, 3, null, 200000.00m, null, "C83", 0 },
                    { 284, null, 3, null, 200000.00m, null, "C84", 0 },
                    { 285, null, 3, null, 200000.00m, null, "C85", 0 },
                    { 286, null, 3, null, 200000.00m, null, "C86", 0 },
                    { 287, null, 3, null, 200000.00m, null, "C87", 0 },
                    { 288, null, 3, null, 200000.00m, null, "C88", 0 },
                    { 289, null, 3, null, 200000.00m, null, "C89", 0 },
                    { 290, null, 3, null, 200000.00m, null, "C90", 0 },
                    { 291, null, 3, null, 200000.00m, null, "C91", 0 },
                    { 292, null, 3, null, 200000.00m, null, "C92", 0 },
                    { 293, null, 3, null, 200000.00m, null, "C93", 0 },
                    { 294, null, 3, null, 200000.00m, null, "C94", 0 },
                    { 295, null, 3, null, 200000.00m, null, "C95", 0 },
                    { 296, null, 3, null, 200000.00m, null, "C96", 0 },
                    { 297, null, 3, null, 200000.00m, null, "C97", 0 },
                    { 298, null, 3, null, 200000.00m, null, "C98", 0 },
                    { 299, null, 3, null, 200000.00m, null, "C99", 0 },
                    { 300, null, 3, null, 200000.00m, null, "C100", 0 },
                    { 301, null, 4, null, 300000.00m, null, "D1", 0 },
                    { 302, null, 4, null, 300000.00m, null, "D2", 0 },
                    { 303, null, 4, null, 300000.00m, null, "D3", 0 },
                    { 304, null, 4, null, 300000.00m, null, "D4", 0 },
                    { 305, null, 4, null, 300000.00m, null, "D5", 0 },
                    { 306, null, 4, null, 300000.00m, null, "D6", 0 },
                    { 307, null, 4, null, 300000.00m, null, "D7", 0 },
                    { 308, null, 4, null, 300000.00m, null, "D8", 0 },
                    { 309, null, 4, null, 300000.00m, null, "D9", 0 },
                    { 310, null, 4, null, 300000.00m, null, "D10", 0 },
                    { 311, null, 4, null, 300000.00m, null, "D11", 0 },
                    { 312, null, 4, null, 300000.00m, null, "D12", 0 },
                    { 313, null, 4, null, 300000.00m, null, "D13", 0 },
                    { 314, null, 4, null, 300000.00m, null, "D14", 0 },
                    { 315, null, 4, null, 300000.00m, null, "D15", 0 },
                    { 316, null, 4, null, 300000.00m, null, "D16", 0 },
                    { 317, null, 4, null, 300000.00m, null, "D17", 0 },
                    { 318, null, 4, null, 300000.00m, null, "D18", 0 },
                    { 319, null, 4, null, 300000.00m, null, "D19", 0 },
                    { 320, null, 4, null, 300000.00m, null, "D20", 0 },
                    { 321, null, 4, null, 300000.00m, null, "D21", 0 },
                    { 322, null, 4, null, 300000.00m, null, "D22", 0 },
                    { 323, null, 4, null, 300000.00m, null, "D23", 0 },
                    { 324, null, 4, null, 300000.00m, null, "D24", 0 },
                    { 325, null, 4, null, 300000.00m, null, "D25", 0 },
                    { 326, null, 4, null, 300000.00m, null, "D26", 0 },
                    { 327, null, 4, null, 300000.00m, null, "D27", 0 },
                    { 328, null, 4, null, 300000.00m, null, "D28", 0 },
                    { 329, null, 4, null, 300000.00m, null, "D29", 0 },
                    { 330, null, 4, null, 300000.00m, null, "D30", 0 },
                    { 331, null, 4, null, 300000.00m, null, "D31", 0 },
                    { 332, null, 4, null, 300000.00m, null, "D32", 0 },
                    { 333, null, 4, null, 300000.00m, null, "D33", 0 },
                    { 334, null, 4, null, 300000.00m, null, "D34", 0 },
                    { 335, null, 4, null, 300000.00m, null, "D35", 0 },
                    { 336, null, 4, null, 300000.00m, null, "D36", 0 },
                    { 337, null, 4, null, 300000.00m, null, "D37", 0 },
                    { 338, null, 4, null, 300000.00m, null, "D38", 0 },
                    { 339, null, 4, null, 300000.00m, null, "D39", 0 },
                    { 340, null, 4, null, 300000.00m, null, "D40", 0 },
                    { 341, null, 4, null, 300000.00m, null, "D41", 0 },
                    { 342, null, 4, null, 300000.00m, null, "D42", 0 },
                    { 343, null, 4, null, 300000.00m, null, "D43", 0 },
                    { 344, null, 4, null, 300000.00m, null, "D44", 0 },
                    { 345, null, 4, null, 300000.00m, null, "D45", 0 },
                    { 346, null, 4, null, 300000.00m, null, "D46", 0 },
                    { 347, null, 4, null, 300000.00m, null, "D47", 0 },
                    { 348, null, 4, null, 300000.00m, null, "D48", 0 },
                    { 349, null, 4, null, 300000.00m, null, "D49", 0 },
                    { 350, null, 4, null, 300000.00m, null, "D50", 0 },
                    { 351, null, 4, null, 300000.00m, null, "D51", 0 },
                    { 352, null, 4, null, 300000.00m, null, "D52", 0 },
                    { 353, null, 4, null, 300000.00m, null, "D53", 0 },
                    { 354, null, 4, null, 300000.00m, null, "D54", 0 },
                    { 355, null, 4, null, 300000.00m, null, "D55", 0 },
                    { 356, null, 4, null, 300000.00m, null, "D56", 0 },
                    { 357, null, 4, null, 300000.00m, null, "D57", 0 },
                    { 358, null, 4, null, 300000.00m, null, "D58", 0 },
                    { 359, null, 4, null, 300000.00m, null, "D59", 0 },
                    { 360, null, 4, null, 300000.00m, null, "D60", 0 },
                    { 361, null, 4, null, 300000.00m, null, "D61", 0 },
                    { 362, null, 4, null, 300000.00m, null, "D62", 0 },
                    { 363, null, 4, null, 300000.00m, null, "D63", 0 },
                    { 364, null, 4, null, 300000.00m, null, "D64", 0 },
                    { 365, null, 4, null, 300000.00m, null, "D65", 0 },
                    { 366, null, 4, null, 300000.00m, null, "D66", 0 },
                    { 367, null, 4, null, 300000.00m, null, "D67", 0 },
                    { 368, null, 4, null, 300000.00m, null, "D68", 0 },
                    { 369, null, 4, null, 300000.00m, null, "D69", 0 },
                    { 370, null, 4, null, 300000.00m, null, "D70", 0 },
                    { 371, null, 4, null, 300000.00m, null, "D71", 0 },
                    { 372, null, 4, null, 300000.00m, null, "D72", 0 },
                    { 373, null, 4, null, 300000.00m, null, "D73", 0 },
                    { 374, null, 4, null, 300000.00m, null, "D74", 0 },
                    { 375, null, 4, null, 300000.00m, null, "D75", 0 },
                    { 376, null, 4, null, 300000.00m, null, "D76", 0 },
                    { 377, null, 4, null, 300000.00m, null, "D77", 0 },
                    { 378, null, 4, null, 300000.00m, null, "D78", 0 },
                    { 379, null, 4, null, 300000.00m, null, "D79", 0 },
                    { 380, null, 4, null, 300000.00m, null, "D80", 0 },
                    { 381, null, 4, null, 300000.00m, null, "D81", 0 },
                    { 382, null, 4, null, 300000.00m, null, "D82", 0 },
                    { 383, null, 4, null, 300000.00m, null, "D83", 0 },
                    { 384, null, 4, null, 300000.00m, null, "D84", 0 },
                    { 385, null, 4, null, 300000.00m, null, "D85", 0 },
                    { 386, null, 4, null, 300000.00m, null, "D86", 0 },
                    { 387, null, 4, null, 300000.00m, null, "D87", 0 },
                    { 388, null, 4, null, 300000.00m, null, "D88", 0 },
                    { 389, null, 4, null, 300000.00m, null, "D89", 0 },
                    { 390, null, 4, null, 300000.00m, null, "D90", 0 },
                    { 391, null, 4, null, 300000.00m, null, "D91", 0 },
                    { 392, null, 4, null, 300000.00m, null, "D92", 0 },
                    { 393, null, 4, null, 300000.00m, null, "D93", 0 },
                    { 394, null, 4, null, 300000.00m, null, "D94", 0 },
                    { 395, null, 4, null, 300000.00m, null, "D95", 0 },
                    { 396, null, 4, null, 300000.00m, null, "D96", 0 },
                    { 397, null, 4, null, 300000.00m, null, "D97", 0 },
                    { 398, null, 4, null, 300000.00m, null, "D98", 0 },
                    { 399, null, 4, null, 300000.00m, null, "D99", 0 },
                    { 400, null, 4, null, 300000.00m, null, "D100", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_role_id",
                table: "customers",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_venue_id",
                table: "events",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_customer_id",
                table: "tickets",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_event_id",
                table: "tickets",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_order_id",
                table: "tickets",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "venues");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
