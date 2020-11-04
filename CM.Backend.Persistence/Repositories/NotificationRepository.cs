using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface INotificationRepository : IMongoReadmodelRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsPageAsyncForId(HashSet<Guid> availableNotifications, int page,
            int pageSize);
        Task UpdateBatchForNotificationNameAsync(Guid invokerId, string name);
        Task UpdateBatchForNotificationInvokerImageAsync(Guid invokerId, Guid invokerProfileImgId);
        Task<IEnumerable<Notification>> GetLatestNotificationsForUserAsync(HashSet<Guid> availableNotifications,
            DateTime fromDate);
    }

    public class NotificationRepository : MongoReadmodelRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<Notification>> GetNotificationsPageAsyncForId(
            HashSet<Guid> availableNotifications, int page, int pageSize)
        {
            return await ExecuteCmd(() =>
                DefaultCollection
                    .Find(c => availableNotifications.Contains(c.Id))
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .SortByDescending(c => c.Date)
                    .ToListAsync(),
                $"{nameof(GetNotificationsPageAsyncForId)} - {nameof(NotificationRepository)}");
        }

        public async Task UpdateBatchForNotificationNameAsync(Guid invokerId, string name)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(u => u.InvokerId == invokerId, Update.Set(u => u.InvokerName, name)),
                $"{nameof(UpdateBatchForNotificationNameAsync)} - {nameof(NotificationRepository)}");
        }

        public async Task UpdateBatchForNotificationInvokerImageAsync(Guid invokerId, Guid invokerProfileImgId)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(u => u.InvokerId == invokerId,
                    Update.Set(u => u.InvokerProfileImgId, invokerProfileImgId)),
                $"{nameof(UpdateBatchForNotificationInvokerImageAsync)} - {nameof(NotificationRepository)}");
        }

        public async Task<IEnumerable<Notification>> GetLatestNotificationsForUserAsync(HashSet<Guid> availableNotifications, DateTime fromDate)
        {
            return await ExecuteCmd(() =>
                DefaultCollection.Find(
                    n => availableNotifications.Contains(n.Id)
                         && n.Date.CompareTo(fromDate) > 0).ToListAsync(),
                $"{nameof(GetLatestNotificationsForUserAsync)} - {nameof(NotificationRepository)}");
        }
    }
}