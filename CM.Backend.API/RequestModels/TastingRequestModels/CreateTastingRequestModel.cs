using System;

namespace CM.Backend.API.RequestModels.TastingRequestModels
{
    public class CreateTastingRequestModel
    {
        public string Review { get; set; }
        public double Rating { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}