namespace CM.Instrumentation
{
    public class InstrumentationConfiguration
    {
        public string ElasticsearchUrl { get; set; } = "http://127.0.0.1:9200";
        public string Username { get; set; }
        public string Password { get; set; }
    }
}