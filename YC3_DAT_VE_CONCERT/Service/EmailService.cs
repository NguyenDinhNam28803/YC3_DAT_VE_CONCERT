using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
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
        public async Task SendEmail(string toName, string toEmail, string subject, string body)
        {
            // Implement email sending logic here
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();

            // Build HTML email with inline styles and a plain-text fallback.
            // Use HtmlEncode for user-provided body to avoid breaking the template,
            // but allow simple newlines -> paragraphs.
            string safeBody = WebUtility.HtmlEncode(body ?? string.Empty)
                .Replace("\r\n", "\n")   // normalize
                .Replace("\n\n", "</p><p>") // double newline => paragraph break
                .Replace("\n", "<br />");   // single newline => line break

            var html = $@"
                <!doctype html>
                <html lang=""vi"">
                <head>
                  <meta charset=""utf-8"" />
                  <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
                  <style>
                    /* Basic responsive email styles */
                    body {{
                      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial;
                      background-color: #f4f6f8;
                      margin: 0;
                      padding: 0;
                      color: #333333;
                    }}
                    .container {{
                      max-width: 680px;
                      margin: 24px auto;
                      background: #ffffff;
                      border-radius: 8px;
                      box-shadow: 0 2px 8px rgba(15, 23, 42, 0.08);
                      overflow: hidden;
                    }}
                    .header {{
                      background: linear-gradient(90deg,#1f6feb,#60a5fa);
                      color: #fff;
                      padding: 20px 24px;
                    }}
                    .header h1 {{
                      margin: 0;
                      font-size: 20px;
                      font-weight: 600;
                    }}
                    .content {{
                      padding: 24px;
                      line-height: 1.6;
                      font-size: 15px;
                    }}
                    .cta {{
                      display: inline-block;
                      margin: 18px 0;
                      padding: 12px 20px;
                      background: #1f6feb;
                      color: #fff !important;
                      text-decoration: none;
                      border-radius: 6px;
                      font-weight: 600;
                    }}
                    .footer {{
                      background: #fafafa;
                      color: #666;
                      padding: 16px 24px;
                      font-size: 13px;
                    }}
                    .muted {{
                      color: #888888;
                      font-size: 13px;
                    }}
                    @media (max-width: 600px) {{
                      .container {{ margin: 12px; }}
                      .content {{ padding: 16px; }}
                    }}
                  </style>
                </head>
                <body>
                  <div class=""container"">
                    <div class=""header"">
                      <h1>{WebUtility.HtmlEncode(_emailSettings.SenderName ?? "YC3 DAT VE CONCERT")}</h1>
                    </div>
                    <div class=""content"">
                      <p>Xin chào <strong>{WebUtility.HtmlEncode(toName ?? "Khách hàng")}</strong>,</p>

                      <p>{safeBody}</p>

                      <!-- Example CTA (remove or customize as needed) -->
                      <p>
                        <a class=""cta"" href=""#"" target=""_blank"">Xem chi tiết</a>
                      </p>

                      <p class=""muted"">Nếu bạn không yêu cầu email này, vui lòng bỏ qua.</p>
                    </div>

                    <div class=""footer"">
                      <div>© {DateTime.UtcNow.Year} {WebUtility.HtmlEncode(_emailSettings.SenderName ?? "YC3 DAT VE CONCERT")}. All rights reserved.</div>
                      <div class=""muted"">Liên hệ: {WebUtility.HtmlEncode(_emailSettings.SenderEmail ?? "")}</div>
                    </div>
                  </div>
                </body>
                </html>
                ";
            // Plain text fallback: decode encoded body and replace <br/>/<p> with newlines
            var plainText = (body ?? string.Empty).Replace("\r\n", "\n");

            bodyBuilder.HtmlBody = html;
            bodyBuilder.TextBody = plainText;
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
    }
}
