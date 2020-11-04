using Newtonsoft.Json;

namespace CM.Backend.API.Middleware.GlobalExceptionFilter
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}