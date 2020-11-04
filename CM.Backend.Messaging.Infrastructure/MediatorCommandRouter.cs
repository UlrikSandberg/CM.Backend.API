using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Infrastructure
{
    public class MediatorCommandRouter : ICommandRouter 
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public MediatorCommandRouter(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<TResponse> RouteAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) 
            where TCommand : ICommand<TResponse>
        {
            _logger.Information("Routing {CommandName}: {@Command}", command.GetType().Name, command);
            return await _mediator.SendAsync<TCommand, TResponse>(command, cancellationToken);
        }

        public async Task RouteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) 
            where TCommand : ICommand<Response>
        {
            var response = await _mediator.SendAsync<TCommand, Response>(command, cancellationToken);

            _logger.Information("Routing {CommandName}: {@Command} got {@Response}", command.GetType().Name, command, response);
            if (!response.IsSuccessful)
            {
                throw new CommandExecutionException(response.Message, command, response.Exception);
            }
        }
    }

    public class CommandExecutionException : Exception
    {
        public object Command { get; }

        public CommandExecutionException(string message, object command, Exception innerException) : base(message, innerException)
        {
            Command = command;
        }
    }
}