using Microsoft.AspNetCore.Mvc;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;


namespace EmailMoodReboot.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    static readonly string mailUser = "moodreboot@gmail.com";
    static readonly string mailPassword = "miqgrxbpypgbjbxg";
    static readonly int port = 25;
    static readonly string host = "smtp.gmail.com";
    static readonly bool enableSSL = true;
    static readonly bool defaultCredentials = false;
    static readonly HelperMailSMTP helperMail = new(mailUser, mailPassword, port, host, enableSSL, defaultCredentials);

    // POST api/values
    [HttpPost]
    public async Task<bool> Post([FromBody] SendEmailModel email)
    {
        if (email.Links != null) {
            return await SendMailAsync(email.To, email.Subject, email.Message, email.Links, email.BaseUrl);
        }
        return await SendMailAsync(email.To, email.Subject, email.Message, email.Links, email.BaseUrl);
    }

    public static async Task<bool> SendMailAsync
    (string para, string asunto, string mensaje, string baseUrl) {
        await helperMail.SendMailAsync(para, asunto, mensaje, baseUrl);
        return true;
    }

    public static async Task<bool> SendMailAsync(string para, string asunto, string mensaje, List<MailLink> links, string baseUrl) {
        await helperMail.SendMailAsync(para, asunto, mensaje, links, baseUrl);
        return true;
    }
}
