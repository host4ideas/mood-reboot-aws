using Amazon.Lambda.Core;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmailServiceMoodReboot
{
    public class Function
    {
        static readonly string mailUser = "moodreboot@gmail.com";
        static readonly string mailPassword = "miqgrxbpypgbjbxg";
        static readonly int port = 25;
        static readonly string host = "smtp.gmail.com";
        static readonly bool enableSSL = true;
        static readonly bool defaultCredentials = false;
        static readonly HelperMailSMTP helperMail = new(mailUser, mailPassword, port, host, enableSSL, defaultCredentials);

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> FunctionHandler(SendEmailModel email, ILambdaContext context)
        {
            if (email.Links != null)
            {
                return await SendMailAsync(email.To, email.Subject, email.Message, email.Links, email.BaseUrl);
            }
            return await SendMailAsync(email.To, email.Subject, email.Message, email.Links, email.BaseUrl);
        }

        public static async Task<bool> SendMailAsync
            (string para, string asunto, string mensaje, string baseUrl)
        {
            await helperMail.SendMailAsync(para, asunto, mensaje, baseUrl);
            return true;
        }

        public static async Task<bool> SendMailAsync(string para, string asunto, string mensaje, List<MailLink> links, string baseUrl)
        {
            await helperMail.SendMailAsync(para, asunto, mensaje, links, baseUrl);
            return true;
        }
    }
}
