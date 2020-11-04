namespace CM.Backend.API.RequestModels.UserRequestModels
{
    public class FeedbackAndBugSubmissionRequestModel
    {
        public bool MayBeContacted { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public byte[] Image { get; set; }
    }
}