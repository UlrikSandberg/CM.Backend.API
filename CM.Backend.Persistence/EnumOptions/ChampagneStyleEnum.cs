using System.Collections.Generic;

namespace CM.Backend.Persistence.EnumOptions
{
    public class PersistanceChampagneStyleEnum
    {

        public enum ChampagneStyleEnum
        {
            Rose,
            BlancDeBlanc,
            BlancDeNoir,
            OnIce,
            TradBrut,
            TradSweet
        }

        public static List<ChampagneStyleEnum> GetFullChampagneStyleEnumList()
        {
            return new List<ChampagneStyleEnum> { ChampagneStyleEnum.Rose, ChampagneStyleEnum.BlancDeBlanc, ChampagneStyleEnum.BlancDeNoir, ChampagneStyleEnum.OnIce, ChampagneStyleEnum.TradBrut, ChampagneStyleEnum.TradSweet};
        }
    }
}