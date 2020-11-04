using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.BrandPageSections.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
	public class BrandPageSectionRMEventHandler : 
	IEventHandler<MessageEnvelope<BrandPageSectionCreated>>,
	IEventHandler<MessageEnvelope<SectionDeleted>>,
	IEventHandler<MessageEnvelope<SectionEdited>>
	{
		private readonly IBrandPageSectionRepository brandPageSectionRepository;

		public BrandPageSectionRMEventHandler(IBrandPageSectionRepository brandPageSectionRepository)
		{
			this.brandPageSectionRepository = brandPageSectionRepository;
		}

		public async Task HandleAsync(MessageEnvelope<BrandPageSectionCreated> evt, CancellationToken ct)
		{

			await brandPageSectionRepository.Insert(new BrandPageSection
			{
				Id = evt.Id,
				BrandId = evt.Event.BrandId.Value,
				Title = evt.Event.Title,
				SubTitle = evt.Event.SubTitle,
				Body = evt.Event.Body,
				Champagnes = evt.Event.ChampagneIds.ConvertToGuidList().ToArray(),
				Images = evt.Event.ImageIds.ConvertToGuidList().ToArray()

			});
		}

		public async Task HandleAsync(MessageEnvelope<SectionDeleted> evt, CancellationToken ct)
		{
			await brandPageSectionRepository.Delete(evt.Id);
		}

		public async Task HandleAsync(MessageEnvelope<SectionEdited> evt, CancellationToken ct)
		{
			await brandPageSectionRepository.EditSection(
				evt.Id,
				evt.Event.Title,
				evt.Event.SubTitle,
				evt.Event.Body,
				evt.Event.ImageIds.ConvertToGuidList().ToArray(),
				evt.Event.ChampagneIds.ConvertToGuidList().ToArray());
		}
	}
}
