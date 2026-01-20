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
            var headerHeight = 140;

            // Gradient simulation with multiple rectangles
            for (int i = 0; i < headerHeight; i += 2)
            {
                var gradientColor = XColor.FromArgb(
                    (byte)(99 + (139 - 99) * i / headerHeight),
                    (byte)(102 + (92 - 102) * i / headerHeight),
                    (byte)(241 + (246 - 241) * i / headerHeight)
                );
                gfx.DrawRectangle(new XSolidBrush(gradientColor), 0, i, pageWidth, 2);
            }

            // Decorative circles in header
            gfx.DrawEllipse(new XSolidBrush(XColor.FromArgb(50, 255, 255, 255)),
                pageWidth - 100, -30, 120, 120);
            gfx.DrawEllipse(new XSolidBrush(XColor.FromArgb(30, 255, 255, 255)),
                -40, headerHeight - 80, 100, 100);

            // Logo placeholder (optional)
            try
            {
                var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
                if (File.Exists(logoPath))
                {
                    var logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 30, 25, 70, 70);
                }
            }
            catch { /* Logo không tồn tại */ }

            // Main Title
            gfx.DrawString("VÉ THAM GIA CONCERT", titleFont, XBrushes.White,
                new XRect(0, 45, pageWidth, 40), XStringFormats.Center);

            // Ticket ID Badge
            var badgeWidth = 180;
            var badgeX = (pageWidth - badgeWidth) / 2;
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(80, 255, 255, 255)),
                badgeX, 95, badgeWidth, 30);
            gfx.DrawRectangle(new XPen(XColor.FromArgb(150, 255, 255, 255), 1.5),
                badgeX, 95, badgeWidth, 30);
            gfx.DrawString($"#{ticket.TicketId}", subHeaderFont, XBrushes.White,
                new XRect(badgeX, 95, badgeWidth, 30), XStringFormats.Center);

            // ============================================
            // EVENT INFO CARD - Elevated Design
            // ============================================
            var cardY = headerHeight + 25;
            var cardPadding = 30;
            var cardWidth = pageWidth - 60;

            // Card shadow
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(20, 0, 0, 0)),
                32, cardY + 3, cardWidth, 100);

            // Card background
            gfx.DrawRectangle(XBrushes.White, 30, cardY, cardWidth, 100);
            gfx.DrawRectangle(new XPen(bgGray, 1), 30, cardY, cardWidth, 100);

            // Left border accent
            gfx.DrawRectangle(new XSolidBrush(primaryColor), 30, cardY, 5, 100);

            var infoY = cardY + 20;

            // Event Name - Bold & Large
            gfx.DrawString(ticket.EventName, headerFont, new XSolidBrush(textDark),
                new XRect(50, infoY, cardWidth - 40, 25), XStringFormats.TopLeft);

            infoY += 35;

            // Event details in two columns
            // Left column
            DrawModernInfoItem(gfx, "📅", "Ngày & Giờ",
                ticket.EventDate.ToString("dd/MM/yyyy • HH:mm"),
                50, infoY, boldFont, normalFont, textMedium);

            // Right column
            DrawModernInfoItem(gfx, "📍", "Địa điểm",
                ticket.VenueName,
                pageWidth / 2 + 10, infoY, boldFont, normalFont, textMedium);

            // ============================================
            // SEAT & PRICE - Big Focus Area
            // ============================================
            var seatY = cardY + 140;
            var seatBoxHeight = 130;

            // Background with subtle pattern
            gfx.DrawRectangle(new XSolidBrush(bgLight), 30, seatY, cardWidth, seatBoxHeight);

            // Top border accent
            var gradientPen = new XPen(primaryColor, 3);
            gfx.DrawLine(gradientPen, 30, seatY, 30 + cardWidth, seatY);

            var seatContentY = seatY + 25;

            // Seat Number Section - LEFT (60% width)
            var seatSectionWidth = cardWidth * 0.6;

            gfx.DrawString("CHỖ NGỒI", boldFont, new XSolidBrush(textMedium),
                new XRect(50, seatContentY, seatSectionWidth, 20), XStringFormats.TopLeft);

            var megaSeatFont = new XFont("Arial", 48, XFontStyle.Bold);
            gfx.DrawString(ticket.SeatNumber, megaSeatFont, new XSolidBrush(primaryColor),
                new XRect(50, seatContentY + 20, seatSectionWidth, 60), XStringFormats.TopLeft);

            // Decorative underline
            gfx.DrawLine(new XPen(accentColor, 3), 50, seatContentY + 85,
                50 + ticket.SeatNumber.Length * 30, seatContentY + 85);

            // Vertical divider
            var dividerX = 30 + seatSectionWidth + 20;
            gfx.DrawLine(new XPen(bgGray, 2), dividerX, seatContentY,
                dividerX, seatContentY + 90);

            // Price Section - RIGHT (40% width)
            var priceX = dividerX + 30;
            var priceSectionWidth = cardWidth * 0.35;

            gfx.DrawString("GIÁ VÉ", boldFont, new XSolidBrush(textMedium),
                new XRect(priceX, seatContentY, priceSectionWidth, 20), XStringFormats.TopLeft);

            var priceFont = new XFont("Arial", 32, XFontStyle.Bold);
            gfx.DrawString($"{ticket.Price:N0}", priceFont, new XSolidBrush(successColor),
                new XRect(priceX, seatContentY + 20, priceSectionWidth, 45), XStringFormats.TopLeft);

            gfx.DrawString("VNĐ", normalFont, new XSolidBrush(textMedium),
                new XRect(priceX, seatContentY + 65, priceSectionWidth, 20), XStringFormats.TopLeft);

            // ============================================
            // CUSTOMER INFORMATION - Clean List
            // ============================================
            var customerY = seatY + seatBoxHeight + 30;

            gfx.DrawString("THÔNG TIN KHÁCH HÀNG", subHeaderFont, new XSolidBrush(textDark),
                new XRect(30, customerY, cardWidth, 20), XStringFormats.TopLeft);

            customerY += 30;

            // Info items with icons
            DrawCleanInfoRow(gfx, "👤", "Khách hàng", ticket.CustomerName,
                50, customerY, textMedium, textDark);
            customerY += 25;

            DrawCleanInfoRow(gfx, "📧", "Email", ticket.CustomerEmail,
                50, customerY, textMedium, textDark);
            customerY += 25;

            DrawCleanInfoRow(gfx, "🎫", "Mã đơn hàng", ticket.OrderId.ToString(),
                50, customerY, textMedium, textDark);
            customerY += 25;

            if (ticket.PurchasedDate != null)
            {
                DrawCleanInfoRow(gfx, "📆", "Ngày thanh toán",
                    ticket.PurchasedDate.ToString(),
                    50, customerY, textMedium, textDark);
                customerY += 35;
            }
            else
            {
                customerY += 10;
            }

            // ============================================
            // QR CODE - NO BORDER VERSION
            // ============================================
            var qrSectionY = customerY + 10; // Margin từ customer info

            // QR Code generation with temp file
            try
            {
                var qrCode = _qrCodeService.GenerateQrCode(ticket.QrCodeData, 20);
                var tempQrPath = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                File.WriteAllBytes(tempQrPath, qrCode);

                try
                {
                    using var qrImage = XImage.FromFile(tempQrPath);

                    var qrSize = 160;
                    var qrX = (pageWidth - qrSize) / 2;
                    var qrY = qrSectionY;

                    // ✅ White background box cho QR (không có border card nữa)
                    var qrPadding = 15;
                    gfx.DrawRectangle(XBrushes.White,
                        qrX - qrPadding, qrY - qrPadding,
                        qrSize + (qrPadding * 2), qrSize + (qrPadding * 2));

                    // ✅ Subtle shadow
                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(10, 0, 0, 0)),
                        qrX - qrPadding + 3, qrY - qrPadding + 3,
                        qrSize + (qrPadding * 2), qrSize + (qrPadding * 2));

                    // ✅ Draw QR
                    gfx.DrawImage(qrImage, qrX, qrY, qrSize, qrSize);

                    // ✅ Instructions below QR
                    var instructionY = qrY + qrSize + 20;

                    gfx.DrawString("Quét mã QR để check-in", subHeaderFont, new XSolidBrush(textDark),
                        new XRect(0, instructionY, pageWidth, 20), XStringFormats.Center);

                    gfx.DrawString("Vui lòng xuất trình mã này tại cổng vào", smallFont, new XSolidBrush(textMedium),
                        new XRect(0, instructionY + 22, pageWidth, 15), XStringFormats.Center);

                    // ✅ Update position cho footer
                    qrSectionY = instructionY + 45; // Cập nhật vị trí kết thúc QR section
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
                    new XRect(0, qrSectionY + 80, pageWidth, 20), XStringFormats.Center);
                qrSectionY += 120;
                Console.WriteLine($"QR Code error: {ex.Message}");
            }

            // ============================================
            // FOOTER - DYNAMIC POSITION (bắt đầu SAU QR)
            // ============================================
            double footerY = qrSectionY + 20; // ✅ 20px margin từ QR section
            var footerHeight = 90;

            // ✅ CHECK: Đảm bảo footer không vượt quá trang
            var pageBottomMargin = 10;
            if (footerY + footerHeight > pageHeight - pageBottomMargin)
            {
                footerY = pageHeight - footerHeight - pageBottomMargin;
            }

            // Footer background
            gfx.DrawRectangle(new XSolidBrush(bgGray), 0, footerY, pageWidth, footerHeight);

            // Top border accent
            gfx.DrawLine(new XPen(primaryColor, 2), 0, footerY, pageWidth, footerY);

            var footerContentY = footerY + 15;

            // Warning icon + title
            var warningIconSize = 14;
            gfx.DrawEllipse(new XSolidBrush(accentColor), 30, footerContentY, warningIconSize, warningIconSize);
            gfx.DrawString("!", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.White,
                new XRect(30, footerContentY, warningIconSize, warningIconSize), XStringFormats.Center);

            gfx.DrawString("LƯU Ý QUAN TRỌNG", boldFont, new XSolidBrush(textDark),
                new XRect(50, footerContentY + 2, 200, 18), XStringFormats.TopLeft);

            footerContentY += 22;

            // Notes - 2 columns for compact
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

            // Company info at bottom
            var companyY = footerContentY + 28;
            gfx.DrawString("YC3 Concert Booking © 2025 | support@yc3concert.com",
                tinyFont, new XSolidBrush(textLight),
                new XRect(0, companyY, pageWidth, 10), XStringFormats.Center);
        }

        // ============================================
        // HELPER METHODS - Modern Design
        // ============================================
        private void DrawModernInfoItem(XGraphics gfx, string icon, string label, string value,
            double x, double y, XFont labelFont, XFont valueFont, XColor labelColor)
        {
            // Icon
            gfx.DrawString(icon, new XFont("Arial", 14), new XSolidBrush(labelColor),
                new XRect(x, y - 2, 20, 20), XStringFormats.TopLeft);

            // Label
            gfx.DrawString(label, labelFont, new XSolidBrush(labelColor),
                new XRect(x + 25, y, 150, 15), XStringFormats.TopLeft);

            // Value
            gfx.DrawString(value, valueFont, XBrushes.Black,
                new XRect(x + 25, y + 16, 250, 15), XStringFormats.TopLeft);
        }

        private void DrawCleanInfoRow(XGraphics gfx, string icon, string label, string value,
            double x, double y, XColor labelColor, XColor valueColor)
        {
            var iconFont = new XFont("Arial", 12);
            var labelFont = new XFont("Arial", 10, XFontStyle.Regular);
            var valueFont = new XFont("Arial", 10, XFontStyle.Bold);

            // Icon
            gfx.DrawString(icon, iconFont, new XSolidBrush(labelColor),
                new XRect(x - 25, y - 1, 20, 20), XStringFormats.TopLeft);

            // Label with colon
            gfx.DrawString($"{label}:", labelFont, new XSolidBrush(labelColor),
                new XRect(x, y, 120, 18), XStringFormats.TopLeft);

            // Value
            gfx.DrawString(value, valueFont, new XSolidBrush(valueColor),
                new XRect(x + 120, y, 350, 18), XStringFormats.TopLeft);
        }
    }
}
