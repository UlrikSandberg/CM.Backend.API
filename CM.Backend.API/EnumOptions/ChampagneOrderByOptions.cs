using System;
using CM.Backend.Persistence.EnumOptions;

namespace CM.Backend.API.EnumOptions
{
    public interface ITastingOrderByOptions
    {
        Persistence.EnumOptions.TastingOrderByOption.OrderBy TastingOrderByPersistenceConversion(
            TastingOrderByOptions.OrderByOptions orderByOptions);
    }
    
    public class TastingOrderByOptions
    {
        public enum OrderByOptions
        {
            AcendingByRating,
            DecendingByRating,
            AcendingByDate,
            DecendingByDate,
            
        }

        public static TastingOrderByOption.OrderBy TastingOrderByPersistenceConversion(OrderByOptions orderByOptions)
        {
            return (Persistence.EnumOptions.TastingOrderByOption.OrderBy)Enum.Parse(
                typeof(Persistence.EnumOptions.TastingOrderByOption.OrderBy), orderByOptions.ToString());

        }
    }
}