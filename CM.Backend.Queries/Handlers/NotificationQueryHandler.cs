using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.StaticResources;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Model.NotificationModels;
using CM.Backend.Queries.Queries.NotificationQueries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class NotificationQueryHandler :
        IQueryHandler<GetNotificationsPagedAsync, IEnumerable<NotificationQueryModel>>,
        IQueryHandler<GetLatestNotificationUpdateForUserAsync, LatestNotificationUpdateModel>
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IUserRepository userRepository;

        public NotificationQueryHandler(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            this.notificationRepository = notificationRepository;
            this.userRepository = userRepository;
        }
        
        public async Task<IEnumerable<NotificationQueryModel>> HandleAsync(GetNotificationsPagedAsync query, CancellationToken ct)
        {

            var user = await userRepository.GetById(query.UserId);

            if (user == null)
            {
                return null;
            }

            if (user.AvailableNotifications == null)
            {
                user.AvailableNotifications = new HashSet<Guid>();
            }
            
            var result =
                await notificationRepository.GetNotificationsPageAsyncForId(user.AvailableNotifications, query.Page, query.PageSize);

            foreach (var notification in result)
            {
                if (notification.Message != null)
                {
                    if (notification.InvokedByMethod.Equals(NotificationMethod.CommunityUpdate))
                    {
                        notification.Message = PrepareNotificationMessage(notification.Message, user.Name);
                    }
                    else
                    {
                        notification.Message =
                            PrepareNotificationMessage(notification.Message, notification.InvokerName);
                    }
                }
            }

            return convertToNotificationQueryModels(result, user);

        }

        private IEnumerable<NotificationQueryModel> convertToNotificationQueryModels(
            IEnumerable<Notification> notifications, User user)
        {
            if (user.ReadNotifications == null)//If user has not been updated yet...
            {
                user.ReadNotifications = new HashSet<Guid>();
            }
            
            var list = new List<NotificationQueryModel>();

            foreach (var notification in notifications)
            {
                var convertedNotification = new NotificationQueryModel();
                GenericMapper<Notification, NotificationQueryModel>.Map(notification, convertedNotification);
                if (user.ReadNotifications.Contains(notification.Id))
                {
                    convertedNotification.IsRead = true;
                }
                list.Add(convertedNotification);
            }

            return list;
        }
        
        private string PrepareNotificationMessage(string message, string placeholderContent)
        {
            const string placeholder = "{InvokedByName}";

            var preparedString = message.Replace(placeholder, placeholderContent);

            return preparedString;
        }

        public async Task<LatestNotificationUpdateModel> HandleAsync(GetLatestNotificationUpdateForUserAsync query, CancellationToken ct)
        {
            var user = await userRepository.GetById(query.UserId);

            if (user == null)
            {
                return null;
            }

            IEnumerable<Notification> newNotifications = null;
            if (!query.IncludeUpdates)
            {
                var lastSeenNotification = user.LastNotificationSeen;
                DateTime notificationsLastUpdatedOn = DateTime.MinValue;

                if (!lastSeenNotification.Equals(Guid.Empty))
                {
                    var notification = await notificationRepository.GetById(lastSeenNotification);
                    if (notification != null)
                    {
                        notificationsLastUpdatedOn = notification.Date;
                    }
                }
                
                 newNotifications = await notificationRepository.GetLatestNotificationsForUserAsync(user.AvailableNotifications,
                        notificationsLastUpdatedOn);
            }
            else
            {
                var fromNotification = await notificationRepository.GetById(query.FromId);

                if (fromNotification == null)
                {
                    return new LatestNotificationUpdateModel
                    {
                        NewNotifications = new List<NotificationQueryModel>(),
                        NumberOfUnreadNotifications = 0
                    };
                }
                
                newNotifications =
                    await notificationRepository.GetLatestNotificationsForUserAsync(user.AvailableNotifications,
                        fromNotification.Date);
            }

            foreach (var notification in newNotifications)
            {
                if (notification.Message != null)
                {
                    if (notification.InvokedByMethod.Equals(NotificationMethod.CommunityUpdate))
                    {
                        notification.Message = PrepareNotificationMessage(notification.Message, user.Name);
                    }
                    else
                    {
                        notification.Message =
                            PrepareNotificationMessage(notification.Message, notification.InvokerName);
                    }
                }
            }
            
            var unreadNotifications = new HashSet<Notification>();

            foreach (var notification in newNotifications)
            {
                if (user.ReadNotifications != null)
                {
                    if (!user.ReadNotifications.Contains(notification.Id))
                    {
                        unreadNotifications.Add(notification);
                    }
                }
            }

            if (!query.IncludeUpdates)
            {
                return new LatestNotificationUpdateModel
                {
                    NumberOfUnreadNotifications = unreadNotifications.Count,
                    NewNotifications = new List<NotificationQueryModel>()
                };
            }
            else
            {
                return new LatestNotificationUpdateModel
                {
                    NumberOfUnreadNotifications = unreadNotifications.Count,
                    NewNotifications = convertToNotificationQueryModels(newNotifications, user)
                };
            }
        }
    }
}