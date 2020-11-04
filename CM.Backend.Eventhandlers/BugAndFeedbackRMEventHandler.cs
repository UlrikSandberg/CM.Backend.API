using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.BugAndFeedback.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class BugAndFeedbackRMEventHandler : 
        IEventHandler<MessageEnvelope<BugAndFeedbackSubmitted>>
    {
        private readonly IBugAndFeedbackRepository bugAndFeedbackRepository;

        public BugAndFeedbackRMEventHandler(IBugAndFeedbackRepository bugAndFeedbackRepository)
        {
            this.bugAndFeedbackRepository = bugAndFeedbackRepository;
        }


        public async Task HandleAsync(MessageEnvelope<BugAndFeedbackSubmitted> evt, CancellationToken ct)
        {
            await bugAndFeedbackRepository.Insert(new BugAndFeedback
            {
                Id = evt.Event.Id,
                UserId = evt.Event.UserId.Value,
                MayBeContacted = evt.Event.MayBeContacted,
                Type = evt.Event.Type.Value,
                Content = evt.Event.Content,
                ImageId = evt.Event.Imageid.Value,
                SubmittedDate = DateTime.UtcNow
            });
        }
    }
}