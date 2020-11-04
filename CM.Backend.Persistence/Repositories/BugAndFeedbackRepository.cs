using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IBugAndFeedbackRepository : IMongoReadmodelRepository<BugAndFeedback>
    {
        Task<IEnumerable<BugAndFeedback>> GetSubmittedBugAndFeedback(int page, int pageSize, DateTime fromDate,
            DateTime toDate);
    }
    
    public class BugAndFeedbackRepository : MongoReadmodelRepository<BugAndFeedback>, IBugAndFeedbackRepository
    {
        public BugAndFeedbackRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<BugAndFeedback>> GetSubmittedBugAndFeedback(int page, int pageSize, DateTime fromDate, DateTime toDate)
        {
            return await ExecuteCmd(() =>
                    DefaultCollection
                        .Find(c => c.SubmittedDate.CompareTo(fromDate) > 0 && c.SubmittedDate.CompareTo(toDate) < 0)
                        .Skip(page * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
                $"{nameof(GetSubmittedBugAndFeedback)} - {nameof(BugAndFeedbackRepository)}");
        }
    }
}