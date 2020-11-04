using System;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class VintageInfo
    {
        public bool IsVintage { get; private set; }
        public int? VintageYear { get; set; }

        public VintageInfo(bool isVintage, int? vintageYear)
        {
            if(isVintage && !vintageYear.HasValue)
                throw new ArgumentException("Vintage bottles must have a vintage year", nameof(vintageYear));

            IsVintage = isVintage;
            VintageYear = vintageYear;
        }
    }
}