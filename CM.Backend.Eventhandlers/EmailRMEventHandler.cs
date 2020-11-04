using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.EventHandlers.Helpers.SendGrid;
using CM.Backend.EventHandlers.Helpers.SendGrid.DynamicEmailTemplates;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class EmailRMEventHandler : 
        IEventHandler<MessageEnvelope<UserCreated>>,
        IEventHandler<MessageEnvelope<UserEmailUpdated>>,
        IEventHandler<MessageEnvelope<EmailConfirmed>>,
        IEventHandler<MessageEnvelope<ConfirmationEmailResend>>
    {
        
        
        private readonly IEmailConfirmationRepository _emailConfirmationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<EventHandlerCMBackendUrlConfiguration> _urlConfigs;
        private readonly IOptions<EventHandlerSendGridConfiguration> _sendGridConfigs;
        private readonly ILogger _logger;
        private readonly IOptions<EmailAuthorityConfiguration> _emailAuthorityConfig;

        private readonly string ConfirmationTemplateId;
        private readonly string WelcomeEmailWithConfirmation;
        
        public EmailRMEventHandler(IEmailConfirmationRepository emailConfirmationRepository, IUserRepository userRepository, IOptions<EventHandlerCMBackendUrlConfiguration> urlConfigs, IOptions<EventHandlerSendGridConfiguration> sendGridConfigs, ILogger logger, IOptions<EmailAuthorityConfiguration> emailAuthorityConfig)
        {
            _emailConfirmationRepository = emailConfirmationRepository;
            _userRepository = userRepository;
            _urlConfigs = urlConfigs;
            _sendGridConfigs = sendGridConfigs;
            _logger = logger;
            _emailAuthorityConfig = emailAuthorityConfig;

            ConfirmationTemplateId = sendGridConfigs.Value.ConfirmationTemplateId;
            WelcomeEmailWithConfirmation = sendGridConfigs.Value.WelcomeEmailWithConfirmation;
        }

        public async Task HandleAsync(MessageEnvelope<UserCreated> evt, CancellationToken ct)
        {
            var confirmationToken = Guid.NewGuid();
            
            //Start confirmationEmailProcedure by generating an emailConfirmationProcess
            _emailConfirmationRepository.Insert(new EmailConfirmationProcess
            {
                Id = Guid.NewGuid(),
                UserId = evt.Event.Id,
                Email = evt.Event.Email.Value,
                ConfirmationToken = Hash_sha256(confirmationToken.ToString()),
                ConfirmationEmailInitiatedAt = DateTime.UtcNow,
                IsActive = true
            });

            //Configure email personalizations
            var personalizations = new Personalization<ConfirmEmailTemplateData>();
            personalizations.to = new List<To> {new To {email = evt.Event.Email.Value, name = evt.Event.Name.Value}};
            personalizations.dynamic_template_data = new ConfirmEmailTemplateData
            {
                confirmationEmailLink = $"{_urlConfigs.Value.ApiBaseUrl}/api/v1/confirmEmail/{confirmationToken}",
                confirmingEmail = evt.Event.Email.Value,
                name = evt.Event.Name.Value
            };
            
            //Send welcome confirmation email
            SendConfirmationEmail(evt.Event.Email.Value, WelcomeEmailWithConfirmation, personalizations);

        }

        public async Task HandleAsync(MessageEnvelope<UserEmailUpdated> evt, CancellationToken ct)
        {
            //Confirmation token to new email.
            var confirmationToken = Guid.NewGuid();
            
            //Invalidate all confirmationEmail, respective to this user.
            await _emailConfirmationRepository.InvalidateAllEmailConfirmationProcessesForUser(evt.Id);
            
            //Start confirmationEmailProcedure by generating an emailConfirmationProcess
            _emailConfirmationRepository.Insert(new EmailConfirmationProcess
            {
                Id = Guid.NewGuid(),
                UserId = evt.Event.Id,
                Email = evt.Event.Email.Value,
                ConfirmationToken = Hash_sha256(confirmationToken.ToString()),
                ConfirmationEmailInitiatedAt = DateTime.UtcNow,
                IsActive = true
            });
            
            //Configure email personalizations
            var personalizations = new Personalization<ConfirmEmailTemplateData>();
            personalizations.to = new List<To> {new To {email = evt.Event.Email.Value, name = evt.Event.Name.Value}};
            personalizations.dynamic_template_data = new ConfirmEmailTemplateData
            {
                confirmationEmailLink = $"{_urlConfigs.Value.ApiBaseUrl}/api/v1/confirmEmail/{confirmationToken}",
                confirmingEmail = evt.Event.Email.Value,
                name = evt.Event.Name.Value
            };
            
            //Send welcome confirmation email
            await SendConfirmationEmail(evt.Event.Email.Value, ConfirmationTemplateId, personalizations);

        }

        private async Task SendConfirmationEmail(string toEmail, string templateId, Personalization<ConfirmEmailTemplateData> personalized)
        {
            //Start SendGridClient
            var client = new SendGridClient(_sendGridConfigs.Value.API_Key);
            
            //Configure email
            var email = new SendGridEmailTemplate<ConfirmEmailTemplateData>(templateId);
           
            //Add personalization to email!
            email.AddPersonalization(personalized);

            //Email from parameters
            email.from = new From
            {
                email = _emailAuthorityConfig.Value.Email,
                name = _emailAuthorityConfig.Value.Name
            };

            var json = JsonConvert.SerializeObject(email, Formatting.None);

            var response = await client.SendTransactionalEmail(json);

            if (!response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    _logger.Fatal("Failed to send confirmationEmail {@Email}, failed with {StatusCode} and {@Message}", email, response.StatusCode, errorMsg);
                    Console.WriteLine(errorMsg);
                }
                _logger.Fatal("Failed to send confirmationEmail {@Email}, failed with {StatusCode} and {@Message}", email, response.StatusCode, response.ReasonPhrase);
            }
        }

        public static string Hash_sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public async Task HandleAsync(MessageEnvelope<EmailConfirmed> evt, CancellationToken ct)
        {
            await _emailConfirmationRepository.InvalidateAllEmailConfirmationProcessesForUser(evt.Id);
        }

        public async Task HandleAsync(MessageEnvelope<ConfirmationEmailResend> evt, CancellationToken ct)
        {
           
            var confirmationToken = Guid.NewGuid();
            
            //Invalidate all confirmationEmail, respective to this user.
            await _emailConfirmationRepository.InvalidateAllEmailConfirmationProcessesForUser(evt.Id);
            
            //Start confirmationEmailProcedure by generating an emailConfirmationProcess
            _emailConfirmationRepository.Insert(new EmailConfirmationProcess
            {
                Id = Guid.NewGuid(),
                UserId = evt.Event.Id,
                Email = evt.Event.Email.Value,
                ConfirmationToken = Hash_sha256(confirmationToken.ToString()),
                ConfirmationEmailInitiatedAt = DateTime.UtcNow,
                IsActive = true
            });
            
            //Configure email personalizations
            var personalizations = new Personalization<ConfirmEmailTemplateData>();
            personalizations.to = new List<To> {new To {email = evt.Event.Email.Value, name = evt.Event.Name.Value}};
            personalizations.dynamic_template_data = new ConfirmEmailTemplateData
            {
                confirmationEmailLink = $"{_urlConfigs.Value.ApiBaseUrl}/api/v1/confirmEmail/{confirmationToken}",
                confirmingEmail = evt.Event.Email.Value,
                name = evt.Event.Name.Value
            };
            
            //Send welcome confirmation email
            await SendConfirmationEmail(evt.Event.Email.Value, ConfirmationTemplateId, personalizations);
        }
    }
}