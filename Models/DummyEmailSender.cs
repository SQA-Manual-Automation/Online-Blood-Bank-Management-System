using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // No actual email sending needed for university project
        return Task.CompletedTask;
    }
}
