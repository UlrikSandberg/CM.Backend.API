﻿using System.Collections.Generic;
using SimpleSoft.Mediator;
using SimpleSoft.Mediator.Pipeline;
using StructureMap;

namespace CM.Backend.Messaging.Infrastructure.Factories
{
    class StructureMapMediatorFactory : IMediatorFactory
    {
        private readonly IContainer _container;

        public StructureMapMediatorFactory(IContainer container)
        {
            _container = container;
        }

        public ICommandHandler<TCommand> BuildCommandHandlerFor<TCommand>() where TCommand : ICommand
        {
            return _container.GetInstance<ICommandHandler<TCommand>>();
        }

        public ICommandHandler<TCommand, TResult> BuildCommandHandlerFor<TCommand, TResult>() where TCommand : ICommand<TResult>
        {
            return _container.GetInstance<ICommandHandler<TCommand, TResult>>();
        }

        public IEnumerable<IEventHandler<TEvent>> BuildEventHandlersFor<TEvent>() where TEvent : IEvent
        {
            var handlers = _container.GetAllInstances<IEventHandler<TEvent>>();

            return handlers;
        }

        public IQueryHandler<TQuery, TResult> BuildQueryHandlerFor<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            return _container.GetInstance<IQueryHandler<TQuery, TResult>>();
        }

        public IEnumerable<ICommandMiddleware> BuildCommandMiddlewares()
        {
            return _container.GetAllInstances<ICommandMiddleware>();
        }

        public IEnumerable<IEventMiddleware> BuildEventMiddlewares()
        {
            return _container.GetAllInstances<IEventMiddleware>();
        }

        public IEnumerable<IQueryMiddleware> BuildQueryMiddlewares()
        {
            return _container.GetAllInstances<IQueryMiddleware>();
        }
    }
}
