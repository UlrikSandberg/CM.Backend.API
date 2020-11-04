using System;
using CM.Backend.Queries.Model.CommentModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetComment : Query<CommentModel>
    {
        public Guid CommentId { get; private set; }
        public Guid RequesterId { get; private set; }

        public GetComment(Guid commentId, Guid requesterId)
        {
            CommentId = commentId;
            RequesterId = requesterId;
        }
        
    }
}