using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                bodyBuilder.HtmlBody = body ?? string.Empty;
                bodyBuilder.TextBody = StripHtmlForPlainText(body);
            }
            else
            {
                // Build HTML template with safe-encoded body and plain-text fallback
                string safeBody = WebUtility.HtmlEncode(body ?? string.Empty)
                    .Replace("\r\n", "\n")
                    .Replace("\n\n", "</p><p>")
                    .Replace("\n", "<br />");

                var html = WrapHtmlTemplate(_emailSettings.SenderName ?? "YC3 DAT VE CONCERT", _emailSettings.SenderEmail ?? string.Empty, toName, safeBody);
                bodyBuilder.HtmlBody = html;
                bodyBuilder.TextBody = (body ?? string.Empty).Replace("\r\n", "\n");
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

                // Build seat list table rows
                var seatsSb = new StringBuilder();
                seatsSb.AppendLine("<table style=\"width:100%; border-collapse:collapse;\">");
                seatsSb.AppendLine("<thead><tr><th style=\"text-align:left; padding:8px; border-bottom:1px solid #eee\">Chỗ ngồi</th></tr></thead>");
                seatsSb.AppendLine("<tbody>");
                seatsSb.AppendLine($"<tr><td style=\"padding:8px 0; border-bottom:1px dashed #f0f0f0\">{WebUtility.HtmlEncode(seatInfo)}</td></tr>");
                seatsSb.AppendLine("</tbody>");
                seatsSb.AppendLine("</table>");
                 
                // Payment CTA
                var ctaHtml = string.Empty;
                if (!string.IsNullOrWhiteSpace(paymentLink))
                {
                    var safeLink = WebUtility.HtmlEncode(paymentLink);
                    ctaHtml = $@"<p style=""text-align:center;margin:18px 0;"">
                                    <a href=""{safeLink}"" style=""display:inline-block;padding:12px 20px;background:#1f6feb;color:#fff;text-decoration:none;border-radius:6px;font-weight:600;"">Thanh toán ngay</a>
                                 </p>";
                }

                var contentSb = new StringBuilder();
                contentSb.AppendLine($"<p>Xin chào <strong>{WebUtility.HtmlEncode(toName ?? "Khách hàng")}</strong>,</p>");
                contentSb.AppendLine("<p>Cảm ơn bạn đã đặt vé tại <strong>YC3 DAT VE CONCERT</strong>. Dưới đây là thông tin đơn hàng của bạn:</p>");
                contentSb.AppendLine("<table style=\"width:100%; margin-bottom:12px;\">");
                contentSb.AppendLine($"<tr><td style=\"width:140px;font-weight:600\">Mã đơn hàng</td><td>{WebUtility.HtmlEncode(orderId)}</td></tr>");
                contentSb.AppendLine($"<tr><td style=\"font-weight:600\">Sự kiện</td><td>{WebUtility.HtmlEncode(concertName)}</td></tr>");
                contentSb.AppendLine($"<tr><td style=\"font-weight:600\">Ngày diễn</td><td>{concertDate:dd/MM/yyyy HH:mm}</td></tr>");
                contentSb.AppendLine($"<tr><td style=\"font-weight:600\">Tổng</td><td>{totalAmount:C}</td></tr>");
                contentSb.AppendLine("</table>");

                contentSb.AppendLine("<h3 style=\"margin-top:6px;\">Danh sách chỗ ngồi</h3>");
                contentSb.AppendLine(seatsSb.ToString());

                contentSb.AppendLine(ctaHtml);

                contentSb.AppendLine("<p style=\"color:#666;font-size:13px;\">Nếu bạn có câu hỏi, vui lòng liên hệ hỗ trợ của chúng tôi.</p>");
                contentSb.AppendLine("<p style=\"margin-top:20px;color:#999;font-size:12px;\">Trân trọng,<br/>Đội ngũ YC3 DAT VE CONCERT</p>");

                var fullHtml = WrapHtmlTemplate(_emailSettings.SenderName ?? "YC3 DAT VE CONCERT", _emailSettings.SenderEmail ?? string.Empty, toName, contentSb.ToString(), isContentHtml: true);

                return SendEmail(toName, toEmail, subject, fullHtml, isHtml: true);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send order confirmation email.", ex);
            }
        }

        // Helper: wrap content into full HTML template
        private string WrapHtmlTemplate(string senderName, string senderEmail, string toName, string contentFragment, bool isContentHtml = true)
        {
            var header = WebUtility.HtmlEncode(senderName ?? "YC3 DAT VE CONCERT");
            var contact = WebUtility.HtmlEncode(senderEmail ?? string.Empty);

            var html = $@"<!doctype html>
                <html lang=""vi"">
                <head>
                  <meta charset=""utf-8"" />
                  <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
                  <style>
                    body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial; background-color:#f4f6f8; margin:0; padding:0; color:#333; }}
                    .container {{ max-width:680px; margin:24px auto; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(15,23,42,0.08); overflow:hidden; }}
                    .header {{ background:linear-gradient(90deg,#1f6feb,#60a5fa); color:#fff; padding:20px 24px; }}
                    .header h1 {{ margin:0; font-size:20px; font-weight:600; }}
                    .content {{ padding:24px; line-height:1.6; font-size:15px; }}
                    .footer {{ background:#fafafa; color:#666; padding:16px 24px; font-size:13px; }}
                    @media (max-width:600px) {{ .container {{ margin:12px; }} .content {{ padding:16px; }} }}
                  </style>
                </head>
                <body>
                  <div class=""container"">
                    <div class=""header"">
                      <h1>{header}</h1>
                    </div>
                    <div class=""content"">
                      {contentFragment}
                    </div>
                    <div class=""footer"">
                      <div>© {DateTime.UtcNow.Year} {header}. All rights reserved.</div>
                      <div style=""color:#888;font-size:13px;"">Liên hệ: {contact}</div>
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
    }
}
