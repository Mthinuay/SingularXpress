
using SendGrid;
using SendGrid.Helpers.Mail;
using SingularExpress.Interfaces;

namespace SingularExpress.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private const string FromEmail = "nokubongangema8@gmail.com";
        private const string FromName = "Singular Express";

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string otp, string username)
        {
            try
            {
                var apiKey = _config["SendGrid:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("SendGrid API key is not configured");
                }

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(FromEmail, FromName);
                var to = new EmailAddress(toEmail, username);
                var subject = "Your Password Reset Code";

                string resetLink = $"http://localhost:3000/reset-password?email={Uri.EscapeDataString(toEmail)}&step=otp";
                _logger.LogInformation($"Generated password reset link: {resetLink}");

                var plainTextContent = $"Hello {username},\n\n" +
                                      $"Your password reset code is: {otp}\n\n" +
                                      $"This code will expire in 30 minutes.\n\n" +
                                      $"To reset your password, click the link below:\n{resetLink}";
                                     

                var htmlContent = $@"
                    <html>
                        <body>
                            <h2>Hello {username},</h2>
                            <p>Your password reset code is: <strong>{otp}</strong></p>
                            <p>This code will expire in 30 minutes.</p>
                             <p>Click the link below to reset your password:</p>
                            <a href='{resetLink}' style='color:#1a73e8;'>Reset Password</a>
                        </body>
                    </html>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Body.ReadAsStringAsync();
                    throw new Exception($"Failed to send email. Status: {response.StatusCode}, Response: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email");
                throw; // Re-throw to let the controller handle it
            }
        }
            public async Task SendPasswordResetConfirmationEmailAsync(string toEmail, string username)
            {
                var apiKey = _config["SendGrid:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("SendGrid API key is not configured");

                var client = new SendGridClient(apiKey);
                var from   = new EmailAddress(FromEmail, FromName);
                var to     = new EmailAddress(toEmail, username);
                var subject = "Password successfully changed";

                var plain  = $"Hello {username},\n\nYour password was changed on {DateTime.UtcNow:yyyy‑MM‑dd HH:mm} UTC.";
                var html   = $@"
                    <html>
                        <body>
                            <h2>Hello {username},</h2>
                            <p>Your password was successfully changed on {DateTime.UtcNow:yyyy‑MM‑dd HH:mm} UTC.</p>
                            <p>If you did not make this change, contact support immediately.</p>
                        </body>
                    </html>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plain, html);
                var resp = await client.SendEmailAsync(msg);
                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Failed to send confirmation. Status {resp.StatusCode}");
            }


    }
}