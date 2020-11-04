using System;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class UpdateBrandInfo
    {
		public BrandName Name { get; private set; }
        public string ProfileText { get; set; }

        public UpdateBrandInfo(BrandName name, string profileText)
        {
	        if (name == null)
	        {
		        throw new ArgumentException(nameof(name) + ": UpdateBrandInfo(BrandName name,...) --> BrandName is null which is not allowed in the given context");
	        }
	        
            ProfileText = profileText;
			Name = name;
		}
    }
}
