using System;
using System.Collections.Generic;

namespace CM.Backend.API.RequestModels
{
    public class GetChampagnesFromListRequestModel
    {
		public string Name { get; set; }
		public List<Guid> ChampagneIds { get; set; }

    }
}
