using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CM.Backend.Documents.Events;
using CM.Backend.Documents.GlobalLogging;
using CM.Backend.Domain;
using CM.Backend.Messaging.Contracts;
using Marten;
using Marten.Linq;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace CM.Backend.Eventstore.Persistence
{
    public sealed class PublishingAggregateRepository : IPublishingAggregateRepository
    {
        private readonly IDocumentStore _store;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PublishingAggregateRepository(IDocumentStore store, IEventPublisher eventPublisher, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _store = store;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Stores the uncomitted events in the eventstore.
        /// Events are published via an <see cref="IEventPublisher"/>
        /// </summary>
        /// <param name="aggregate">The DDD aggregate to store events for</param>
        /// <param name="transientSession">If supplied, this session represents the transaction boundary. Remember to call SaveChanges yoorself if you use this</param>
        public async Task StoreAsync(Aggregate aggregate, IDocumentSession transientSession = null)
        {
            var sw = new Stopwatch();
            sw.Start();//Track the communication in elapsed millis between API and postgress sql
            using (var session = transientSession ?? _store.OpenSession())
            {
                // Take non-persisted events, push them to the event stream, indexed by the aggregate ID
                var events = aggregate.GetUncommittedEvents().ToArray();
                
                session.Events.Append(aggregate.Id, events);
                //Only save and publish changes if it is our own session (meaning none supplied)
                if (transientSession == null)
                {
                    await session.SaveChangesAsync();
                    await PublishEventsAsync(events);
                    
                    _logger.Information("Persisted {@Aggregate} to event stream and published events", aggregate);
                    
                    // Once succesfully persisted, clear events from list of uncommitted events
                    aggregate.ClearUncommittedEvents();   
                }
            }    
            sw.Stop();
            SharedLoggingProperties.AddPostgressCommunicationInfo(
                _httpContextAccessor.HttpContext.TraceIdentifier,
                sw.ElapsedMilliseconds,
                $"{nameof(StoreAsync)} - {typeof(Aggregate).Name}");
            
        }        

        private static readonly MethodInfo ApplyEvent = typeof(Aggregate).GetMethod("ApplyEvent", BindingFlags.Instance | BindingFlags.NonPublic);

        public async Task<T> LoadAsync<T>(Guid id, int? version = null) where T : Aggregate
        {
            var sw = new Stopwatch();
            sw.Start();
            IReadOnlyList<Marten.Events.IEvent> events;
            using (var session = _store.LightweightSession())
            {
                events = await session.Events.FetchStreamAsync(id, version ?? 0);                
            }
            sw.Stop();
            SharedLoggingProperties.AddPostgressCommunicationInfo(
                _httpContextAccessor.HttpContext.TraceIdentifier,
                sw.ElapsedMilliseconds,
                $"{nameof(LoadAsync)} - {typeof(T).Name}");
            
            if (events == null || !events.Any()) throw new InvalidOperationException($"No aggregate by id {id}.");
            
            var instance = Activator.CreateInstance(typeof(T), true);                
            // Replay our aggregate state from the event stream
            events.Aggregate(instance, (o, @event) => ApplyEvent.Invoke(instance, new [] { @event.Data }));
            return (T)instance;

        }

        /// <summary>
        /// Queries the entire event stream for events of type T. Use this with caution and only for debug
        /// </summary>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> QueryRawEvents<T>(Func<T, bool> predicate)
        {
            using (var session = _store.LightweightSession())
            {
                return session.Events.QueryRawEventDataOnly<T>().Where(predicate);
            }
        }

        public IDocumentSession CreateSession()
        {
            return _store.OpenSession();
        }

        public async Task PublishEventsAsync(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                await _eventPublisher.PublishAsync(@event);
            }
        }
    }
}