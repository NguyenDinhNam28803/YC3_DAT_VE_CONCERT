using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;
using static System.Net.Mime.MediaTypeNames;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSettings;
        public EmailService(IOptions<EmailSetting> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmail(string toName, string toEmail, string subject, string body)
    => SendEmail(toName, toEmail, subject, body, isHtml: false);

        public async Task SendEmail(string toName, string toEmail, string subject, string body, bool isHtml)
        {
            // Implement email sending logic here
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();

            if (isHtml)
            {
                // Nếu body đã là HTML (có chứa thẻ HTML), dùng trực tiếp
                bodyBuilder.HtmlBody = body ?? string.Empty;
                bodyBuilder.TextBody = StripHtmlForPlainText(body);
            }
            else
            {
                // Kiểm tra xem body có phải là HTML không (chứa thẻ HTML)
                bool bodyContainsHtml = !string.IsNullOrEmpty(body) &&
                                        (body.Contains("<p>") || body.Contains("<table>") ||
                                         body.Contains("<ul>") || body.Contains("<strong>") ||
                                         body.Contains("<br"));

                if (bodyContainsHtml)
                {
                    // Nếu body đã chứa HTML, wrap vào template và dùng luôn
                    var html = WrapHtmlTemplate(
                        _emailSettings.SenderName ?? "YC3 DAT VE CONCERT",
                        _emailSettings.SenderEmail ?? string.Empty,
                        toName,
                        body,
                        isContentHtml: true  // Đánh dấu content đã là HTML
                    );
                    bodyBuilder.HtmlBody = html;
                    bodyBuilder.TextBody = StripHtmlForPlainText(body);
                }
                else
                {
                    // Nếu là plain text, encode và format
                    string safeBody = WebUtility.HtmlEncode(body ?? string.Empty)
                        .Replace("\r\n", "\n")
                        .Replace("\n\n", "</p><p>")
                        .Replace("\n", "<br />");

                    var html = WrapHtmlTemplate(
                        _emailSettings.SenderName ?? "YC3 DAT VE CONCERT",
                        _emailSettings.SenderEmail ?? string.Empty,
                        toName,
                        safeBody,
                        isContentHtml: false
                    );
                    bodyBuilder.HtmlBody = html;
                    bodyBuilder.TextBody = (body ?? string.Empty).Replace("\r\n", "\n");
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _emailSettings.Username,
                    _emailSettings.Password
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        // Nicely formatted order confirmation email
        public Task SendOrderConfirmationEmail(string toName, string toEmail, string orderId, string concertName, DateTime concertDate, string seatInfo, decimal totalAmount, string? paymentLink = null)
        {
            try
            {
                var subject = $"Xác nhận đơn hàng #{WebUtility.HtmlEncode(orderId)} - {WebUtility.HtmlEncode(concertName)}";

                // Build seat list with modern card design
                var seatsSb = new StringBuilder();
                seatsSb.AppendLine("<div style=\"background:#f8f9fa;border-radius:12px;padding:20px;margin:20px 0;\">");
                seatsSb.AppendLine("<h3 style=\"margin:0 0 16px 0;font-size:16px;color:#1a1a1a;font-weight:600;\">🎫 Thông tin chỗ ngồi</h3>");
                seatsSb.AppendLine("<div style=\"background:white;border-radius:8px;padding:16px;border-left:4px solid #6366f1;\">");
                seatsSb.AppendLine($"<p style=\"margin:0;color:#374151;font-size:15px;line-height:1.6;\">{WebUtility.HtmlEncode(seatInfo)}</p>");
                seatsSb.AppendLine("</div>");
                seatsSb.AppendLine("</div>");

                // Modern Payment Button
                var ctaHtml = string.Empty;
                if (!string.IsNullOrWhiteSpace(paymentLink))
                {
                    var safeLink = WebUtility.HtmlEncode(paymentLink);
                    ctaHtml = $@"
                <div style=""text-align:center;margin:32px 0;"">
                    <a href=""{safeLink}"" 
                       style=""display:inline-block;
                              padding:16px 48px;
                              background:linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                              color:#ffffff;
                              text-decoration:none;
                              border-radius:12px;
                              font-weight:600;
                              font-size:16px;
                              box-shadow:0 4px 15px rgba(102, 126, 234, 0.4);
                              transition:all 0.3s ease;"">
                        💳 Thanh toán ngay
                    </a>
                    <p style=""margin-top:12px;color:#6b7280;font-size:13px;"">
                        Hoặc copy link: <a href=""{safeLink}"" style=""color:#667eea;text-decoration:none;"">{safeLink}</a>
                    </p>
                </div>";
                }

                var contentSb = new StringBuilder();

                // Header greeting
                contentSb.AppendLine($"<p style=\"font-size:16px;color:#1f2937;margin-bottom:8px;\">Xin chào <strong style=\"color:#111827;\">{WebUtility.HtmlEncode(toName ?? "Khách hàng")}</strong>,</p>");
                contentSb.AppendLine("<p style=\"color:#6b7280;font-size:15px;line-height:1.6;margin-bottom:24px;\">Cảm ơn bạn đã đặt vé tại <strong style=\"color:#667eea;\">YC3 DAT VE CONCERT</strong>. Đơn hàng của bạn đã được xác nhận thành công! 🎉</p>");

                // Order details card
                contentSb.AppendLine("<div style=\"background:#ffffff;border:1px solid #e5e7eb;border-radius:12px;padding:24px;margin:20px 0;box-shadow:0 1px 3px rgba(0,0,0,0.1);\">");
                contentSb.AppendLine("<h3 style=\"margin:0 0 20px 0;font-size:18px;color:#111827;font-weight:600;border-bottom:2px solid #f3f4f6;padding-bottom:12px;\">📋 Chi tiết đơn hàng</h3>");

                // Order info grid
                contentSb.AppendLine("<table style=\"width:100%;border-collapse:collapse;\">");

                contentSb.AppendLine("<tr style=\"border-bottom:1px solid #f3f4f6;\">");
                contentSb.AppendLine("<td style=\"padding:12px 0;font-weight:600;color:#6b7280;font-size:14px;width:140px;\">Mã đơn hàng</td>");
                contentSb.AppendLine($"<td style=\"padding:12px 0;color:#111827;font-size:14px;\"><span style=\"background:#f3f4f6;padding:4px 12px;border-radius:6px;font-family:monospace;\">{WebUtility.HtmlEncode(orderId)}</span></td>");
                contentSb.AppendLine("</tr>");

                contentSb.AppendLine("<tr style=\"border-bottom:1px solid #f3f4f6;\">");
                contentSb.AppendLine("<td style=\"padding:12px 0;font-weight:600;color:#6b7280;font-size:14px;\">🎤 Sự kiện</td>");
                contentSb.AppendLine($"<td style=\"padding:12px 0;color:#111827;font-size:14px;font-weight:500;\">{WebUtility.HtmlEncode(concertName)}</td>");
                contentSb.AppendLine("</tr>");

                contentSb.AppendLine("<tr style=\"border-bottom:1px solid #f3f4f6;\">");
                contentSb.AppendLine("<td style=\"padding:12px 0;font-weight:600;color:#6b7280;font-size:14px;\">📅 Ngày diễn</td>");
                contentSb.AppendLine($"<td style=\"padding:12px 0;color:#111827;font-size:14px;\">{concertDate:dd/MM/yyyy} lúc {concertDate:HH:mm}</td>");
                contentSb.AppendLine("</tr>");

                contentSb.AppendLine("<tr>");
                contentSb.AppendLine("<td style=\"padding:12px 0;font-weight:600;color:#6b7280;font-size:14px;\">💰 Tổng thanh toán</td>");
                contentSb.AppendLine($"<td style=\"padding:12px 0;\"><span style=\"color:#059669;font-size:20px;font-weight:700;\">{totalAmount:N0} ₫</span></td>");
                contentSb.AppendLine("</tr>");

                contentSb.AppendLine("</table>");
                contentSb.AppendLine("</div>");

                // Seats info
                contentSb.AppendLine(seatsSb.ToString());

                // Payment CTA
                contentSb.AppendLine(ctaHtml);

                // Important notice
                contentSb.AppendLine("<div style=\"background:#fef3c7;border-left:4px solid #f59e0b;border-radius:8px;padding:16px;margin:24px 0;\">");
                contentSb.AppendLine("<p style=\"margin:0;color:#92400e;font-size:14px;line-height:1.6;\">⚠️ <strong>Lưu ý quan trọng:</strong> Vui lòng hoàn tất thanh toán trước <strong>24 giờ</strong> để giữ chỗ ngồi của bạn.</p>");
                contentSb.AppendLine("</div>");

                // Footer
                contentSb.AppendLine("<div style=\"margin-top:32px;padding-top:24px;border-top:1px solid #e5e7eb;\">");
                contentSb.AppendLine("<p style=\"color:#6b7280;font-size:14px;line-height:1.6;margin-bottom:8px;\">Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi.</p>");
                contentSb.AppendLine("<p style=\"color:#9ca3af;font-size:13px;margin:0;\">Trân trọng,<br/><strong style=\"color:#667eea;\">Đội ngũ YC3 DAT VE CONCERT</strong></p>");
                contentSb.AppendLine("</div>");

                var fullHtml = WrapHtmlTemplate(
                    _emailSettings.SenderName ?? "YC3 DAT VE CONCERT",
                    _emailSettings.SenderEmail ?? string.Empty,
                    toName,
                    contentSb.ToString(),
                    isContentHtml: true
                );

                return SendEmail(toName, toEmail, subject, fullHtml, isHtml: true);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send order confirmation email.", ex);
            }
        }

        // Helper: wrap content into full HTML template
        // Thay đổi phần WrapHtmlTemplate để có giao diện đẹp hơn, hiện đại hơn
        private string WrapHtmlTemplate(string senderName, string senderEmail, string toName, string contentFragment, bool isContentHtml = true)
        {
            var header = WebUtility.HtmlEncode(senderName ?? "YC3 DAT VE CONCERT");
            var contact = WebUtility.HtmlEncode(senderEmail ?? string.Empty);
            var recipientName = WebUtility.HtmlEncode(toName ?? "Khách hàng");

            var html = $@"<!doctype html>
                <html lang=""vi"">
                <head>
                  <meta charset=""utf-8"" />
                  <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
                  <style>
                    body {{ 
                      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; 
                      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                      margin: 0; 
                      padding: 20px 0; 
                      color: #1f2937;
                    }}
                    .container {{ 
                      max-width: 600px; 
                      margin: 0 auto; 
                      background: #ffffff; 
                      border-radius: 16px; 
                      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
                      overflow: hidden; 
                    }}
                    .header {{ 
                      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                      color: #ffffff; 
                      padding: 32px 28px;
                      text-align: center;
                    }}
                    .header h1 {{ 
                      margin: 0 0 8px 0; 
                      font-size: 26px; 
                      font-weight: 700;
                      letter-spacing: -0.5px;
                    }}
                    .header p {{
                      margin: 0;
                      font-size: 14px;
                      opacity: 0.9;
                    }}
                    .greeting {{
                      background: #f9fafb;
                      padding: 20px 28px;
                      border-bottom: 1px solid #e5e7eb;
                    }}
                    .greeting h2 {{
                      margin: 0 0 4px 0;
                      font-size: 18px;
                      color: #111827;
                      font-weight: 600;
                    }}
                    .greeting p {{
                      margin: 0;
                      font-size: 14px;
                      color: #6b7280;
                    }}
                    .content {{ 
                      padding: 28px; 
                      line-height: 1.7; 
                      font-size: 15px;
                      color: #374151;
                    }}
                    .content p {{
                      margin: 0 0 16px 0;
                    }}
                    .content p:last-child {{
                      margin-bottom: 0;
                    }}
                    .footer {{ 
                      background: #f9fafb; 
                      color: #6b7280; 
                      padding: 24px 28px; 
                      font-size: 13px;
                      border-top: 1px solid #e5e7eb;
                      text-align: center;
                    }}
                    .footer-divider {{
                      margin: 12px 0;
                      height: 1px;
                      background: #e5e7eb;
                    }}
                    .contact-info {{
                      color: #9ca3af;
                      font-size: 12px;
                      margin-top: 8px;
                    }}
                    a {{ 
                      color: #667eea; 
                      text-decoration: none;
                    }}
                    a:hover {{
                      text-decoration: underline;
                    }}
                    @media (max-width: 640px) {{ 
                      body {{ padding: 12px 0; }}
                      .container {{ 
                        margin: 0 12px;
                        border-radius: 12px;
                      }}
                      .header {{ padding: 24px 20px; }}
                      .header h1 {{ font-size: 22px; }}
                      .greeting {{ padding: 16px 20px; }}
                      .content {{ padding: 20px; }}
                      .footer {{ padding: 20px; }}
                    }}
                  </style>
                </head>
                <body>
                  <div class=""container"">
                    <div class=""header"">
                      <h1>🎵 {header}</h1>
                      <p>Hệ thống đặt vé concert trực tuyến</p>
                    </div>
                    <div class=""greeting"">
                      <h2>Xin chào, {recipientName}!</h2>
                      <p>Chúng tôi rất vui được phục vụ bạn</p>
                    </div>
                    <div class=""content"">
                      {contentFragment}
                    </div>
                    <div class=""footer"">
                      <div style=""font-weight: 600; color: #374151; margin-bottom: 8px;"">
                        © {DateTime.UtcNow.Year} {header}
                      </div>
                      <div>Bản quyền thuộc về YC3 DAT VE CONCERT</div>
                      <div class=""footer-divider""></div>
                      <div class=""contact-info"">
                        📧 Liên hệ: <a href=""mailto:{contact}"">{contact}</a>
                      </div>
                    </div>
                  </div>
                </body>
                </html>";
            return html;
        }

        // Helper: crude HTML to plain-text conversion for fallback
        private static string StripHtmlForPlainText(string? html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var sb = new StringBuilder();
            bool inTag = false;
            foreach (var ch in html)
            {
                if (ch == '<') inTag = true;
                else if (ch == '>') inTag = false;
                else if (!inTag) sb.Append(ch);
            }
            return WebUtility.HtmlDecode(sb.ToString()).Replace("\r\n", "\n");
        }

        public async Task SendTicketEmailAsync(
            string toName,
            string toEmail,
            string eventName,
            DateTime eventDate,
            string seatInfo,
            List<EmailAttachment> attachments)
                {
                    var subject = $"🎫 Vé của bạn - {eventName}";

                    var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #667eea;'>Chúc mừng! Vé của bạn đã sẵn sàng 🎉</h2>
                    <p>Xin chào <strong>{toName}</strong>,</p>
                    <p>Cảm ơn bạn đã đặt vé tại YC3 Concert Booking!</p>
            
                    <div style='background: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <h3 style='margin-top: 0;'>📋 Thông tin sự kiện</h3>
                        <p><strong>Sự kiện:</strong> {eventName}</p>
                        <p><strong>Ngày giờ:</strong> {eventDate:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Chỗ ngồi:</strong> {seatInfo}</p>
                    </div>
            
                    <p><strong>⚠️ Lưu ý quan trọng:</strong></p>
                    <ul>
                        <li>Vui lòng mang theo vé này (in ra hoặc hiển thị trên điện thoại)</li>
                        <li>Đến trước 30 phút để check-in</li>
                        <li>Vé đính kèm ở dưới dạng file PDF</li>
                    </ul>
            
                    <p style='margin-top: 30px;'>Chúc bạn có trải nghiệm tuyệt vời! 🎤</p>
                    <p style='color: #666;'>Trân trọng,<br/>Đội ngũ YC3 Concert Booking</p>
                </div>
            ";

            await SendEmailWithAttachmentsAsync(toEmail, subject, body, attachments);
        }

        public async Task SendEmailWithAttachmentsAsync(
        string toEmail,
        string subject,
        string body,
        List<EmailAttachment> attachments)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _emailSettings.SenderName ?? "YC3 Concert Booking",
                    _emailSettings.SenderEmail ?? "noreply@yc3.com"));

                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                // ✅ Thêm attachments
                if (attachments != null && attachments.Any())
                {
                    foreach (var attachment in attachments)
                    {
                        bodyBuilder.Attachments.Add(
                            attachment.FileName,
                            attachment.Content,
                            ContentType.Parse(attachment.ContentType));
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        _emailSettings.SmtpServer,
                        _emailSettings.Port,
                        SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _emailSettings.Username,
                        _emailSettings.Password
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email with attachments: {ex.Message}", ex);
            }
        }
    }
}
