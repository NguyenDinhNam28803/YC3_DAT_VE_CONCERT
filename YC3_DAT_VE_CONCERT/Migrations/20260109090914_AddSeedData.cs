using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YC3_DAT_VE_CONCERT.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "id", "date", "description", "name", "venue_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 10, 19, 0, 0, 0, DateTimeKind.Utc), "Chuỗi ban nhạc và DJ biểu diễn tại Sân vận động Mỹ Đình.", "Lễ hội Âm nhạc Xuân", 2 },
                    { 2, new DateTime(2026, 6, 12, 19, 30, 0, 0, DateTimeKind.Utc), "Buổi hòa nhạc cổ điển quy mô nhỏ tại Nhà Hát Lớn Hà Nội.", "Đêm Nhạc Cổ Điển", 1 },
                    { 3, new DateTime(2026, 7, 5, 20, 0, 0, 0, DateTimeKind.Utc), "Các ban nhạc indie và nghệ sĩ trẻ biểu diễn ở Nhà Văn Hóa Thanh Niên.", "Indie Showcase TP.HCM", 3 },
                    { 4, new DateTime(2026, 8, 20, 20, 0, 0, 0, DateTimeKind.Utc), "Đêm nhạc jazz lãng mạn tại Nhà Hát Lớn Hà Nội.", "Đêm Jazz Mùa Hè", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "venues",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "venues",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "venues",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
