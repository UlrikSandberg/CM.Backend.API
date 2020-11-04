using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BugAndFeedback.ValueObjects
{
    public class BugAndFeedbackType : SingleValueObject<string>
    {
        private const string Bug = "Bug";
        private const string Feedback = "Feedback";
        
        public BugAndFeedbackType(string value) : base(value)
        {
            if (!string.Equals(value, Bug) && !string.Equals(value, Feedback))
            {
                throw new ArgumentException(nameof(BugAndFeedbackType) + ": Incompatible BugAndFeedbackType");
            }
        }
    }
}