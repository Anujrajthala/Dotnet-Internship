using System.Net;
using System.Net.Mail;
using TodoApi.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;
    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger){
        _configuration = configuration;
        _logger = logger;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        string SmtpHost = _configuration["Email:SmtpHost"];
        int SmtpPort = int.Parse(_configuration["Email:SmtpPort"]);
        string SmtpUser = _configuration["Email:SmtpUser"];
        string SmtpPass = _configuration["Email:SmtpPass"];
        string SmtpFrom = _configuration["Email:From"];

        using var client = new SmtpClient(SmtpHost,SmtpPort){
            EnableSsl = true,
            Credentials = new NetworkCredential(SmtpUser,SmtpPass)

        };
        using var mail = new MailMessage(SmtpFrom,to,subject,body){
            IsBodyHtml = true
        };
        try{
            await client.SendMailAsync(mail);
        }
        catch(Exception ex){
            _logger.LogError(ex,"Error sending email for verification.");
        }
    }
}