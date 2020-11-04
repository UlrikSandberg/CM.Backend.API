using System;
namespace CM.Backend.Commands.Commands
{
	public class EditChampagne : CommandWithIdResponse
    {
		public string BottleName { get; private set; }
		public Guid BrandId { get; private set; }
		public Guid BottleImgId { get; private set; }
		public bool IsVintage { get; private set; }
		public int? VintageYear { get; private set; }
		public Guid ChampagneId { get; private set; }

		public EditChampagne(string bottleName, Guid brandId, Guid champagneId, Guid bottleImgId, bool isVintage, int? vintageYear)
        {
            ChampagneId = champagneId;
			VintageYear = vintageYear;
			IsVintage = isVintage;
			BottleImgId = bottleImgId;
			BrandId = brandId;
			BottleName = bottleName;
		}
    }
}
