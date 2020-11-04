using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Entities
{
    public class Cellar
    {

		public string Title { get; private set; }
		public ImageId CardImageId { get; set; }
		public ImageId CoverImageId { get; set; }
		public List<CellarSection> Sections { get; private set; }
        
		public Cellar(string title, ImageId cardImageId, ImageId coverImageId, List<CellarSection> sections)
        {
	        if (sections == null)
	        {
		        throw new ArgumentException(nameof(sections) + ": List<CellarSection> cannot be null");
	        }
	        
			Title = title;
			CardImageId = cardImageId;
			CoverImageId = coverImageId;
			Sections = sections;
        }
    }
}
