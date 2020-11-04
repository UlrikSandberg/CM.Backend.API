using System;

namespace CM.Backend.Persistence.Model
{
    public class BrandSearchProjectionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}