using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class DeleteCellarSection
    {
        public AggregateId SectionId { get; private set; }

        public DeleteCellarSection(AggregateId sectionId)
        {
            if (sectionId == null)
            {
                throw new ArgumentException(nameof(sectionId) + ": DeleteCellarSection(AggregateId sectionId) --> SectionId may not be null");
            }
            
            SectionId = sectionId;
        }
    }
}