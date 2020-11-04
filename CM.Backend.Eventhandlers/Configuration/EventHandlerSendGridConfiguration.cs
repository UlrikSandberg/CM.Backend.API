namespace CM.Backend.EventHandlers.Configuration
{
    public class EventHandlerSendGridConfiguration
    {
        public string API_Key { get; set; }
        public string ConfirmationTemplateId { get; set; }
        public string WelcomeEmailWithConfirmation { get; set; }
    }
}