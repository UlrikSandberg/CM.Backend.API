using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Aggregates.Tasting.Commands;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting
{
	public class Tasting : Aggregate
    {
	    //Tasting content
        public string Review { get; private set; }
        public Rating Rating { get; private set; }

        public AggregateId AuthorId { get; private set; }
        public AggregateId ChampagneId { get; private set; }
        public AggregateId BrandId { get; private set; }

        //A tasting would further have some knowlegde of the people whom have liked the review and further more information about who commented on this post
	    public DateTime TimeStamp { get; private set; }
	    
	    public bool IsPublic { get; private set; }
	    public bool IsDeleted { get; private set; }
	    
	    public MigrationSource MigrationSource { get; private set; }

        //Furthermore additional tastings should be a possiblity within a tasting, even different tastings of different sisiz of bottles
        //public HashSet<Guid> NestedTastings { get; set; }

	    public void Execute(CreateTasting cmd)
	    {
		    if (AuthorId != null  || ChampagneId != null || BrandId != null || IsDeleted)
		    {
			    throw new DomainException("Can't change identification information");
		    }

		    if (string.IsNullOrEmpty(cmd.Review))
		    {
			    RaiseEvent(new TastingCreated(cmd.Id.Value, null, cmd.Rating, cmd.AuthorId, cmd.ChampagneId, cmd.BrandId, cmd.IsDeleted, cmd.TimeStamp, cmd.IsPublic));
		    }
		    else
		    {
			    RaiseEvent(new TastingCreated(cmd.Id.Value, cmd.Review, cmd.Rating, cmd.AuthorId, cmd.ChampagneId, cmd.BrandId, cmd.IsDeleted, cmd.TimeStamp, cmd.IsPublic));
		    }
	    }

	    public void Execute(EditTasting cmd)
	    {
		    if (IsDeleted)
		    {
			    throw new DomainException("Can't change deleted item");
		    }
		    
		    RaiseEvent(new TastingEdited(Id, ChampagneId, cmd.Review, cmd.Rating));
	    }

	    public void Execute(DeleteTasting cmd)
	    {
		    if (IsDeleted)
		    {
			    throw new DomainException("Can't change deleted item");
		    }
		    
		    RaiseEvent(new TastingDeleted(Id, AuthorId, ChampagneId, cmd.IsDeleted));
	    }

	    public void Execute(SetMigrationSource cmd)
	    {
		    RaiseEvent(new MigrationSourceSet(Id, cmd.MigrationSource));
	    }
	    
	    
	    protected override void RegisterHandlers()
	    {
		    Handle<TastingCreated>(evt =>
		    {
			    Id = evt.Id;
			    Review = evt.Review;
			    Rating = evt.Rating;
			    AuthorId = evt.AuthorId;
			    ChampagneId = evt.ChampagneId;
			    BrandId = evt.BrandId;
			    IsDeleted = evt.IsDeleted;
			    TimeStamp = evt.TimeStamp;
			    IsPublic = evt.IsPublic;
		    });
		    
		    Handle<TastingEdited>(evt =>
		    {
			    Review = evt.Review;
			    Rating = evt.Rating;
		    });
		    
		    Handle<TastingDeleted>(evt =>
		    {
			    IsDeleted = evt.IsDeleted;
		    });
		    
		    Handle<MigrationSourceSet>(evt => { MigrationSource = evt.MigrationSource; });
	    }
	}
}
