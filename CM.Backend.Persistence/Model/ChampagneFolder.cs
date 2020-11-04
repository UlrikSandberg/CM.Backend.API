using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class ChampagneFolder : IEntity, ITextSearchSortable
    {
       
		[BsonId]
		public Guid Id { get; set; }

		public string FolderName { get; set; }

		public Guid AuthorId { get; set; }

		public Guid DisplayImageId { get; set; }

		public Guid[] ChampagneIds { get; set; } = new Guid[0];

		public string ContentType { get; set; }
	    
	    public string FolderType { get; set; }
	    
	    public bool IsOnDiscover { get; set; }

	    [BsonIgnoreIfNull]
	    public double? TextMatchScore { get; set; }

    }
}
