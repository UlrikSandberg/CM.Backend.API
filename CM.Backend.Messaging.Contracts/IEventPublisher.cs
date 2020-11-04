using System.Threading.Tasks;
using CM.Backend.Documents.Events;

namespace CM.Backend.Messaging.Contracts
   {
       public interface IEventPublisher
       {
           Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
       }
   }