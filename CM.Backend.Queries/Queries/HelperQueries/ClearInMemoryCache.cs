using System.Collections.Generic;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.HelperQueries
{
    public class ClearInMemoryCache : Query<bool>
    {
        public List<string> Cmds { get; }

        public ClearInMemoryCache(List<string> cmds)
        {
            Cmds = cmds;
        }
    }
}