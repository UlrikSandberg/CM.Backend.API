using System.Collections.Generic;

namespace CM.Backend.Domain
{
    public class OrderedSet<T> : List<T> where T : IEqualityComparer<T>
    {
        public new void Add(T item)
        {
            if (!Contains(item))
            {
                base.Add(item);
            }
        }
    }
}