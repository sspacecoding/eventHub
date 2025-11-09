using System.Net;
using System.Net.Mail;

namespace SpaceEventHub.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendEventCreatedEmailAsync(string organizerEmail, string organizerName, string eventTitle);
    Task SendRegistrationConfirmationEmailAsync(string attendeeEmail, string attendeeName, string eventTitle, DateTime eventDate);
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

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpHost = emailSettings["SmtpHost"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        var smtpUsername = emailSettings["SmtpUsername"];
        var smtpPassword = emailSettings["SmtpPassword"];
        var fromEmail = emailSettings["FromEmail"] ?? "noreply@spaceeventhub.com";
        var fromName = emailSettings["FromName"] ?? "SpaceEventHub";

        // If SMTP credentials are not configured, log the email instead of sending
        if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            _logger.LogInformation("Email would be sent to {ToEmail} with subject: {Subject}", toEmail, subject);
            _logger.LogInformation("Email body: {Body}", body);
            return;
        }

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
        }
    }

    public async Task SendEventCreatedEmailAsync(string organizerEmail, string organizerName, string eventTitle)
    {
        var subject = "Event Published Successfully - SpaceEventHub";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #000000; color: #E0E0E0; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #1a1a1a; padding: 30px; border-radius: 8px;'>
                    <h1 style='color: #FFFFFF; margin-bottom: 20px;'>Event Published Successfully!</h1>
                    <p>Hello {organizerName},</p>
                    <p>Your event <strong style='color: rgb(0, 0, 255);'>{eventTitle}</strong> has been successfully published on SpaceEventHub.</p>
                    <p>Developers can now discover and register for your event. You can manage your event from your dashboard.</p>
                    <p style='margin-top: 30px;'>
                        <a href='https://spaceeventhub.manus.space' style='background-color: rgb(0, 0, 255); color: #FFFFFF; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                            View Dashboard
                        </a>
                    </p>
                    <p style='margin-top: 30px; color: #999999; font-size: 12px;'>
                        This is an automated email from SpaceEventHub. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(organizerEmail, subject, body);
    }

    public async Task SendRegistrationConfirmationEmailAsync(string attendeeEmail, string attendeeName, string eventTitle, DateTime eventDate)
    {
        var subject = "Registration Confirmed - SpaceEventHub";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #000000; color: #E0E0E0; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #1a1a1a; padding: 30px; border-radius: 8px;'>
                    <h1 style='color: #FFFFFF; margin-bottom: 20px;'>Registration Confirmed!</h1>
                    <p>Hello {attendeeName},</p>
                    <p>You have successfully registered for <strong style='color: rgb(0, 0, 255);'>{eventTitle}</strong>.</p>
                    <p><strong>Event Date:</strong> {eventDate:MMMM dd, yyyy 'at' hh:mm tt}</p>
                    <p>We look forward to seeing you at the event!</p>
                    <p style='margin-top: 30px;'>
                        <a href='https://spaceeventhub.manus.space' style='background-color: rgb(0, 0, 255); color: #FFFFFF; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                            View My Events
                        </a>
                    </p>
                    <p style='margin-top: 30px; color: #999999; font-size: 12px;'>
                        This is an automated email from SpaceEventHub. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(attendeeEmail, subject, body);
    }
}
