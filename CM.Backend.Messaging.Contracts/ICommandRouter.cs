using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Responses;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Contracts
{
    public interface ICommandRouter
    {
        Task<TResponse> RouteAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand<TResponse>;
        Task RouteAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<Response>;
    }
}