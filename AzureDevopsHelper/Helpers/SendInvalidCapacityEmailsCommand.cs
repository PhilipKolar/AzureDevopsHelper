using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Microsoft.Extensions.Logging;

namespace AzureDevopsHelper.Helpers
{
    public class SendInvalidCapacityEmailsCommand
    {
        private readonly ConfigContainer _config;
        private readonly ILogger _logger;

        public SendInvalidCapacityEmailsCommand(ConfigContainer config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task RunRequestAsync(SendInvalidCapacityEmailsCommandRequest request)
        {
            foreach (var member in request.InvalidCapacities)
            {
                _logger.LogInformation($"Send an email to {member.Email} (user {member.DisplayName}), with current hours {member.CurrentCapacity} and correct hours {member.CorrectCapacity}");
                await SendEmail(member);
            }
        }
         
        private async Task SendEmail(MemberCapacity member)
        {
            var displayName = member.DisplayName.Split("<")[0].Trim();

            MailMessage mail = new MailMessage(_config.EmailCredentialsUserName, member.Email);
            mail.Subject = $"Completed Hours Mismatch - {member.CurrentCapacity}/{member.CorrectCapacity}";
            mail.Body = $"Hi {displayName},\n\n" +
                        $"Your completed hours are {member.CurrentCapacity}, but you should have {member.CorrectCapacity} hours as of {DateTime.Today.AddDays(-1):yyyy/MM/dd}.\n\n" +
                        $"Please note that public holidays, annual leave, sick leave, and all other leave should be counted towards your completed hours.\n\n" +
                        $"Beep Boop";

            SmtpClient client = new SmtpClient();
            client.Host = _config.EmailHost;
            client.Port = _config.EmailPort;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = _config.EmailEnableSsl;
            client.Credentials = new NetworkCredential(_config.EmailCredentialsUserName, _config.EmailCredentialsPassword);
            client.Send(mail);
        }
    }
}
