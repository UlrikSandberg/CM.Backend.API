using System;
using System.Collections.Generic;

namespace CM.Backend.Queries.Model
{
	public class BrandPageLight
	{
		public Guid BrandPageId { get; set; }

		public string Title { get; set; }

		public Guid CardImgId { get; set; }

		public string Url { get; set; }

		public IList<string> Labels { get; set; }
	}
}
