using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace cem.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
        _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? throw new InvalidOperationException("SMTP_USERNAME non configurato");
        _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new InvalidOperationException("SMTP_PASSWORD non configurato");
        _fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? throw new InvalidOperationException("SMTP_FROM_EMAIL non configurato");
        _fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME");
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation(
                "Tentativo di invio email - Da: {From} ({FromName}), A: {To}, Oggetto: {Subject}, Server SMTP: {Server}:{Port}",
                _fromEmail,
                _fromName,
                to,
                subject,
                _smtpServer,
                _smtpPort
            );

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            var startTime = DateTime.UtcNow;
            await client.SendMailAsync(message);
            var duration = DateTime.UtcNow - startTime;

            _logger.LogInformation(
                "Email inviata con successo - Da: {From} ({FromName}), A: {To}, Oggetto: {Subject}, Durata: {Duration}ms",
                _fromEmail,
                _fromName,
                to,
                subject,
                duration.TotalMilliseconds
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Errore durante l'invio dell'email - Da: {From} ({FromName}), A: {To}, Oggetto: {Subject}, Server SMTP: {Server}:{Port}",
                _fromEmail,
                _fromName,
                to,
                subject,
                _smtpServer,
                _smtpPort
            );
            throw;
        }
    }
}