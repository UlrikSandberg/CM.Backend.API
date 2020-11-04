using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AutoMapper;
using CM.Backend.Documents.StaticResources;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.ChampagneModels;
using CM.Backend.Queries.Model.UserModels;

namespace CM.Backend.Queries.Helpers
{
    public static class MappingResources
    {
        public static double CalculateAverageRating(Dictionary<string, double> ratings)
        {
            var sum = 0.0;
		    
            foreach(var rating in ratings.Values)
            {
                sum += rating;
            }

            return sum / ratings.Count;
        }

        public static double CalculateRatingSum(Dictionary<string, double> ratings)
        {
            var sum = 0.0;

            foreach (var rating in ratings.Values)
            {
                sum += rating;
            }

            return sum;
        }

        
        /// <summary>
        /// Maps a persistence champagne model to a champagneLight model.
        /// </summary>
        /// <param name="champagne"></param>
        /// <param name="brandName"></param>
        /// <returns></returns>
        public static ChampagneLight MapChampagneToChampagneLight(Champagne champagne, string brandName)
        {
            var result = GenericMapper<Champagne, ChampagneLight>.Map(champagne);

            result.BrandName = brandName;
            result.BottleName = champagne.BottleName;
            result.ChampagneRootId = champagne.Id;
            result.GetVintageInfo = new ChampagneLight.VintageInfo {IsVintage = champagne.vintageInfo.IsVintage, Year = champagne.vintageInfo.Year};
            result.NumberOfTastings = champagne.RatingDictionary.Count;
            result.RatingSumOfTastings = CalculateRatingSum(champagne.RatingDictionary);

            return result;
        }

        public static BrandLight MapBrandToBrandLight(Brand brand, int brandFollowers, int tastingsForBrand,
            int editionFoldersForBrand)
        {
            var result = GenericMapper<Brand, BrandLight>.Map(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<Brand, BrandLight>()
                            .ForMember(x => x.BottleCoverImageId, opt => opt.MapFrom(src => src.BottleCoverImgId))
                            .ForMember(x => x.BrandCoverImageId, opt => opt.MapFrom(src => src.BrandCoverImgId))
                            .ForMember(x => x.BrandListImageId, opt => opt.MapFrom(src => src.BrandListImgId))
                            .ForMember(x => x.BrandLogoImageId, opt => opt.MapFrom(src => src.LogoImgId));
                    }), brand);

            result.NumberOfTastings = tastingsForBrand;
            result.NumberOfFollowers = brandFollowers;
            result.NumberOfChampagnes = editionFoldersForBrand;

            return result;
        }

        public static ChampagneSearchModel MapChampagneToChampagneSearchModel(Champagne champagne, string brandName)
        {
            var result = GenericMapper<Champagne, ChampagneSearchModel>.Map(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<Champagne, ChampagneSearchModel>()
                            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.BottleName))
                            .ForMember(x => x.ImageId, opt => opt.MapFrom(src => src.BottleImgId))
                            .ForMember(x => x.BrandName, opt => opt.UseValue(brandName))
                            .ForMember(x => x.IsVintage, opt => opt.MapFrom(src => src.vintageInfo.IsVintage))
                            .ForMember(x => x.Year, opt => opt.MapFrom(src => src.vintageInfo.Year))
                            .ForMember(x => x.NumberOfTastings, opt => opt.MapFrom(src => src.RatingDictionary.Count));
                    }), champagne);

            return result;
        }

        public static ChampagneSearchModel MapChampagneSearchProjectionModelToChampagneSearchModel(
            ChampagneSearchProjectionModel champagneSearchProjectionModel, string brandName)
        {
            return GenericMapper<ChampagneSearchProjectionModel, ChampagneSearchModel>.Map(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<ChampagneSearchProjectionModel, ChampagneSearchModel>()
                            .ForMember(x => x.NumberOfTastings, opt => opt.MapFrom(src => src.RatingDictionary.Count))
                            .ForMember(x => x.BrandName, opt => opt.UseValue(brandName));
                    }), champagneSearchProjectionModel);
        }

        public static UserSearchModel MapUserSearchProjectionModelToUserSearchModel(UserSearchProjectionModel pM)
        {
            return GenericMapper<UserSearchProjectionModel, UserSearchModel>.Map(pM);
        }
        
        public static string RemoveDiacritics(string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
 
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
 
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}