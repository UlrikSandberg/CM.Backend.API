using System;
using System.Collections.Generic;

namespace CM.Backend.Queries.Model
{
    public class CellarLight
    {

		public string Title { get; set; }
		public Guid CardImgId { get; set; }
		public string URL { get; set; }
		public List<string> Labels { get; set; }

    }
}
