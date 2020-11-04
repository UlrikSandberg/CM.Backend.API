using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Events
{
	public class ChampagneFolderDeleted : DomainEvent
	{
		public bool IsDeleted { get; private set; }
		public HashSet<AggregateId> FolderContent { get; private set; }

		public ChampagneFolderDeleted(Guid id, bool isDeleted, HashSet<AggregateId> folderContent) : base(id)
		{
			IsDeleted = isDeleted;
			FolderContent = folderContent;
		}
	}
}
