using System;

namespace CM.Backend.Commands.Commands
{
    public class CreateChampagne : CommandWithIdResponse
    {
		public Guid BrandId { get; private set; }
		public bool IsVintage { get; private set; }
		public int? VintageYear { get; private set; }
	    public string CreateChampagneOption { get; private set; }
	    public Guid ChampagneFolderId { get; private set; }
	    public bool IsOnDiscover { get; }
	    public string BottleName { get; private set; }
		public Guid BottleImgId { get; private set; }

		public CreateChampagne(string bottleName, Guid brandId, Guid bottleImgId, bool isVintage, int? vintageYear, string createChampagneOption, Guid champagneFolderId, bool isOnDiscover)
		{
			BottleName = bottleName;
            BrandId = brandId;
            IsVintage = isVintage;
            VintageYear = vintageYear;
			CreateChampagneOption = createChampagneOption;
			ChampagneFolderId = champagneFolderId;
			IsOnDiscover = isOnDiscover;
			BottleImgId = bottleImgId;
        }
    }
}
