using MailKit.Net.Smtp;
using MimeKit;

namespace RepairShop.Helpers;

public class EmailHelper
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailHelper(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
    }

    public async Task SendOrderCompleteEmail(string toEmail, string firstName, string lastName, string itemName)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Your App Name", _smtpUser));
        email.To.Add(new MailboxAddress("Bob's It Repair", toEmail));
        email.Subject = "Order Complete";

        email.Body = new TextPart("html")
        {
            Text = $@"
                <p>Hello dear {firstName} {lastName}!</p>
                <p>We are happy to inform you that your {itemName} has been repaired and is ready to be picked up!</p>
                <p>We hope you are satisfied with our services.</p>
                <p>Kind regards,</p>
                <p>Bob's it repair</p>
            "
        };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_smtpServer, _smtpPort, true); 
            await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}