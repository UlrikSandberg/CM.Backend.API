using System;
using System.Collections.Generic;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Queries.Model;

namespace CM.Backend.Queries.Builders
{
    public class FilterSearchQuery
    {
        public VintageInfo IsVintage { get; }
        public List<string> ChampagneStyle { get; }
        public List<string> ChampagneDosage { get; }
        public double LowerRating { get; }
        public double UpperRating { get; }
        
        
        public class VintageInfo
        {
            public bool Vintage { get; set; }
            public bool NonVintage { get; set; }
        }
        
        private FilterSearchQuery(VintageInfo isVintage, IEnumerable<PersistanceChampagneStyleEnum.ChampagneStyleEnum> champagneStyle, IEnumerable<PersistenceChampagneDosageEnum.ChampagneDosageEnum> champagneDosage, double lowerRating, double upperRating)
        {
            IsVintage = isVintage;
            ChampagneStyle = new List<string>();
            foreach (var champagneStyleEnum in champagneStyle)
            {
                ChampagneStyle.Add(champagneStyleEnum.ToString());
            }
            ChampagneDosage = new List<string>();
            foreach (var champagneDosageEnum in champagneDosage)
            {
                ChampagneDosage.Add(champagneDosageEnum.ToString());
            }
            LowerRating = lowerRating;
            UpperRating = upperRating;
        }

        public class FilterSearchQueryBuilder : IBuilder<FilterSearchQuery>
        {

            private VintageInfo IsVintage = null;

            private List<PersistanceChampagneStyleEnum.ChampagneStyleEnum> ChampagneStyle = null;

            private List<PersistenceChampagneDosageEnum.ChampagneDosageEnum> ChampagneDosage = null;

            private double LowerRating = 0.0;

            private double UpperRating = 5.0;
            
            public FilterSearchQueryBuilder SetIsVintage(bool isVintage)
            {
                if(IsVintage == null)
                {
                    IsVintage = new VintageInfo();
                }
                IsVintage.Vintage = isVintage;
                return this;
            }

            public FilterSearchQueryBuilder SetIsNonVintage(bool isNonVintage)
            {
                if(IsVintage == null)
                {
                    IsVintage = new VintageInfo();
                }
                IsVintage.NonVintage = isNonVintage;
                return this;
            }

            public FilterSearchQueryBuilder SetChampagneStyle(List<PersistanceChampagneStyleEnum.ChampagneStyleEnum> champagneStyle)
            {            
                ChampagneStyle = champagneStyle;
                return this;
            }

            public FilterSearchQueryBuilder SetChampagneStyle(PersistanceChampagneStyleEnum.ChampagneStyleEnum champagneStyle)
            {
                if(ChampagneStyle == null)
                {
                    ChampagneStyle = new List<PersistanceChampagneStyleEnum.ChampagneStyleEnum>();
                    ChampagneStyle.Add(champagneStyle);
                }
                else
                {
                    ChampagneStyle.Add(champagneStyle);
                }

                return this;
            }

            public FilterSearchQueryBuilder SetChampagneDosage(List<PersistenceChampagneDosageEnum.ChampagneDosageEnum> champagneDosage)
            {
                ChampagneDosage = champagneDosage;
                return this;
            }

            public FilterSearchQueryBuilder SetChampagneDosage(PersistenceChampagneDosageEnum.ChampagneDosageEnum champagneDosage)
            {
                if(ChampagneDosage == null)
                {
                    ChampagneDosage = new List<PersistenceChampagneDosageEnum.ChampagneDosageEnum>();
                    ChampagneDosage.Add(champagneDosage);
                }
                else
                {
                    ChampagneDosage.Add(champagneDosage);
                }

                return this;
            }

            public FilterSearchQueryBuilder SetLowerRating(double lowerRating)
            {
                LowerRating = lowerRating;
                return this;
            }

            public FilterSearchQueryBuilder SetUpperRating(double upperRating)
            {
                UpperRating = upperRating;
                return this;
            }
            
            public FilterSearchQuery Build()
            {
                if (IsVintage == null)
                {
                    IsVintage = new VintageInfo
                    {
                        Vintage = false,
                        NonVintage = false
                    };
                }

                if (ChampagneStyle == null)
                {
                    ChampagneStyle = PersistanceChampagneStyleEnum.GetFullChampagneStyleEnumList();
                }

                if (ChampagneDosage == null)
                {
                    ChampagneDosage = PersistenceChampagneDosageEnum.GetFullChampagneDosageEnumList();
                }
                
                return new FilterSearchQuery(IsVintage, ChampagneStyle, ChampagneDosage, LowerRating, UpperRating);
            }
        }
    }
}