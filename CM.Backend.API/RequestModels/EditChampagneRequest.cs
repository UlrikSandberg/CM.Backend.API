using System;
namespace CM.Backend.API.RequestModels
{
    public class EditChampagneRequest
    {
        
		public string BottleName { get; set; }
		public Guid BottleImgId { get; set; }
		public bool IsVintage { get; set; }
		public int VintageYear { get; set; }

    }
}
