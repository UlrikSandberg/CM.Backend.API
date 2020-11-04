namespace CM.Backend.API.RequestModels
{
    /// <summary>
    /// Configure the visibility of this brand in the app
    /// </summary>
    public class BrandPublishingRequest
    {
        public PublishingStatusEnum PublishingStatus { get; set; }
        
        public enum PublishingStatusEnum
        {
            Published,
            Unpublished
        }
    }
}