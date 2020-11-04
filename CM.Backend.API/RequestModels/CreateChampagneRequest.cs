using System;
namespace CM.Backend.API.RequestModels
{
    public class CreateChampagneRequest
    {
        
		public string BottleName { get; set; }
		public Guid BottleImgId { get; set; }
		public bool Vintage { get; set; }
		public int? Year { get; set; }

      
    }
}
