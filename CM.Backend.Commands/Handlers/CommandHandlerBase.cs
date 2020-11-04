using CM.Backend.Eventstore.Persistence;
using Serilog;

namespace CM.Backend.Commands.Handlers
{
    public abstract class CommandHandlerBase
    {
        protected readonly IPublishingAggregateRepository AggregateRepo;
        protected readonly ILogger Logger;

        protected CommandHandlerBase(IPublishingAggregateRepository aggregateRepo, ILogger logger)
        {
            AggregateRepo = aggregateRepo;
            Logger = logger;
        }
    }
}