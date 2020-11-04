using System;

namespace CM.Backend.Persistence.Model
{
    public interface IEntity
    {
		Guid Id { get; }
    }
}