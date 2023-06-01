using System.Net.Mail;
using System.Net;

namespace NugetMoodReboot.Helpers
{
    public class HelperMailSMTP
    {
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

        private readonly string MailUser;
        private readonly string MailPassword;
        private readonly int Port;
        private readonly string Host;
        private readonly bool EnableSSL;
        private readonly bool DefaultCredentials;

        public HelperMailSMTP(string mailUser, string mailPassword, int port, string host, bool enableSSL, bool defaultCredentials)
        {
            this.MailUser = mailUser;
            this.MailPassword = mailPassword;
            this.Port = port;
            this.EnableSSL = enableSSL;
            this.DefaultCredentials = defaultCredentials;
            this.EnableSSL = enableSSL;
            this.Host = host;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje)
        {
            MailMessage mail = new()
            {
                From = new MailAddress(this.MailUser),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            return mail;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje, string path)
        {
            MailMessage mail = new()
            {
                From = new MailAddress(this.MailUser),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            Attachment attachment = new(path);
            mail.Attachments.Add(attachment);

            return mail;
        }

        private MailMessage ConfigureMailMessage(string para, string asunto, string mensaje, List<string> filePaths)
        {
            MailMessage mail = new()
            {
                From = new MailAddress(this.MailUser),
                Body = mensaje,
                Subject = asunto,
                IsBodyHtml = true,
            };

            mail.To.Add(new MailAddress(para));

            foreach (string path in filePaths)
            {
                Attachment attachment = new(path);
                mail.Attachments.Add(attachment);
            }

            return mail;
        }

        private SmtpClient CofigureSmtpClient()
        {
            SmtpClient smtpClient = new()
            {
                Host = this.Host,
                Port = this.Port,
                EnableSsl = this.EnableSSL,
                UseDefaultCredentials = this.DefaultCredentials,
                Credentials = new NetworkCredential(this.MailUser, this.MailPassword)
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
            string nuevoEmail = this.BuildMailTemplate(asunto, mensaje, baseUrl);
            MailMessage mail = this.ConfigureMailMessage(para, asunto, nuevoEmail, path);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }

        public Task SendMailAsync(string para, string asunto, string mensaje, List<string> filePaths, string baseUrl)
        {
            string nuevoEmail = this.BuildMailTemplate(asunto, mensaje, baseUrl);
            MailMessage mail = this.ConfigureMailMessage(para, asunto, nuevoEmail, filePaths);
            SmtpClient client = this.CofigureSmtpClient();
            return client.SendMailAsync(mail);
        }
    }
}
