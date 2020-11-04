using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
	public class BrandInfoUpdated : DomainEvent
	{
		public BrandName Name { get; private set; }
		public string ProfileText { get; private set; }
		public bool DidNameChange { get; private set; }

		public BrandInfoUpdated(Guid id, BrandName name, string profileText, bool didNameChange) : base(id)
		{
			Name = name;
			ProfileText = profileText;
			DidNameChange = didNameChange;
		}
	}
}
