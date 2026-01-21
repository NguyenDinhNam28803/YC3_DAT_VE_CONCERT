using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using System.IO;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class PdfService : IPdfService
    {
        private readonly IQrCodeService _qrCodeService;
        private readonly IWebHostEnvironment _environment;
        public PdfService(IQrCodeService qrCodeService, IWebHostEnvironment environment)
        {
            _qrCodeService = qrCodeService;
            _environment = environment;
        }

        public byte[] GenerateTicketPdf(TicketDto ticket)
        {
            using var document = new PdfDocument();
            document.Info.Title = $"Ticket_{ticket.TicketId}";

            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;

            using var gfx = XGraphics.FromPdfPage(page);

            DrawTicketContent(gfx, ticket, page);

            using var ms = new MemoryStream();
            document.Save(ms, false);
            return ms.ToArray();
        }

        public byte[] GenerateBulkTicketsPdf(List<TicketDto> tickets)
        {
            using var document = new PdfDocument();
            document.Info.Title = $"Tickets_Order_{tickets.First().OrderId}";

            foreach (var ticket in tickets)
            {
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                using var gfx = XGraphics.FromPdfPage(page);
                DrawTicketContent(gfx, ticket, page);
            }

            using var ms = new MemoryStream();
            document.Save(ms, false);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateTicketPdfAsync(TicketDto ticket)
        {
            return await Task.Run(() => GenerateTicketPdf(ticket));
        }

        private void DrawTicketContent(XGraphics gfx, TicketDto ticket, PdfPage page)
        {
            var pageWidth = page.Width;
            var pageHeight = page.Height;

            // ============================================
            // FONTS
            // ============================================
            var titleFont = new XFont("Arial", 32, XFontStyle.Bold);
            var headerFont = new XFont("Arial", 18, XFontStyle.Bold);
            var subHeaderFont = new XFont("Arial", 14, XFontStyle.Bold);
            var normalFont = new XFont("Arial", 12, XFontStyle.Regular);
            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);
            var smallFont = new XFont("Arial", 10, XFontStyle.Regular);
            var tinyFont = new XFont("Arial", 8, XFontStyle.Regular);

            // ============================================
            // COLORS - Modern Palette
            // ============================================
            var primaryColor = XColor.FromArgb(99, 102, 241);      // Indigo #6366f1
            var secondaryColor = XColor.FromArgb(139, 92, 246);    // Purple #8b5cf6
            var accentColor = XColor.FromArgb(236, 72, 153);       // Pink #ec4899
            var successColor = XColor.FromArgb(16, 185, 129);      // Green #10b981
            var textDark = XColor.FromArgb(17, 24, 39);            // Gray-900 #111827
            var textMedium = XColor.FromArgb(75, 85, 99);          // Gray-600 #4b5563
            var textLight = XColor.FromArgb(156, 163, 175);        // Gray-400 #9ca3af
            var bgLight = XColor.FromArgb(249, 250, 251);          // Gray-50 #f9fafb
            var bgGray = XColor.FromArgb(243, 244, 246);           // Gray-100 #f3f4f6

            // ============================================
            // MODERN HEADER with Gradient Effect
            // ============================================
            var headerHeight = 120;  // ✅ Giảm từ 140 xuống 120

            // Gradient simulation
            for (int i = 0; i < headerHeight; i += 2)
            {
                var gradientColor = XColor.FromArgb(
                    (byte)(99 + (139 - 99) * i / headerHeight),
                    (byte)(102 + (92 - 102) * i / headerHeight),
                    (byte)(241 + (246 - 241) * i / headerHeight)
                );
                gfx.DrawRectangle(new XSolidBrush(gradientColor), 0, i, pageWidth, 2);
            }

            // Decorative circles
            gfx.DrawEllipse(new XSolidBrush(XColor.FromArgb(50, 255, 255, 255)),
                pageWidth - 100, -30, 120, 120);
            gfx.DrawEllipse(new XSolidBrush(XColor.FromArgb(30, 255, 255, 255)),
                -40, headerHeight - 80, 100, 100);

            // Logo
            try
            {
                var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
                if (File.Exists(logoPath))
                {
                    var logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 30, 20, 60, 60);  // ✅ Giảm size
                }
            }
            catch { }

            // Main Title
            gfx.DrawString("VÉ THAM GIA CONCERT", titleFont, XBrushes.White,
                new XRect(0, 35, pageWidth, 40), XStringFormats.Center);

            // Ticket ID Badge
            var badgeWidth = 180;
            var badgeX = (pageWidth - badgeWidth) / 2;
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(80, 255, 255, 255)),
                badgeX, 85, badgeWidth, 28);  // ✅ Giảm height
            gfx.DrawRectangle(new XPen(XColor.FromArgb(150, 255, 255, 255), 1.5),
                badgeX, 85, badgeWidth, 28);
            gfx.DrawString($"#{ticket.TicketId}", subHeaderFont, XBrushes.White,
                new XRect(badgeX, 85, badgeWidth, 28), XStringFormats.Center);

            // ============================================
            // EVENT INFO CARD - Compact
            // ============================================
            var cardY = headerHeight + 20;  // ✅ Giảm margin
            var cardPadding = 30;
            var cardWidth = pageWidth - 60;

            // Card shadow
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(20, 0, 0, 0)),
                32, cardY + 3, cardWidth, 90);  // ✅ Giảm từ 100 xuống 90

            // Card background
            gfx.DrawRectangle(XBrushes.White, 30, cardY, cardWidth, 90);
            gfx.DrawRectangle(new XPen(bgGray, 1), 30, cardY, cardWidth, 90);
            gfx.DrawRectangle(new XSolidBrush(primaryColor), 30, cardY, 5, 90);

            var infoY = cardY + 15;  // ✅ Giảm padding

            // Event Name
            gfx.DrawString(ticket.EventName, headerFont, new XSolidBrush(textDark),
                new XRect(50, infoY, cardWidth - 40, 25), XStringFormats.TopLeft);

            infoY += 32;  // ✅ Giảm spacing

            // Event details
            DrawModernInfoItem(gfx, "📅", "Ngày & Giờ",
                ticket.EventDate.ToString("dd/MM/yyyy • HH:mm"),
                50, infoY, boldFont, normalFont, textMedium);

            DrawModernInfoItem(gfx, "📍", "Địa điểm",
                ticket.VenueName,
                pageWidth / 2 + 10, infoY, boldFont, normalFont, textMedium);

            // ============================================
            // SEAT & PRICE - Compact
            // ============================================
            var seatY = cardY + 110;  // ✅ Điều chỉnh
            var seatBoxHeight = 110;  // ✅ Giảm từ 130 xuống 110

            gfx.DrawRectangle(new XSolidBrush(bgLight), 30, seatY, cardWidth, seatBoxHeight);
            gfx.DrawLine(new XPen(primaryColor, 3), 30, seatY, 30 + cardWidth, seatY);

            var seatContentY = seatY + 20;  // ✅ Giảm padding
            var seatSectionWidth = cardWidth * 0.6;

            gfx.DrawString("CHỖ NGỒI", boldFont, new XSolidBrush(textMedium),
                new XRect(50, seatContentY, seatSectionWidth, 20), XStringFormats.TopLeft);

            var megaSeatFont = new XFont("Arial", 42, XFontStyle.Bold);  // ✅ Giảm từ 48
            gfx.DrawString(ticket.SeatNumber, megaSeatFont, new XSolidBrush(primaryColor),
                new XRect(50, seatContentY + 18, seatSectionWidth, 55), XStringFormats.TopLeft);

            gfx.DrawLine(new XPen(accentColor, 3), 50, seatContentY + 78,
                50 + ticket.SeatNumber.Length * 28, seatContentY + 78);

            // Divider
            var dividerX = 30 + seatSectionWidth + 20;
            gfx.DrawLine(new XPen(bgGray, 2), dividerX, seatContentY,
                dividerX, seatContentY + 80);

            // Price Section
            var priceX = dividerX + 30;
            var priceSectionWidth = cardWidth * 0.35;

            gfx.DrawString("GIÁ VÉ", boldFont, new XSolidBrush(textMedium),
                new XRect(priceX, seatContentY, priceSectionWidth, 20), XStringFormats.TopLeft);

            var priceFont = new XFont("Arial", 28, XFontStyle.Bold);  // ✅ Giảm từ 32
            gfx.DrawString($"{ticket.Price:N0}", priceFont, new XSolidBrush(successColor),
                new XRect(priceX, seatContentY + 18, priceSectionWidth, 40), XStringFormats.TopLeft);

            gfx.DrawString("VNĐ", normalFont, new XSolidBrush(textMedium),
                new XRect(priceX, seatContentY + 58, priceSectionWidth, 20), XStringFormats.TopLeft);

            // ============================================
            // CUSTOMER INFORMATION - Compact
            // ============================================
            var customerY = seatY + seatBoxHeight + 20;  // ✅ Giảm margin

            gfx.DrawString("THÔNG TIN KHÁCH HÀNG", subHeaderFont, new XSolidBrush(textDark),
                new XRect(30, customerY, cardWidth, 20), XStringFormats.TopLeft);

            customerY += 25;  // ✅ Giảm spacing

            DrawCleanInfoRow(gfx, "👤", "Khách hàng", ticket.CustomerName,
                50, customerY, textMedium, textDark);
            customerY += 22;  // ✅ Giảm từ 25

            DrawCleanInfoRow(gfx, "📧", "Email", ticket.CustomerEmail,
                50, customerY, textMedium, textDark);
            customerY += 22;

            DrawCleanInfoRow(gfx, "🎫", "Mã đơn hàng", ticket.OrderId.ToString(),
                50, customerY, textMedium, textDark);
            customerY += 22;

            if (ticket.PurchasedDate != null)
            {
                DrawCleanInfoRow(gfx, "📆", "Ngày thanh toán",
                    ticket.PurchasedDate.ToString(),
                    50, customerY, textMedium, textDark);
                customerY += 30;  // ✅ Giảm từ 35
            }
            else
            {
                customerY += 10;
            }

            // ============================================
            // QR CODE - FULL DISPLAY với đủ khoảng trống
            // ============================================
            var qrSectionY = customerY + 15;  // ✅ Margin tối ưu

            try
            {
                var qrCode = _qrCodeService.GenerateQrCode(ticket.QrCodeData, 20);
                var tempQrPath = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                File.WriteAllBytes(tempQrPath, qrCode);

                try
                {
                    using var qrImage = XImage.FromFile(tempQrPath);

                    var qrSize = 150;  // ✅ Giảm từ 160 để tiết kiệm không gian
                    var qrX = (pageWidth - qrSize) / 2;
                    var qrY = qrSectionY;

                    // White background with shadow
                    var qrPadding = 12;  // ✅ Giảm padding

                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(10, 0, 0, 0)),
                        qrX - qrPadding + 3, qrY - qrPadding + 3,
                        qrSize + (qrPadding * 2), qrSize + (qrPadding * 2));

                    gfx.DrawRectangle(XBrushes.White,
                        qrX - qrPadding, qrY - qrPadding,
                        qrSize + (qrPadding * 2), qrSize + (qrPadding * 2));

                    // Draw QR
                    gfx.DrawImage(qrImage, qrX, qrY, qrSize, qrSize);

                    // Instructions
                    var instructionY = qrY + qrSize + 15;  // ✅ Giảm spacing

                    gfx.DrawString("Quét mã QR để check-in", subHeaderFont, new XSolidBrush(textDark),
                        new XRect(0, instructionY, pageWidth, 20), XStringFormats.Center);

                    gfx.DrawString("Vui lòng xuất trình mã này tại cổng vào", smallFont, new XSolidBrush(textMedium),
                        new XRect(0, instructionY + 20, pageWidth, 15), XStringFormats.Center);

                    // ✅ Cập nhật vị trí kết thúc QR section
                    qrSectionY = instructionY + 40;
                }
                finally
                {
                    if (File.Exists(tempQrPath))
                        File.Delete(tempQrPath);
                }
            }
            catch (Exception ex)
            {
                gfx.DrawString($"QR Code: {ticket.TicketId}", normalFont, new XSolidBrush(textMedium),
                    new XRect(0, qrSectionY + 75, pageWidth, 20), XStringFormats.Center);
                qrSectionY += 110;
                Console.WriteLine($"QR Code error: {ex.Message}");
            }

            // ============================================
            // FOOTER - LUÔN Ở CUỐI TRANG
            // ============================================
            var footerHeight = 85;  // ✅ Giảm từ 90
            var footerY = pageHeight - footerHeight;  // ✅ LUÔN neo ở cuối trang

            // Footer background
            gfx.DrawRectangle(new XSolidBrush(bgGray), 0, footerY, pageWidth, footerHeight);
            gfx.DrawLine(new XPen(primaryColor, 2), 0, footerY, pageWidth, footerY);

            var footerContentY = footerY + 12;  // ✅ Giảm padding

            // Warning icon + title
            var warningIconSize = 13;  // ✅ Giảm size
            gfx.DrawEllipse(new XSolidBrush(accentColor), 30, footerContentY, warningIconSize, warningIconSize);
            gfx.DrawString("!", new XFont("Arial", 9, XFontStyle.Bold), XBrushes.White,
                new XRect(30, footerContentY, warningIconSize, warningIconSize), XStringFormats.Center);

            gfx.DrawString("LƯU Ý QUAN TRỌNG", boldFont, new XSolidBrush(textDark),
                new XRect(48, footerContentY + 1, 200, 18), XStringFormats.TopLeft);

            footerContentY += 20;  // ✅ Giảm spacing

            // Notes - 2 columns
            var col1X = 35;
            var col2X = pageWidth / 2 + 5;
            var noteFont = new XFont("Arial", 8, XFontStyle.Regular);

            var notesCol1 = new[]
            {
                "• Mang theo vé (bản in/điện tử)",
                "• Vui lòng đến trước 30 phút"
            };

            var notesCol2 = new[]
            {
                "• Không hoàn lại sau khi mua",
                "• Hotline: 1900-xxxx"
            };

            var noteY1 = footerContentY;
            foreach (var note in notesCol1)
            {
                gfx.DrawString(note, noteFont, new XSolidBrush(textMedium),
                    new XRect(col1X, noteY1, 250, 11), XStringFormats.TopLeft);
                noteY1 += 11;
            }

            var noteY2 = footerContentY;
            foreach (var note in notesCol2)
            {
                gfx.DrawString(note, noteFont, new XSolidBrush(textMedium),
                    new XRect(col2X, noteY2, 250, 11), XStringFormats.TopLeft);
                noteY2 += 11;
            }

            // Company info
            var companyY = footerContentY + 26;  // ✅ Điều chỉnh
            gfx.DrawString("YC3 Concert Booking © 2025 | support@yc3concert.com",
                tinyFont, new XSolidBrush(textLight),
                new XRect(0, companyY, pageWidth, 10), XStringFormats.Center);
        }

        // ============================================
        // HELPER METHODS
        // ============================================
        private void DrawModernInfoItem(XGraphics gfx, string icon, string label, string value,
            double x, double y, XFont labelFont, XFont valueFont, XColor labelColor)
        {
            gfx.DrawString(icon, new XFont("Arial", 14), new XSolidBrush(labelColor),
                new XRect(x, y - 2, 20, 20), XStringFormats.TopLeft);

            gfx.DrawString(label, labelFont, new XSolidBrush(labelColor),
                new XRect(x + 25, y, 150, 15), XStringFormats.TopLeft);

            gfx.DrawString(value, valueFont, XBrushes.Black,
                new XRect(x + 25, y + 16, 250, 15), XStringFormats.TopLeft);
        }

        private void DrawCleanInfoRow(XGraphics gfx, string icon, string label, string value,
            double x, double y, XColor labelColor, XColor valueColor)
        {
            var iconFont = new XFont("Arial", 12);
            var labelFont = new XFont("Arial", 10, XFontStyle.Regular);
            var valueFont = new XFont("Arial", 10, XFontStyle.Bold);

            gfx.DrawString(icon, iconFont, new XSolidBrush(labelColor),
                new XRect(x - 25, y - 1, 20, 20), XStringFormats.TopLeft);

            gfx.DrawString($"{label}:", labelFont, new XSolidBrush(labelColor),
                new XRect(x, y, 120, 18), XStringFormats.TopLeft);

            gfx.DrawString(value, valueFont, new XSolidBrush(valueColor),
                new XRect(x + 120, y, 350, 18), XStringFormats.TopLeft);
        }
    }
}