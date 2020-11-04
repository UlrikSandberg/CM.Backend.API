using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Baseline.Dates;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.Notification.Events;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.EventHandlers.Helpers;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using SimpleSoft.Mediator;
using Notification = CM.Backend.Persistence.Model.Notification;
using NotificationMethod = CM.Backend.Persistence.EnumOptions.NotificationMethod;

namespace CM.Backend.EventHandlers
{
    public class NotificationRMEventHandler :
        IEventHandler<MessageEnvelope<NotificationCreated>>,
        IEventHandler<MessageEnvelope<UserInfoEdited>>,
        IEventHandler<MessageEnvelope<UserImageCustomizationEdited>>
    {
        private readonly IUserRepository userRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IOptions<NotificationHubConfiguration> config;
        private readonly ICommentRepository commentRepository;
        private readonly IFollowRepository followRepository;
        private readonly IFollowBrandRepository followBrandRepository;

        public NotificationRMEventHandler(IUserRepository userRepository, INotificationRepository notificationRepository, IOptions<NotificationHubConfiguration> config, ICommentRepository commentRepository, IFollowRepository followRepository, IFollowBrandRepository followBrandRepository)
        {
            this.userRepository = userRepository;
            this.notificationRepository = notificationRepository;
            this.config = config;
            this.commentRepository = commentRepository;
            this.followRepository = followRepository;
            this.followBrandRepository = followBrandRepository;
        }

        public async Task HandleAsync(MessageEnvelope<NotificationCreated> evt, CancellationToken ct)
        {
            NotificationMethod invokedByMethod =
                (NotificationMethod) Enum.Parse(typeof(NotificationMethod), evt.Event.InvokedByMethod.ToString());
            NotificationAction invokedByAction = 
                (NotificationAction) Enum.Parse(typeof(NotificationAction), evt.Event.InvokedByAction.ToString());

            var contextUrl = ContextUrlProvider.GetContextUrlForNotification(evt.Event);
            
            await notificationRepository.Insert(new Notification
            {
                Id = evt.Id,
                Type = evt.Event.Type.Value,
                InvokedByAction = invokedByAction,
                InvokedByMethod = invokedByMethod,
                Date = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(60),
                InvokerId = evt.Event.InvokerId.Value,
                InvokerName = evt.Event.InvokerName.Value,
                InvokerProfileImgId = evt.Event.InvokerProfileImageId.Value,
                Title = evt.Event.Title,
                Message = evt.Event.Message,
                ContextUrl = contextUrl,
                OriginalMessage = evt.Event.OriginalMessage,
                PlaceholderDictionary = evt.Event.PlaceholderDictionary
            });
            
            //When the notification is created we can safely add its id to the list of available notification for all eligble users respective to the invoking action
            if (evt.Event.InvokedByAction.Equals(Domain.EnumOptions.NotificationAction.UserAction))
            {
                await HandleUserActionNotifications(evt.Event, contextUrl);
            }

            if (evt.Event.InvokedByAction.Equals(Domain.EnumOptions.NotificationAction.BrandNews))
            {
                await HandleBrandNewsNotificationsAsync(evt.Event, contextUrl);
            }

            if (evt.Event.InvokedByAction.Equals(Domain.EnumOptions.NotificationAction.CommunityUpdate))
            {
                await HandleCommunityUpdateNotifications(evt.Event, contextUrl);
            }
        }

        
        private async Task HandleBrandNewsNotificationsAsync(NotificationCreated notification, string contextUrl)
        {
            await userRepository.AddNotificationForBrandFollowers(notification.InvokerId.Value, notification.Id);

            var brandFollowers = await followBrandRepository.FindFollowByBrandId(notification.InvokerId.Value);
            
            var receivers = new HashSet<Guid>();

            foreach (var follower in brandFollowers)
            {
                receivers.Add(follower.FollowByUserId);
            }
            
            var eligibleReceivers = await userRepository.GetEligibleUsersForNotificationAsync(receivers, NotificationMethod.BrandUpdate);

            if (eligibleReceivers != null)
            {
                if (eligibleReceivers.Any())
                {
                    await SendNotifications(PrepareNotificationMessage(notification.Message, notification.InvokerName.Value), contextUrl, notification.Id, eligibleReceivers);
                }
            }
        }

        private async Task HandleCommunityUpdateNotifications(NotificationCreated notification, string contextUrl)
        {
            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.CommunityUpdate))
            {
                //Since its a community update, all users should receive them in app, therefor we 
                await userRepository.AddNotificationForAllUsers(notification.Id);

                //Check if this update should be broadcastet.
                var placerHolderDic = notification.PlaceholderDictionary;

                if (placerHolderDic.ContainsKey("{BroadCastMessage}"))
                {
                    await SendNotifications(placerHolderDic["{BroadCastMessage}"], contextUrl, notification.Id, new List<string> {NotificationTags.CMNotification.ToString()});
                }
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.WelcomeMessage))
            {
                //Add welcome notification to new user
                await userRepository.AddNotificationForSpecificUsersAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()), notification.Id);
                //Should not broadcast or make pushNotification.
            }
        }
        
        private async Task HandleUserActionNotifications(NotificationCreated notification, string contextUrl)
        {
            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.UserFollowed))
            {
                //Add the notification id to the respective user who are eligible to receice the respective notification
                await userRepository.AddNotificationForSpecificUsersAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()),
                    notification.Id);
                
                //Send the notification to eligible users!
                await ResolveAndSendNotificationToEligibleReceiversAsync(notification, NotificationMethod.UserFollowed, contextUrl);
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.TastingCommented))
            {
                //Add the notification id to the respective user who are eligible to receive the respective notification
                await userRepository.AddNotificationForSpecificUsersAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()), notification.Id);

                //Send the notification to eligible users!
                await ResolveAndSendNotificationToEligibleReceiversAsync(notification,
                    NotificationMethod.TastingCommented, contextUrl);
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.TastingLiked))
            {
                //Add the notification id to the respective user who are eligible to receive the respective notification
                await userRepository.AddNotificationForSpecificUsersAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()), notification.Id);
                
                //Send the notification to eligible users!
                await ResolveAndSendNotificationToEligibleReceiversAsync(notification, NotificationMethod.TastingLiked, contextUrl);
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.CommentLiked))
            {
                //Add the notification id to the respective user who are eligible to receive the respective notification
                await userRepository.AddNotificationForSpecificUsersAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()), notification.Id);
                
                //Send the notification to eligible users!
                await ResolveAndSendNotificationToEligibleReceiversAsync(notification, NotificationMethod.CommentLiked, contextUrl);
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.ChampagneTasted))
            {
                var followers = await followRepository.GetFollowersOfUserId(notification.InvokerId.Value);

                var receivers = new HashSet<Guid>();

                foreach (var follower in followers)
                {
                    receivers.Add(follower.FollowById);
                }

                await userRepository.AddNotificationForSpecificUsersAsync(receivers, notification.Id);

                await ResolveAndSendNotificationToEligibleReceiversAsync(notification,
                    NotificationMethod.ChampagneTasted, receivers, contextUrl);
            }

            if (notification.InvokedByMethod.Equals(Domain.EnumOptions.NotificationMethod.ActivityInCommentThread))
            {
                //Get ids of all people whom have commented on the same tastingId within the last 10 days
                var convertedContextUrl = ContextUrlProvider.ConvertContextUrl(notification.ProvidedContextUrl);
                if (convertedContextUrl.Groups.Count > 4) //This means we have necessary info
                {
                    var tastingIdString = convertedContextUrl.Groups[4].Value;
                    Guid tastingId = Guid.Empty;
                    var result = Guid.TryParse(tastingIdString, out tastingId);
                    if (result)
                    {
                        var commentators =
                            await commentRepository.GetCommentsForContextIdFromPeriodAsync(tastingId, TimeSpan.FromDays(10));
                        
                        var receivers = new HashSet<Guid>();

                        foreach (var commentator in commentators)
                        {
                            receivers.Add(commentator.AuthorId);
                        }
                        
                        //The author of the new comment should not be notified when he post a comment
                        receivers.Remove(notification.Receivers.First().Value);

                        await userRepository.AddNotificationForSpecificUsersAsync(receivers, notification.Id);

                        await ResolveAndSendNotificationToEligibleReceiversAsync(notification,
                            NotificationMethod.ActivityInCommentThread,
                            receivers, contextUrl); //await HandleActivityInCommentThreadNotification(notification, receivers);

                    }
                }
            }
        }

        private async Task ResolveAndSendNotificationToEligibleReceiversAsync(NotificationCreated notification,
            NotificationMethod notificationMethod, string contextUrl)
        {
            var eligibleReceivers =
                await userRepository.GetEligibleUsersForNotificationAsync(new HashSet<Guid>(notification.Receivers.ConverToGuidList()),
                    notificationMethod);
            
            if (eligibleReceivers != null)
            {
                if (eligibleReceivers.Any())
                {
                    await SendNotifications(PrepareNotificationMessage(notification.Message, notification.InvokerName.Value), contextUrl, notification.Id, eligibleReceivers);
                }
            }
        }

        private async Task ResolveAndSendNotificationToEligibleReceiversAsync(NotificationCreated notification,
            NotificationMethod notificationMethod, HashSet<Guid> receivers, string contextUrl)
        {
            var eligibleReceivers =
                await userRepository.GetEligibleUsersForNotificationAsync(receivers,
                    notificationMethod);
            
            if (eligibleReceivers != null)
            {
                if (eligibleReceivers.Any())
                {
                    await SendNotifications(PrepareNotificationMessage(notification.Message, notification.InvokerName.Value), contextUrl, notification.Id, eligibleReceivers);
                }
            }
        }
        
        private async Task SendNotifications(string message, string contextUrl, Guid notificationId, IEnumerable<User> eligibleReceivers)
        {
            //Create notification template
            var templateParams = new Dictionary<string, string>
            {
                ["messageParam"] = message,
                ["contextUrlParam"] = contextUrl,
                ["notificationIdParam"] = notificationId.ToString(),
                ["soundParam"] = "default"
            };
            
            var client = NotificationHubClient.CreateClientFromConnectionString(
                config.Value.NotificationHubConnectionString, config.Value.NotificationHubName);

            foreach (var user in eligibleReceivers)
            {
                if (AssertSleepSettings(user))
                {
                    foreach (var deviceInstallation in user.DeviceInstallations)
                    {
                        await client
                            .SendTemplateNotificationAsync(templateParams,
                                "$InstallationId:{" + deviceInstallation.InstallationId + "}").ConfigureAwait(false);
                    }
                }
            } 
        }
        
        private async Task SendNotifications(string message, string contextUrl, Guid notificationId, IEnumerable<string> tags)
        {
            NotificationOutcome result = null;
        
            //Create notification template
            var templateParams = new Dictionary<string, string>
            {
                ["messageParam"] = message,
                ["contextUrlParam"] = contextUrl,
                ["notificationIdParam"] = notificationId.ToString(),
                ["soundParam"] = "default"
            };
            
            var client = NotificationHubClient.CreateClientFromConnectionString(
                config.Value.NotificationHubConnectionString, config.Value.NotificationHubName);

            await client.SendTemplateNotificationAsync(templateParams, tags).ConfigureAwait(false);
        }

        private string PrepareNotificationMessage(string message, string placeholderContent)
        {
            const string placeholder = "{InvokedByName}";

            var preparedString = message.Replace(placeholder, placeholderContent);

            return preparedString;
        }

        public async Task HandleAsync(MessageEnvelope<UserInfoEdited> evt, CancellationToken ct)
        {
            if (evt.Event.DidNameChange)
            {
                await notificationRepository.UpdateBatchForNotificationNameAsync(evt.Id, evt.Event.Name.Value);
            }
        }

        public async Task HandleAsync(MessageEnvelope<UserImageCustomizationEdited> evt, CancellationToken ct)
        {
            if (evt.Event.ProfileImageChanged)
            {
                await notificationRepository.UpdateBatchForNotificationInvokerImageAsync(evt.Id,
                    evt.Event.ProfileImageId.Value);
            }
        }

        /// <summary>
        /// Indicates whether the user should receive notifications respective to their timezone
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool AssertSleepSettings(User user)
        {
            //This is the timezone of the respective user
            var timezone = new DateTime(DateTime.UtcNow.Ticks + user.SleepSettings.utcOffset);

            var hourOfDay = timezone.Hour;

            if (hourOfDay > user.SleepSettings.NotifyFrom.Hours && hourOfDay < user.SleepSettings.NotifyTo.Hours -2)
            {
                return true;
            }
            return false;
        }
    }
}