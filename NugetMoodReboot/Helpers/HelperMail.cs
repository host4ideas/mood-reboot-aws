using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace NugetMoodReboot.Helpers
{
    public class MailLink
    {
        public string Link { get; set; }
        public string LinkText { get; set; }
    }

    public class HelperMail
    {
        private readonly IConfiguration configuration;
        private string MRBaseMail = @"<!DOCTYPE html>
<html
    lang=""en""
    xmlns=""http://www.w3.org/1999/xhtml""
    xmlns:o=""urn:schemas-microsoft-com:office:office""
>
    <head>
        <meta charset=""UTF-8"" />
        <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
        <meta name=""x-apple-disable-message-reformatting"" />
        <title></title>
        <!--[if mso]>
            <noscript>
                <xml>
                    <o:OfficeDocumentSettings>
                        <o:PixelsPerInch>96</o:PixelsPerInch>
                    </o:OfficeDocumentSettings>
                </xml>
            </noscript>
        <![endif]-->
        <style>
            table,
            td,
            div,
            h1,
            p {
                font-family: Arial, sans-serif;
            }
        </style>
    </head>
    <body style=""margin: 0; padding: 0"">
        <table
            role=""presentation""
            style=""
                width: 100%;
                border-collapse: collapse;
                border: 0;
                border-spacing: 0;
                background: #ffffff;
            ""
        >
            <tr>
                <td align=""center"" style=""padding: 0"">
                    <table
                        role=""presentation""
                        style=""
                            width: 602px;
                            border-collapse: collapse;
                            border: 1px solid #cccccc;
                            border-spacing: 0;
                            text-align: left;
                        ""
                    >
                        <tr>
                            <td
                                align=""center""
                                style=""
                                    padding: 40px 0 30px 0;
                                    background: #70bbd9;
                                ""
                            >
                            <!-- MoodReboot Logo -->
                                <img
                                    src=""%MOODREBOOT_IMAGE%""
                                    alt=""""
                                    width=""200""
                                    style=""height: auto; display: block""
                                />
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding: 36px 30px 42px 30px"">
                                <table
                                    role=""presentation""
                                    style=""
                                        width: 100%;
                                        border-collapse: collapse;
                                        border: 0;
                                        border-spacing: 0;
                                    ""
                                >
                                    <tr>
                                        <td
                                            style=""
                                                padding: 0 0 36px 0;
                                                color: #153643;
                                            ""
                                        >
                                            <h1
                                                style=""
                                                    font-size: 24px;
                                                    margin: 0 0 20px 0;
                                                    font-family: Arial,
                                                        sans-serif;
                                                ""
                                            >
                                                %SUBJECT%
                                            </h1>
                                            <p
                                                style=""
                                                    margin: 0 0 12px 0;
                                                    font-size: 16px;
                                                    line-height: 24px;
                                                    font-family: Arial,
                                                        sans-serif;
                                                ""
                                            >
                                                %BODY%
                                            </p>
                                            <div
                                                style=""
                                                    display: flex;
                                                    justify-content: center;
                                                    gap: 10px;
                                                ""
                                            >
                                                %LINKS%
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding: 30px; background: #ee4c50"">
                                <table
                                    role=""presentation""
                                    style=""
                                        width: 100%;
                                        border-collapse: collapse;
                                        border: 0;
                                        border-spacing: 0;
                                        font-size: 9px;
                                        font-family: Arial, sans-serif;
                                    ""
                                >
                                    <tr>
                                        <td
                                            style=""padding: 0; width: 50%""
                                            align=""left""
                                        >
                                            <p
                                                style=""
                                                    margin: 0;
                                                    font-size: 14px;
                                                    line-height: 16px;
                                                    font-family: Arial,
                                                        sans-serif;
                                                    color: #ffffff;
                                                ""
                                            >
                                                <a
                                                    href=""%MOODREBOOTLINK""
                                                    style=""
                                                        color: #ffffff;
                                                        text-decoration: underline;
                                                    ""
                                                    >&reg; MoodReboot, Spain 2023</a
                                                >
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>
";

        public HelperMail(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje)
        {
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");

            MailMessage mail = new()
            {
                From = new MailAddress(user),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            return mail;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje, string path)
        {
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");

            MailMessage mail = new()
            {
                From = new MailAddress(user),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            Attachment attachment = new(path);
            mail.Attachments.Add(attachment);

            return mail;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje, List<string> paths)
        {
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");

            MailMessage mail = new()
            {
                From = new MailAddress(user),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            foreach (string path in paths)
            {
                Attachment attachment = new(path);
                mail.Attachments.Add(attachment);
            }

            return mail;
        }

        private SmtpClient CofigureSmtpClient()
        {
            string user = this.configuration.GetValue<string>("MailSettings:Credentials:User");
            string password = this.configuration.GetValue<string>("MailSettings:Credentials:Password");
            string hostName = this.configuration.GetValue<string>("MailSettings:Smtp:Host");
            int port = this.configuration.GetValue<int>("MailSettings:Smtp:Port");
            bool enableSSL = this.configuration.GetValue<bool>("MailSettings:Smtp:EnableSSL");
            bool defaultCredentials = this.configuration.GetValue<bool>("MailSettings:Smtp:DefaultCredentials");

            SmtpClient smtpClient = new()
            {
                Host = hostName,
                Port = port,
                EnableSsl = enableSSL,
                UseDefaultCredentials = defaultCredentials,
                Credentials = new NetworkCredential(user, password)
            };

            return smtpClient;
        }

        public string BuildMailTemplate(string asunto, string mensaje, string baseUrl, List<MailLink>? links = null)
        {
            string nuevoEmail = this.MRBaseMail;
            nuevoEmail = nuevoEmail.Replace("%SUBJECT%", asunto);
            nuevoEmail = nuevoEmail.Replace("%BODY%", mensaje);
            nuevoEmail = nuevoEmail.Replace("%MOODREBOOTLINK%", baseUrl);
            nuevoEmail = nuevoEmail.Replace("%MOODREBOOT_IMAGE%", "https://live.staticflickr.com/65535/52772156860_2cdcd949cb_m.jpg");

            string linksHtml = "";

            if (links != null)
            {
                foreach (MailLink link in links)
                {
                    linksHtml += $@"                                                
                            <p
                                style=""
                                    margin: 0;
                                    font-size: 16px;
                                    line-height: 24px;
                                    font-family: Arial,
                                        sans-serif;
                                ""
                            >
                                <a
                                    href=""{link.Link}""
                                    style=""
                                        color: #ee4c50;
                                        text-decoration: underline;
                                    ""
                                    >{link.LinkText}</a
                                >
                            </p>";
                }
            }

            nuevoEmail = nuevoEmail.Replace("%LINKS%", linksHtml);
            return nuevoEmail;
        }

        public Task SendMailAsync(string para, string asunto, string mensaje, string baseUrl)
        {
            string nuevoEmail = this.BuildMailTemplate(asunto, mensaje, baseUrl);
            MailMessage mail = this.ConfigureMailMessage(para, asunto, nuevoEmail);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }

        public Task SendMailAsync(string para, string asunto, string mensaje, List<MailLink> links, string baseUrl)
        {
            string nuevoEmail = this.BuildMailTemplate(asunto, mensaje, baseUrl, links);
            MailMessage mail = this.ConfigureMailMessage(para, asunto, nuevoEmail);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }

        public Task SendMailAsync(string para, string asunto, string mensaje, string path, string baseUrl)
        {
            MailMessage mail = this.ConfigureMailMessage(para, asunto, mensaje, path);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }

        public Task SendMailAsync(string para, string asunto, string mensaje, List<string> paths, string baseUrl)
        {
            MailMessage mail = this.ConfigureMailMessage(para, asunto, mensaje, paths);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }
    }
}
