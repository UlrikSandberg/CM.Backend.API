using System;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class UpdateBrandInfo : Command
    {
        public Guid BrandId { get; private set; }
        public string Name { get; private set; }
        public string ProfileText { get; private set; }

        public UpdateBrandInfo(Guid brandId, string name, string profileText)
        {
            BrandId = brandId;
            Name = name;
            ProfileText = profileText;
        }
    }
}