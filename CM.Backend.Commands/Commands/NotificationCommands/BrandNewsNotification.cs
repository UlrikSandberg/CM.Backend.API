using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class BrandNewsNotification : Command
    {
        public Guid BrandId { get; private set; }
        public string BrandName { get; private set; }
        public Guid BrandLogoImageId { get; private set; }
        public string Message { get; private set; }
        public string ContextUrl { get; private set; }
        public Guid ChampagneId { get; private set; }

        public BrandNewsNotification(Guid brandId, string brandName, Guid brandLogoImageId, string message, string contextUrl, Guid champagneId)
        {
            BrandId = brandId;
            BrandName = brandName;
            BrandLogoImageId = brandLogoImageId;
            Message = message;
            ContextUrl = contextUrl;
            ChampagneId = champagneId;
        }
    }
}