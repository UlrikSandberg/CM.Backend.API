using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.Comment.Events;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class CommentRMEventHandler :
        IEventHandler<MessageEnvelope<CommentCreated>>,
        IEventHandler<MessageEnvelope<CommentEdited>>,
        IEventHandler<MessageEnvelope<CommentDeleted>>,
        IEventHandler<MessageEnvelope<UserInfoEdited>>,
        IEventHandler<MessageEnvelope<UserImageCustomizationEdited>>
    {
        private readonly ICommentRepository commentRepository;

        public CommentRMEventHandler(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public async Task HandleAsync(MessageEnvelope<CommentCreated> evt, CancellationToken ct)
        {
            await commentRepository.Insert(new Comment
            {
                Id = evt.Id,
                AuthorId = evt.Event.AuthorId.Value,
                AuthorName = evt.Event.AuthorName.Value,
                AuthorProfileImgId = evt.Event.AuthorProfileImgId.Value,
                Content = evt.Event.Content.Value,
                ContextId = evt.Event.ContextId.Value,
                ContextType = evt.Event.ContextType.Value,
                Date = DateTime.UtcNow
            });
        }

        public async Task HandleAsync(MessageEnvelope<CommentEdited> evt, CancellationToken ct)
        {
            await commentRepository.EditComment(evt.Event.Id, evt.Event.Content.Value);
        }

        public async Task HandleAsync(MessageEnvelope<CommentDeleted> evt, CancellationToken ct)
        {
            await commentRepository.Delete(evt.Id);
        }

        public async Task HandleAsync(MessageEnvelope<UserInfoEdited> evt, CancellationToken ct)
        {
            if (evt.Event.DidNameChange)
            {
                await commentRepository.UpdateAuthorNameBatchAsync(evt.Id, evt.Event.Name.Value);
            }
        }

        public async Task HandleAsync(MessageEnvelope<UserImageCustomizationEdited> evt, CancellationToken ct)
        {
            if (evt.Event.ProfileImageChanged)
            {
                await commentRepository.UpdateAuthorProfileImageIdBatchAsync(evt.Id, evt.Event.ProfileImageId.Value);
            }
        }
    }
}