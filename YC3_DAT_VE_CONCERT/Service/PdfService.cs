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
            var titleFont = new XFont("Arial", 28, XFontStyle.Bold);
            var headerFont = new XFont("Arial", 16, XFontStyle.Bold);
            var normalFont = new XFont("Arial", 12, XFontStyle.Regular);
            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);
            var smallFont = new XFont("Arial", 10, XFontStyle.Regular);

            // ============================================
            // COLORS
            // ============================================
            var primaryColor = XColor.FromArgb(102, 126, 234); // #667eea
            var secondaryColor = XColor.FromArgb(118, 75, 162); // #764ba2
            var textColor = XColor.FromArgb(31, 41, 55); // #1f2937
            var lightGray = XColor.FromArgb(243, 244, 246); // #f3f4f6

            // ============================================
            // HEADER - GRADIENT BACKGROUND
            // ============================================
            var headerHeight = 120;
            var headerBrush = new XSolidBrush(primaryColor);
            gfx.DrawRectangle(headerBrush, 0, 0, pageWidth, headerHeight);

            // Logo (nếu có)
            try
            {
                var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
                if (File.Exists(logoPath))
                {
                    var logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 40, 20, 80, 80);
                }
            }
            catch { /* Logo không tồn tại */ }

            // Title
            gfx.DrawString("VÉ THAM GIA CONCERT", titleFont, XBrushes.White,
                new XRect(0, 40, pageWidth, 40), XStringFormats.Center);

            // Ticket ID
            gfx.DrawString($"#{ticket.TicketId}", normalFont, XBrushes.White,
                new XRect(0, 80, pageWidth, 20), XStringFormats.Center);

            // ============================================
            // EVENT INFO SECTION
            // ============================================
            var yPosition = headerHeight + 40;

            // Event Name
            gfx.DrawString(ticket.EventName, headerFont, new XSolidBrush(textColor),
                new XRect(40, yPosition, pageWidth - 80, 30), XStringFormats.TopLeft);
            yPosition += 40;

            // Date & Time
            DrawInfoRow(gfx, "📅 Ngày & Giờ:",
                ticket.EventDate.ToString("dd/MM/yyyy - HH:mm"),
                40, yPosition, boldFont, normalFont);
            yPosition += 30;

            // Venue
            DrawInfoRow(gfx, "📍 Địa điểm:",
                ticket.VenueName,
                40, yPosition, boldFont, normalFont);
            yPosition += 25;

            gfx.DrawString(ticket.VenueAddress, smallFont, XBrushes.Gray,
                new XRect(160, yPosition, pageWidth - 200, 20), XStringFormats.TopLeft);
            yPosition += 40;

            // ============================================
            // TICKET DETAILS BOX
            // ============================================
            var boxY = yPosition;
            var boxHeight = 120;

            // Background box
            gfx.DrawRectangle(new XSolidBrush(lightGray), 40, boxY, pageWidth - 80, boxHeight);
            gfx.DrawRectangle(new XPen(primaryColor, 2), 40, boxY, pageWidth - 80, boxHeight);

            boxY += 20;

            // Seat Number - BIG
            gfx.DrawString("CHỖ NGỒI", boldFont, new XSolidBrush(textColor),
                new XRect(60, boxY, 200, 20), XStringFormats.TopLeft);

            var seatFont = new XFont("Arial", 32, XFontStyle.Bold);
            gfx.DrawString(ticket.SeatNumber, seatFont, new XSolidBrush(primaryColor),
                new XRect(60, boxY + 25, 200, 40), XStringFormats.TopLeft);

            // Ticket Type
            gfx.DrawString("LOẠI VÉ", boldFont, new XSolidBrush(textColor),
                new XRect(280, boxY, 150, 20), XStringFormats.TopLeft);

            // Price
            gfx.DrawString("GIÁ VÉ", boldFont, new XSolidBrush(textColor),
                new XRect(450, boxY, 150, 20), XStringFormats.TopLeft);

            var priceFont = new XFont("Arial", 20, XFontStyle.Bold);
            gfx.DrawString($"{ticket.Price:N0} ₫", priceFont, new XSolidBrush(XColor.FromArgb(5, 150, 105)),
                new XRect(450, boxY + 25, 150, 40), XStringFormats.TopLeft);

            yPosition = boxY + boxHeight + 40;

            // ============================================
            // CUSTOMER INFO
            // ============================================
            DrawInfoRow(gfx, "👤 Khách hàng:", ticket.CustomerName, 40, yPosition, boldFont, normalFont);
            yPosition += 25;

            DrawInfoRow(gfx, "📧 Email:", ticket.CustomerEmail, 40, yPosition, boldFont, normalFont);
            yPosition += 25;

            DrawInfoRow(gfx, "🛒 Mã đơn hàng:", ticket.OrderId.ToString(), 40, yPosition, boldFont, normalFont);
            yPosition += 25;

            if (ticket.PurchasedDate != null)
            {
                DrawInfoRow(gfx, "📆 Ngày thanh toán:",
                    ticket.PurchasedDate.ToString(),
                    40, yPosition, boldFont, normalFont);
                yPosition += 50;
            }

            // ============================================
            // QR CODE
            // ============================================
            var qrCode = _qrCodeService.GenerateQrCode(ticket.QrCodeData, 20);
            using var qrStream = new MemoryStream(qrCode);
            var qrImage = XImage.FromStream(() => qrStream);

            var qrSize = 150;
            var qrX = (pageWidth - qrSize) / 2;

            gfx.DrawImage(qrImage, qrX, yPosition, qrSize, qrSize);

            yPosition += qrSize + 20;

            gfx.DrawString("Quét mã QR để check-in tại sự kiện", smallFont, XBrushes.Gray,
                new XRect(0, yPosition, pageWidth, 20), XStringFormats.Center);

            // ============================================
            // FOOTER - TERMS
            // ============================================
            yPosition = Convert.ToInt32(pageHeight) - 80;

            var footerBrush = new XSolidBrush(lightGray);
            gfx.DrawRectangle(footerBrush, 0, yPosition, pageWidth, 80);

            yPosition += 15;

            var termsFont = new XFont("Arial", 8, XFontStyle.Regular);
            gfx.DrawString("⚠️ LƯU Ý QUAN TRỌNG:",
                new XFont("Arial", 8, XFontStyle.Bold),
                new XSolidBrush(textColor),
                new XRect(40, yPosition, pageWidth - 80, 15), XStringFormats.TopLeft);

            yPosition += 15;

            var terms = new[]
            {
            "• Vui lòng mang theo vé này (bản in hoặc điện tử) khi tham dự sự kiện",
            "• Vé không được hoàn lại hoặc đổi sau khi mua",
            "• Vui lòng đến trước 30 phút để check-in"
        };

            foreach (var term in terms)
            {
                gfx.DrawString(term, termsFont, XBrushes.DimGray,
                    new XRect(40, yPosition, pageWidth - 80, 12), XStringFormats.TopLeft);
                yPosition += 12;
            }
        }

        private void DrawInfoRow(XGraphics gfx, string label, string value,
            double x, double y, XFont labelFont, XFont valueFont)
        {
            gfx.DrawString(label, labelFont, XBrushes.Black,
                new XRect(x, y, 120, 20), XStringFormats.TopLeft);

            gfx.DrawString(value, valueFont, XBrushes.Black,
                new XRect(x + 120, y, 400, 20), XStringFormats.TopLeft);
        }
    }
}
