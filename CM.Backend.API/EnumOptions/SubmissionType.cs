using Microsoft.AspNetCore.Diagnostics;

namespace CM.Backend.API.EnumOptions
{
    public class SubmissionTypeEnum
    {
        public const string Bug = "Bug";
        public const string Feedback = "Feedback";
        
        public enum SubmissionType
        {
            Bug,
            Feedback,
            Unknown
        }

        public static SubmissionType ConvertStringToSubmissionType(string type)
        {
            if (type.Equals(Bug))
            {
                return SubmissionType.Bug;
            }

            if (type.Equals(Feedback))
            {
                return SubmissionType.Feedback;
            }

            return SubmissionType.Unknown;
        }

        public static string SubmissionTypeToString(SubmissionType type)
        {
            switch (type)
            {
                    case SubmissionType.Bug:
                        return Bug;
                    case SubmissionType.Feedback:
                        return Feedback;
                    
                    
            }

            return null;
        }
    }
}