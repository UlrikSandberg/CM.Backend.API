using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class UpdateCellarSection
    {
        public AggregateId SectionId { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public List<AggregateId> Champagnes { get; private set; }

        public UpdateCellarSection(AggregateId sectionId, string title, string body, List<Guid> champagnes)
        {
            if (sectionId == null)
            {
                throw new ArgumentException(nameof(sectionId) + ": UpdateCellarSection(AggregateId sectionId,...) --> SectionId is null which is not valid in the given context");
            }
            
            if (champagnes == null)
            {
                throw new ArgumentException(nameof(champagnes) + ": CellarSection cannot be created with the property --> List<Guid> champagnes == null");
            }
            
            SectionId = sectionId;
            Title = title;
            Body = body;
            
            var aggregateIdList = new List<AggregateId>();

            foreach (var champagneId in champagnes)
            {
                var aggregateId = new AggregateId(champagneId);
                aggregateIdList.Add(aggregateId);
            }
            
            Champagnes = aggregateIdList;
        }
    }
}