using System;
namespace CM.Backend.Queries.Model
{
    public class ChampagneFolderQueryModel
    {
		public Guid Id { get; set; }

        public string FolderName { get; set; }

        public Guid AuthorId { get; set; }
	    
	    public string AuthorName { get; set; }

        public Guid DisplayImageId { get; set; }

        public Guid[] ChampagneIds { get; set; } = new Guid[0];

        public string FolderContentType { get; set; }
	    
	    public string FolderType { get; set; }

		public double AverageRating { get; set; }

		public double SumOfRating { get; set; }

		public double NumberOfTasting { get; set; }
    }
}
