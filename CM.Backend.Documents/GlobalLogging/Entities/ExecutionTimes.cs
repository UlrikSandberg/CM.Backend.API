namespace CM.Backend.Documents.GlobalLogging.Entities
{
    public class ExecutionTimes
    {
        public string Name { get; set; }
        public int NumberOfCalls { get; set; }
        public long AverageElapsedTime { get; set; }

        public void CalculateAverageElapsedTime()
        {
            AverageElapsedTime = AverageElapsedTime / NumberOfCalls;
        }
    }
}