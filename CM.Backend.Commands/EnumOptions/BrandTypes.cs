using System;

namespace CM.Backend.Commands.EnumOptions
{
	public interface IBrandTypes
	{
		Domain.EnumOptions.BrandTypes.TypeOfBrandImage TypeOfBrandImageConversion(BrandTypes.TypeOfBrandImage typeOfBrandImage);
	}

	public class BrandTypes : IBrandTypes
    {
		public enum TypeOfBrandImage {
			Champagne,
            Cover,
            Card
		}

		public BrandTypes()
		{
			
		}

		public Domain.EnumOptions.BrandTypes.TypeOfBrandImage TypeOfBrandImageConversion(TypeOfBrandImage typeOfBrandImage)
		{
			return (Domain.EnumOptions.BrandTypes.TypeOfBrandImage)Enum.Parse(typeof(Domain.EnumOptions.BrandTypes.TypeOfBrandImage), typeOfBrandImage.ToString());
		}
	}
}
