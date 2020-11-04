using System;
using System.Collections.Generic;

namespace CM.Backend.Commands.Commands
{
	public class EditChampagneFilterSearchOptions : CommandWithIdResponse
    {
		public string Dosage { get; private set; }
		public List<string> Styles { get; private set; }
		public List<string> Characters { get; private set; }
		public Guid ChampagneId { get; private set; }
		public Guid BrandId { get; private set; }

		public EditChampagneFilterSearchOptions(Guid champagneId, Guid brandId, string dosage, List<string> styles, List<string> characters)
        {
            BrandId = brandId;
			ChampagneId = champagneId;
			Characters = characters;
			Styles = styles;
			Dosage = dosage;
		}
    }
}
