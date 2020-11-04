using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class MigrationSourceEventHandler :
        IEventHandler<MessageEnvelope<MigrationSourceSet>>
    {
        private readonly IMigrationSourceRepository _migrationSourceRepository;

        public MigrationSourceEventHandler(IMigrationSourceRepository migrationSourceRepository)
        {
            _migrationSourceRepository = migrationSourceRepository;
        }
        
        public async Task HandleAsync(MessageEnvelope<MigrationSourceSet> evt, CancellationToken ct)
        {
            //Insert migrationSource
            await _migrationSourceRepository.Insert(new Persistence.Model.MigrationSource.MigrationSource
            {
                Id = evt.Id,
                MigrationSourceName = evt.Event.MigrationSource.MigrationSourceName,
                SourceId = evt.Event.MigrationSource.SourceId
            });
        }
    }
}