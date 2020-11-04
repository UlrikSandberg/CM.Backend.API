using System.Collections.Generic;

namespace CM.Backend.Documents.GlobalLogging.Entities
{
    public class DbCommunicationInfo
    {
        public long TotalElapsedCommunicationTimeInMillis { get; set; }
        public int NumberOfDbCalls { get; set; }
        
        public List<ExecutionTimes> DbExecutionTimes { get; } = new List<ExecutionTimes>();

        public void AddExecutionTime(string name, long elapsedTime)
        {
            //The name is not null which means we either look for a entry with the same name or create a new.
            if (!string.IsNullOrEmpty(name))
            {
                //Fetch entry
                var entry = FetchNameNotNullExecutionTime(DbExecutionTimes, name);

                if (entry != null) //Found entry match on name, update with an additional call respective to this context
                {
                    entry.NumberOfCalls += 1;
                    entry.AverageElapsedTime += elapsedTime;
                }
                else //Couldn't find entry match on name. Make new entry
                {
                    DbExecutionTimes.Add(new ExecutionTimes
                    {
                        Name = name,
                        NumberOfCalls = 1,
                        AverageElapsedTime = elapsedTime
                    });
                }
            }
            else //Name is null find the entry with null name
            {
                var entry = DbExecutionTimes.Find(x => string.IsNullOrEmpty(x.Name));

                if (entry != null) //Entry with name = null exists
                {
                    entry.NumberOfCalls += 1;
                    entry.AverageElapsedTime += elapsedTime;
                }
                else //Entry with name = null does not exists create new entry with name null
                {
                    DbExecutionTimes.Add(new ExecutionTimes
                    {
                        Name = name,
                        NumberOfCalls = 1,
                        AverageElapsedTime = elapsedTime
                    });
                }
            }
            
            /*foreach (var executionTime in DbExecutionTimes)
            {
                if (!string.IsNullOrEmpty(executionTime.Name))
                {
                    if (executionTime.Name.Equals(name))
                    {
                        executionTime.NumberOfCalls += 1;
                        executionTime.AverageElapsedTime += elapsedTime;
                    }
                }
            }
            
            if (DbExecutionTimes.Exists(x => x.Name.Equals(name) || string.IsNullOrEmpty(x.Name)))
            {
                var entry = DbExecutionTimes.Find(x => x.Name.Equals(name) || string.IsNullOrEmpty(x.Name));

                entry.NumberOfCalls += 1;
                entry.AverageElapsedTime += elapsedTime;
            }
            else
            {
                DbExecutionTimes.Add(new ExecutionTimes
                {
                    Name = name,
                    NumberOfCalls = 1,
                    AverageElapsedTime = elapsedTime
                });
            }*/
        }
        
        public void TruncateDbExecutionTimesToAverage()
        {
            foreach (var executionTime in DbExecutionTimes)
            {
                executionTime.CalculateAverageElapsedTime();
            }
        }

        private ExecutionTimes FetchNameNotNullExecutionTime(List<ExecutionTimes> list, string name)
        {
            foreach (var entry in list)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    if (entry.Name.Equals(name))
                    {
                        return entry;
                    }
                }
            }

            return null;
        }
    }
}