using System;
using Marten.Events.Projections;

namespace CM.Backend.API.EnumOptions
{

    public class ChampagneStyleEnum
    {
        private const string Rose = "Rose";
        private const string BlancDeBlanc = "BlancDeBlanc";
        private const string BlancDeNoir = "BlancDeNoir";
        private const string OnIce = "OnIce";
        private const string TradBrut = "TradBrut";
        private const string TradSweet = "TradSweet";

        public enum Style
        {
            Rose,
            BlancDeBlanc,
            BlancDeNoir,
            OnIce,
            TradBrut,
            TradSweet,
            Unknown
        }

        public static string StyleEnumConvert(ChampagneStyleEnum.Style style)
        {
            switch (style)
            {
                    
                    case Style.Rose:
                        return Rose;
                    
                    case Style.BlancDeBlanc:
                        return BlancDeBlanc;
                    
                    case Style.BlancDeNoir:
                        return BlancDeNoir;
                    
                    case Style.OnIce:
                        return OnIce;
                    
                    case Style.TradBrut:
                        return TradBrut;
                    
                    case Style.TradSweet:
                        return TradSweet;    
            }

            return null;
        }

        public static Style ConvertStringToStyleEnum(string style)
        {
            if (style.Equals(Rose))
            {
                return Style.Rose;
            }

            if (style.Equals(BlancDeBlanc))
            {
                return Style.BlancDeBlanc;
            }

            if (style.Equals(BlancDeNoir))
            {
                return Style.BlancDeNoir;
            }

            if (style.Equals(OnIce))
            {
                return Style.OnIce;
            }

            if (style.Equals(TradBrut))
            {
                return Style.TradBrut;
            }

            if (style.Equals(TradSweet))
            {
                return Style.TradSweet;
            }

            return Style.Unknown;
        }

        public static Persistence.EnumOptions.PersistanceChampagneStyleEnum.ChampagneStyleEnum ConvertToPersistenceEnum(
            ChampagneStyleEnum.Style champagneStyle)
        {
            return (Persistence.EnumOptions.PersistanceChampagneStyleEnum.ChampagneStyleEnum) Enum.Parse(
                typeof(Persistence.EnumOptions.PersistanceChampagneStyleEnum.ChampagneStyleEnum),
                champagneStyle.ToString());
        }
    }
}