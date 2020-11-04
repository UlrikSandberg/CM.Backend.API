using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Queries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class EmailConfirmationQueryHandler :
        IQueryHandler<GetEmailConfirmationProcess, EmailConfirmationProcess>
    {
        private readonly IEmailConfirmationRepository _emailConfirmationRepository;

        public EmailConfirmationQueryHandler(IEmailConfirmationRepository emailConfirmationRepository)
        {
            _emailConfirmationRepository = emailConfirmationRepository;
        }

        public async Task<EmailConfirmationProcess> HandleAsync(GetEmailConfirmationProcess query, CancellationToken ct)
        {
            return await _emailConfirmationRepository.GetByToken(Hash_sha256(query.Token));
        }
        
        public static string Hash_sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}