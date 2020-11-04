using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain
{
    public static class DomainExtensions
    {
        public static IEnumerable<Guid> ConvertToGuidList(this OrderedSet<AggregateId> list)
        {
            return list.Select(x => x.Value);
        }
        
        public static IEnumerable<Guid> ConvertToGuidList(this List<AggregateId> list)
        {
            return list.Select(x => x.Value);
        }

        public static IEnumerable<Guid> ConverToGuidList(this HashSet<AggregateId> list)
        {
            return list.Select(x => x.Value);
        }

        public static IEnumerable<AggregateId> ConverToAggregateIds(this List<Guid> list)
        {
            if (list == null)
            {
                return new List<AggregateId>();
            }
            return list.Select(x => new AggregateId(x));
        }

        public static IEnumerable<ImageId> ConvertToImageIds(this List<Guid> list)
        {
            if (list == null)
            {
                return new List<ImageId>();
            }

            return list.Select(x => new ImageId(x));
        }

        public static HashSet<AggregateId> GetAggregateIdHashSet()
        {
            return new HashSet<AggregateId>(new AggregateIdEqualityComparer());
        }
    }
}

public class AggregateIdEqualityComparer : IEqualityComparer<AggregateId>
{                                                                        
    public bool Equals(AggregateId x, AggregateId y)                     
    {                                                                    
        return x.Value.Equals(y.Value);                                  
    }                                                                    
                                                                         
    public int GetHashCode(AggregateId obj)                              
    {                                                                    
        return obj.GetHashCode();                                        
    }                                                                    
}                                                                        