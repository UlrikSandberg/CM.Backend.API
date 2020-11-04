using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;

namespace CM.Backend.Queries.Model
{
	public class Cellar
	{

		public string Title { get; set; }
		public Guid CardImgId { get; set; }
		public Guid HeaderImgId { get; set; }
		public string Url { get; set; }
		public List<Section> Sections { get; set; }

		public class Section
		{
			public string Title { get; set; }
			public string Body { get; set; }
			public List<ChampagneFolderQueryModel> Champagnes { get; set; }
		}

    }
}
