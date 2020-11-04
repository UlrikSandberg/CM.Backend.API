using System.Threading;
using System.Threading.Tasks;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Contracts
{
    public interface IQueryRouter
    {
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default(CancellationToken))
            where TQuery : IQuery<TResult>;
    }
}