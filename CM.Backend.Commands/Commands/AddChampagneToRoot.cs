using System;
namespace CM.Backend.Commands.Commands
{
	public class AddChampagneToRoot : Command
    {
		public Guid BrandId { get; private set; }
		public Guid ChampagneRootId { get; private set; }
		public Guid ChampagneId { get; private set; }

		public AddChampagneToRoot(Guid brandId, Guid champagneRootId, Guid champagneId)
        {
            ChampagneId = champagneId;
			ChampagneRootId = champagneRootId;
			BrandId = brandId;
		}
    }
}
