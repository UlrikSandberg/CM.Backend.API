using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Messaging.Contracts;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Infrastructure
{
    public class MediatorQueryRouter : IQueryRouter
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public MediatorQueryRouter(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default(CancellationToken))
            where TQuery : IQuery<TResult>
        {
            var result = await _mediator.FetchAsync<TQuery, TResult>(query, ct);
            _logger.Information("Executing {QueryName}: {@Query} got {@Result}", query.GetType().Name, query, result);

            return result;
        }
    }
}