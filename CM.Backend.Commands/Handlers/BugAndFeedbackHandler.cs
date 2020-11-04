using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.BugAndFeedbackCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.BugAndFeedback;
using CM.Backend.Domain.Aggregates.BugAndFeedback.Commands;
using CM.Backend.Domain.Aggregates.BugAndFeedback.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Handlers
{
    public class BugAndFeedbackHandler : CommandHandlerBase,
        ICommandHandler<SubmitBugOrFeedback, Response>
    {
        private readonly IPublishingAggregateRepository aggregateRepo;

        public BugAndFeedbackHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
            this.aggregateRepo = aggregateRepo;
        }

        public async Task<Response> HandleAsync(SubmitBugOrFeedback cmd, CancellationToken ct)
        {
            var bugOrFeedback = new BugAndFeedback();

            bugOrFeedback.Execute(new SubmitBugAndFeedback(
                new AggregateId(Guid.NewGuid()),
                new AggregateId(cmd.UserId),
                cmd.MayBeContacted,
                new BugAndFeedbackType(cmd.Type),
                cmd.Content,
                new ImageId(cmd.ImageId)));

            await AggregateRepo.StoreAsync(bugOrFeedback);
            
            return Response.Success();
        }
    }
}