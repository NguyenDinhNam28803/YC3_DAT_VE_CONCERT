using System;
using Microsoft.EntityFrameworkCore;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Make DbSet properties public so EF Core can discover them at runtime.
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Venue> Venues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role <-> Customer : one Role has many Customers, delete restricted.
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Role)
                .WithMany(r => r.Customers)
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer <-> Order : one Customer has many Orders, cascade delete orders when customer removed.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer <-> Ticket : one Customer has many Tickets, cascade delete tickets when customer removed.
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event <-> Ticket : one Event has many Tickets, cascade delete tickets when event removed.
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ticket <-> Order : explicit behavior (Restrict)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Tickets)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event <-> Venue : one Venue has many Events, prevent cascading deletes of events when a venue is removed.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed roles
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                );

            modelBuilder.Entity<Venue>()
                .HasData(
                    new Venue
                    {
                        Id = 1,
                        Name = "Nhà Hát Lớn Hà Nội",
                        Location = "1 Tràng Tiền, Hoàn Kiếm, Hà Nội",
                        Capacity = 1200
                    },
                    new Venue
                    {
                        Id = 2,
                        Name = "Sân vận động Mỹ Đình",
                        Location = "Mỹ Đình, Nam Từ Liêm, Hà Nội",
                        Capacity = 40000
                    },
                    new Venue
                    {
                        Id = 3,
                        Name = "Nhà Văn Hóa Thanh Niên",
                        Location = "4 Phạm Ngọc Thạch, Quận 1, TP. Hồ Chí Minh",
                        Capacity = 2000
                    }
                );

            // --- Updated: seed events include new columns TicketPrice and TotalSeat ---
            modelBuilder.Entity<Event>()
                .HasData(
                    new Event
                    {
                        Id = 1,
                        Name = "Lễ hội Âm nhạc Xuân",
                        Date = new DateTime(2026, 05, 10, 19, 0, 0, DateTimeKind.Utc),
                        VenueId = 2,
                        Description = "Chuỗi ban nhạc và DJ biểu diễn tại Sân vận động Mỹ Đình.",
                        TicketPrice = 500000.00m,
                        TotalSeat = 35000
                    },
                    new Event
                    {
                        Id = 2,
                        Name = "Đêm Nhạc Cổ Điển",
                        Date = new DateTime(2026, 06, 12, 19, 30, 0, DateTimeKind.Utc),
                        VenueId = 1,
                        Description = "Buổi hòa nhạc cổ điển tại Nhà Hát Lớn Hà Nội.",
                        TicketPrice = 350000.00m,
                        TotalSeat = 1000
                    },
                    new Event
                    {
                        Id = 3,
                        Name = "Indie Showcase TP.HCM",
                        Date = new DateTime(2026, 07, 05, 20, 0, 0, DateTimeKind.Utc),
                        VenueId = 3,
                        Description = "Các ban nhạc indie và nghệ sĩ trẻ biểu diễn ở Nhà Văn Hóa Thanh Niên.",
                        TicketPrice = 200000.00m,
                        TotalSeat = 800
                    },
                    new Event
                    {
                        Id = 4,
                        Name = "Đêm Jazz Mùa Hè",
                        Date = new DateTime(2026, 08, 20, 20, 0, 0, DateTimeKind.Utc),
                        VenueId = 1,
                        Description = "Đêm nhạc jazz lãng mạn tại Hà Nội.",
                        TicketPrice = 300000.00m,
                        TotalSeat = 900
                    }
                );

            // (other seeds for Customer/Order/Ticket remain unchanged)
        }
    }
}