using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetEmailConfirmationProcess : Query<EmailConfirmationProcess>
    {
        public string Token { get; private set; }

        public GetEmailConfirmationProcess(string token)
        {
            Token = token;
        }
    }
}