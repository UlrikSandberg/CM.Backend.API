using System;
using CM.Backend.Domain.Aggregates.Comment.Commands;
using CM.Backend.Domain.Aggregates.Comment.Events;
using CM.Backend.Domain.Aggregates.Comment.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment
{
	public class Comment : Aggregate
    {
	    public AggregateId ContextId { get; set; }
	    
	    public CommentContextType ContextType { get; set; } 
	    
	    public AggregateId AuthorId { get; set; }

		public DateTime Date { get; set; } 

		public CommentContent Content { get; set; }
	    
	    public bool IsDeleted { get; set; }
		
        public Comment()
        {
        }

	    public void Execute(CreateComment cmd)
	    {
		    if (!Id.Equals(Guid.Empty))
		    {
			    throw new DomainException("Can't invoke createCmd on existing object");
		    }
		    
		    RaiseEvent(new CommentCreated(
			    cmd.Id.Value,
			    cmd.ContextId,
			    cmd.ContextType,
			    cmd.AuthorId,
			    cmd.AuthorName,
			    cmd.AuthorProfileImgId,
			    cmd.Date,
			    cmd.Content,
			    cmd.IsDeleted));
		    
	    }

	    public void Execute(EditComment cmd)
	    {
		    if (IsDeleted)
		    {
			    throw new DomainException("Can't edit deleted comment");
		    }
		    
		    RaiseEvent(new CommentEdited(Id, cmd.Content));
	    }

	    public void Execute(DeleteComment cmd)
	    {
		    if (IsDeleted)
		    {
			    throw new DomainException("Can't edit deleted comment");
		    }
		    
		    RaiseEvent(new CommentDeleted(Id, cmd.IsDeleted));
	    }

	    protected override void RegisterHandlers()
	    {
		   Handle<CommentCreated>(evt =>
		   {
			   Id = evt.Id;
			   ContextId = evt.ContextId;
			   ContextType = evt.ContextType;
			   AuthorId = evt.AuthorId;
			   Date = evt.Date;
			   Content = evt.Content;
			   IsDeleted = evt.IsDeleted;
		   });
		    
		   Handle<CommentEdited>(evt => { Content = evt.Content; });
		   
		   Handle<CommentDeleted>(evt => { IsDeleted = evt.IsDeleted; });
		    
	    }
	}
}
