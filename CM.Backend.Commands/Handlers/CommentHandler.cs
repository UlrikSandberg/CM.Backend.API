using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.CommentsCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Comment;
using CM.Backend.Domain.Aggregates.Comment.ValueObjects;
using CM.Backend.Domain.Aggregates.User;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;
using StructureMap.Diagnostics.TreeView;

namespace CM.Backend.Commands.Handlers
{
    public class CommentHandler : CommandHandlerBase,
        ICommandHandler<CreateComment, IdResponse>,
        ICommandHandler<EditComment, Response>,
        ICommandHandler<DeleteComment, Response>
    {
        public CommentHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<IdResponse> HandleAsync(CreateComment cmd, CancellationToken ct)
        {
            var comment = new Comment();

            var user = await AggregateRepo.LoadAsync<User>(cmd.AuthorId);

            comment.Execute(new Domain.Aggregates.Comment.Commands.CreateComment(
                new AggregateId(Guid.NewGuid()),
                new AggregateId(cmd.ContextId),
                new CommentContextType(cmd.ContextType),
                new AggregateId(user.Id),
                new CommentAuthorName(user.Name.Value),
                new ImageId(user.ImageCustomization.ProfilePictureImgId.Value),
                cmd.Date,
                new CommentContent(cmd.Content)));

            await AggregateRepo.StoreAsync(comment);
            
            return new IdResponse(comment.Id);

        }

        public async Task<Response> HandleAsync(EditComment cmd, CancellationToken ct)
        {
            var comment = await AggregateRepo.LoadAsync<Comment>(cmd.CommentId);

            if (!comment.AuthorId.Value.Equals(cmd.AuthorId))
            {
                Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@Comment} authorId did not match commands authorId", cmd, comment);
                return new Response(false, null, "Editing privileges is reserved for author only");
            }
            
            comment.Execute(new Domain.Aggregates.Comment.Commands.EditComment(new CommentContent(cmd.Content)));

            await AggregateRepo.StoreAsync(comment);
            
            return new Response(true);

        }

        public async Task<Response> HandleAsync(DeleteComment cmd, CancellationToken ct)
        {
            var comment = await AggregateRepo.LoadAsync<Comment>(cmd.CommentId);

            if (!comment.AuthorId.Value.Equals(cmd.AuthorId))
            {
                Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@Comment} authorId did not match commands authorId", cmd, comment);
                return new Response(false, null, "Editing privileges is reserved for author only");
            }
            
            comment.Execute(new Domain.Aggregates.Comment.Commands.DeleteComment(true));

            await AggregateRepo.StoreAsync(comment);
            
            return new Response(true);
        }
    }
}