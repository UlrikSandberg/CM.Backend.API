using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class CreateAndAddCellarSection
    {
        public AggregateId Id { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public List<AggregateId> Champagnes { get; private set; }

        public CreateAndAddCellarSection(AggregateId id, string title, string body, List<Guid> champagnes)
        {
            if (id == null)
            {
                throw new ArgumentException(nameof(id) + ": CreateAndAddCellarSection(AggregateId id,...) AggregateId may not be null");
            }
            
            if (champagnes == null)
            {
                throw new ArgumentException(nameof(champagnes) + ": CellarSection cannot be created with the property --> List<Guid> champagnes == null");
            }
            
            Id = id;
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