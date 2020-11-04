using System;
namespace CM.Backend.API.EnumOptions
{

	public interface IBrandTypes
	{
		Commands.EnumOptions.BrandTypes.TypeOfBrandImage TypeOfBrandImageConversion(BrandTypes.TypeOfBrandImage typeOfBrandImage);
	}

	public class BrandTypes : IBrandTypes
    {

		public BrandTypes()
		{
			
		}

		public enum TypeOfBrandImage {
			Champagne,
            Cover,
            Card,
            Logo
		}

		public Commands.EnumOptions.BrandTypes.TypeOfBrandImage TypeOfBrandImageConversion(TypeOfBrandImage typeOfBrandImage)
		{
			return (Commands.EnumOptions.BrandTypes.TypeOfBrandImage)Enum.Parse(typeof(Commands.EnumOptions.BrandTypes.TypeOfBrandImage), typeOfBrandImage.ToString());
		}
    }
}
