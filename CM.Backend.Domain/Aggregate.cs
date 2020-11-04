using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain
{
    public abstract class Aggregate
    {
        public Aggregate()
        {
            RegisterHandlers();
        }
        
        // For indexing our event streams
        public Guid Id { get; protected set; }
        // For protecting the state, i.e. conflict prevention
        public int Version { get; protected set; }

        private readonly List<IDomainEvent> uncommittedEvents = new List<IDomainEvent>();
        private readonly Dictionary<Type, Action<IDomainEvent>> handlers = new Dictionary<Type, Action<IDomainEvent>>();
    
        // Get the deltas, i.e. events that make up the state, not yet persisted
        public IEnumerable<IDomainEvent> GetUncommittedEvents()
        {
            return uncommittedEvents;
        }

        // Mark the deltas as persisted.
        public void ClearUncommittedEvents()
        {
            uncommittedEvents.Clear();            
        }

        // Infrastructure for raising events & registering handlers

        protected abstract void RegisterHandlers();
        
        protected void Handle<T>(Action<T> handle) where T : DomainEvent
        {
            handlers[typeof(T)] = e => handle((T)e);
        } 

        protected void RaiseEvent(IDomainEvent domainEvent)
        {
            ApplyEvent(domainEvent);
            
            domainEvent.Metadata = new EventMetadata(DateTimeOffset.UtcNow, Guid.NewGuid(), Version);
            uncommittedEvents.Add(domainEvent);
        }

        private void ApplyEvent(IDomainEvent domainEvent)
        {
            handlers[domainEvent.GetType()](domainEvent);
            
            // Each event bumps our version
            Version++;
        }    
    }
}