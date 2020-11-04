using System;

namespace CM.Backend.Persistence.Model.MigrationSource
{
    public class MigrationSource : IEntity
    {
        public Guid Id { get; set; }
        public string SourceId { get; set; }
        public string MigrationSourceName { get; set; }
    }
}