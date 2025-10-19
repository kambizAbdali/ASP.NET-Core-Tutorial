using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace IdentityCompleteProject.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendTwoFactorCodeAsync(string email, string code);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            try
            {
                // In a real application, integrate with an email provider like SendGrid, SMTP, etc.
                var subject = "Confirm your email";
                var body = $@"
                    <h1>Welcome to Our Application!</h1>
                    <p>Please confirm your email by clicking the link below:</p>
                    <a href='{confirmationLink}'>Confirm Email</a>
                    <p>If you didn't create an account, please ignore this email.</p>";

                // Simulate email sending
                _logger.LogInformation("Confirmation email sent to {Email}: {ConfirmationLink}",
                    email, confirmationLink);

                // Here you would typically call your email service
                await SendEmailAsync(email, subject, body);





                //var from = new EmailAddress("your-email@example.com", "Your Application"); // Replace with your email



                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", email);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                var subject = "Reset your password";
                var body = $@"
                    <h1>Password Reset Request</h1>
                    <p>You requested to reset your password. Click the link below:</p>
                    <a href='{resetLink}'>Reset Password</a>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>This link will expire in 1 hour.</p>";

                _logger.LogInformation("Password reset email sent to {Email}: {ResetLink}",
                    email, resetLink);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", email);
                throw;
            }
        }

        public async Task SendTwoFactorCodeAsync(string email, string code)
        {
            try
            {
                var subject = "Your two-factor authentication code";
                var body = $@"
                    <h1>Two-Factor Authentication</h1>
                    <p>Your verification code is: <strong>{code}</strong></p>
                    <p>This code will expire in 5 minutes.</p>
                    <p>If you didn't request this code, please secure your account.</p>";

                _logger.LogInformation("2FA code sent to {Email}: {Code}", email, code);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending 2FA code to {Email}", email);
                throw;
            }
        }


        public async Task SendEmailAsync(string email, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("abdaliKambiz@gmail.com", "kami1372"); // from

                var message = new MailMessage
                {
                    From = new MailAddress("abdaliKambiz@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(email);

                await client.SendMailAsync(message);
            }
        }
    }
}