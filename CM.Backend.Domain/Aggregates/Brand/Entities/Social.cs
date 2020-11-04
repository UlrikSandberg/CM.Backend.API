using CM.Backend.Domain.Aggregates.Brand.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Entities
{
    public class Social
    {
        public FacebookUrl FacebookUrl { get; set; }
        public TwitterUrl TwitterUrl { get; set; } 
        public InstagramUrl InstagramUrl { get; set; }
        public PinterestUrl PinterestUrl { get; set; }
        public UrlValueObject WebsiteUrl { get; set; }
    }
}