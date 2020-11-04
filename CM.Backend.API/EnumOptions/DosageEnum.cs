using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CM.Backend.API.EnumOptions
{
    public class DosageEnum
    {
        public const string BrutNature = "BrutNature";
        public const string ExtraBrut = "ExtraBrut";
        public const string Brut = "Brut";
        public const string ExtraDry = "ExtraDry";
        public const string Sec = "Sec";
        public const string DemiSec = "DemiSec";
        public const string Doux = "Doux";
        
        public enum Dosage
        {
            BrutNature,
            ExtraBrut,
            Brut,
            ExtraDry,
            Sec,
            DemiSec,
            Doux,
            Unknown
        }

        public static string DosageEnumConverter(DosageEnum.Dosage dosage)
        {
            switch (dosage)
            {
                case Dosage.BrutNature:
                    return BrutNature;
                    
                case Dosage.ExtraBrut:
                    return ExtraBrut;
                    
                case Dosage.Brut:
                    return Brut;
                
                case Dosage.ExtraDry:
                    return ExtraDry;
                
                case Dosage.Sec:
                    return Sec;
                
                case Dosage.DemiSec:
                    return DemiSec;
                
                case Dosage.Doux:
                    return Doux;
                
            }

            return null;
        }

        public static Dosage ConvertStringToDosageEnum(string dosage)
        {
            if (dosage.Equals(BrutNature))
            {
                return Dosage.BrutNature;
            }
            if (dosage.Equals(ExtraBrut))
            {
                return Dosage.ExtraBrut;
            }
            if (dosage.Equals(Brut))
            {
                return Dosage.Brut;
            }
            if (dosage.Equals(ExtraDry))
            {
                return Dosage.ExtraDry;
            }
            if (dosage.Equals(Sec))
            {
                return Dosage.Sec;
            }
            if (dosage.Equals(DemiSec))
            {
                return Dosage.DemiSec;
            }
            if (dosage.Equals(Doux))
            {
                return Dosage.Doux;
            }

            return Dosage.Unknown;

        }

        public static Persistence.EnumOptions.PersistenceChampagneDosageEnum.ChampagneDosageEnum
            ConvertToPersistenceDosageEnum(DosageEnum.Dosage dosage)
        {
            return (Persistence.EnumOptions.PersistenceChampagneDosageEnum.ChampagneDosageEnum) Enum.Parse(
                typeof(Persistence.EnumOptions.PersistenceChampagneDosageEnum.ChampagneDosageEnum), dosage.ToString());
        }
        
    }
}