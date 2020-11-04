using System.Collections.Generic;
using CM.Backend.Persistence.Model;

namespace CM.Backend.Persistence.Helpers
{
    public class DistinctRootChampagneComparer : IEqualityComparer<Champagne>
    {
        public bool Equals(Champagne x, Champagne y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Champagne obj)
        {
            return obj.GetHashCode();
        }
    }
}