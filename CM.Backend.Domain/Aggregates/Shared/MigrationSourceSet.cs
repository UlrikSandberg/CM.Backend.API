using System;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain.Aggregates.Shared
{
    public class MigrationSourceSet : DomainEvent
    {
        public MigrationSource MigrationSource { get; }

        public MigrationSourceSet(Guid id, MigrationSource migrationSource) : base(id)
        {
            MigrationSource = migrationSource;
        }
    }
}