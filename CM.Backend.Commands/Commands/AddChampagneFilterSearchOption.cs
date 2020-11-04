using System;
using System.Collections.Generic;

namespace CM.Backend.Commands.Commands
{
	public class AddChampagneFilterSearchOption : CommandWithIdResponse
    {
        
		public Guid BrandId { get; set; }
		public Guid ChampagneId { get; set; }
		public string Dosage { get; set; }
		public List<string> Styles { get; set; }
		public List<string> Characters { get; set; }

		public AddChampagneFilterSearchOption(Guid brandId, Guid champagneId, string dosage, List<string> styles, List<string> characters)
        {
			BrandId = brandId;
			ChampagneId = champagneId;
			Dosage = dosage;
			Styles = styles;
			Characters = characters;
        }
    }
}
