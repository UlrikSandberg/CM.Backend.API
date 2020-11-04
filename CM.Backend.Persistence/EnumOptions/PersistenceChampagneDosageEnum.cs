using System.Collections.Generic;

namespace CM.Backend.Persistence.EnumOptions
{
    public class PersistenceChampagneDosageEnum
    {
        public enum ChampagneDosageEnum
        {
            BrutNature,
            ExtraBrut,
            Brut,
            ExtraDry,
            Sec,
            DemiSec,
            Doux
        }


        public static List<ChampagneDosageEnum> GetFullChampagneDosageEnumList()
        {
            return new List<ChampagneDosageEnum> { ChampagneDosageEnum.BrutNature, ChampagneDosageEnum.ExtraBrut, ChampagneDosageEnum.Brut, ChampagneDosageEnum.ExtraDry, ChampagneDosageEnum.Sec, ChampagneDosageEnum.DemiSec, ChampagneDosageEnum.Doux};
        }
    }
}