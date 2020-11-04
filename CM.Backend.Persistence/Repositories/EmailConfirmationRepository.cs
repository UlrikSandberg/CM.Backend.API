using System;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IEmailConfirmationRepository : IMongoReadmodelRepository<EmailConfirmationProcess>
    {
        Task InvalidateAllEmailConfirmationProcessesForUser(Guid userId);
        Task<EmailConfirmationProcess> GetByToken(string token);
    }
    
    public class EmailConfirmationRepository : MongoReadmodelRepository<EmailConfirmationProcess>, IEmailConfirmationRepository
    {
        public EmailConfirmationRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task InvalidateAllEmailConfirmationProcessesForUser(Guid userId)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(u => u.UserId == userId, Update.Set(u => u.IsActive, false)),
                $"{nameof(InvalidateAllEmailConfirmationProcessesForUser)} - {nameof(EmailConfirmationRepository)}");
        }

        public async Task<EmailConfirmationProcess> GetByToken(string token)
        {
            return await ExecuteCmd(() =>
                DefaultCollection.Find(u => u.ConfirmationToken == token).SingleOrDefaultAsync(),
                $"{nameof(GetByToken)} - {nameof(EmailConfirmationRepository)}");
        }
    }
}