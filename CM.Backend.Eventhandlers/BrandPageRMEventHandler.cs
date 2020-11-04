using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.BrandPage.Events;
using CM.Backend.Domain.Aggregates.BrandPageSections.Events;
using CM.Backend.EventHandlers.URLManager;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
	public class BrandPageRMEventHandler : 
	IEventHandler<MessageEnvelope<BrandPageCreated>>,
	IEventHandler<MessageEnvelope<BrandPageSectionCreated>>,
	IEventHandler<MessageEnvelope<BrandPagePublished>>,
	IEventHandler<MessageEnvelope<SectionDeleted>>,
	IEventHandler<MessageEnvelope<BrandPageEdited>>,
	IEventHandler<MessageEnvelope<SectionAdded>>,
	IEventHandler<MessageEnvelope<SectionRemoved>>
    {
		private readonly IBrandPageRepository brandPageRepository;
		private readonly IURLManager urlManager;
              
		public BrandPageRMEventHandler(IBrandPageRepository brandPageRepository)
        {
            this.brandPageRepository = brandPageRepository;
			urlManager = new URLDelegateManager();
		}

		public async Task HandleAsync(MessageEnvelope<BrandPageCreated> evt, CancellationToken ct)
		{

			await brandPageRepository.Insert(new BrandPage
			{
				Id = evt.Id,
				BrandId = evt.Event.BrandId.Value,
				Title = evt.Event.Title.Value,
				Published = evt.Event.Publish,
				CardImgId = evt.Event.CardImgId.Value,
				HeaderImgId = evt.Event.HeaderImgId.Value,
				Url = urlManager.CreateBrandPageURL(evt.Event.BrandId.Value, evt.Id),
				SectionIds = evt.Event.SectionIds.ConvertToGuidList().ToArray(),
				UITemplateIdentifier = evt.Event.UiTemplateIdentifier.Value
                 
			});
		}

		public async Task HandleAsync(MessageEnvelope<BrandPageSectionCreated> evt, CancellationToken ct)
		{
			//await brandPageRepository.AddSection(evt.Event.BrandPageId, evt.Id);
		}

		public async Task HandleAsync(MessageEnvelope<BrandPagePublished> evt, CancellationToken ct)
		{

			await brandPageRepository.UpdatePublishingStatus(evt.Event.BrandPageId.Value, evt.Event.Publish);
		}

		public async Task HandleAsync(MessageEnvelope<SectionDeleted> evt, CancellationToken ct)
		{
			await brandPageRepository.RemoveSection(evt.Event.BrandPageId.Value, evt.Id);
		}

		public async Task HandleAsync(MessageEnvelope<BrandPageEdited> evt, CancellationToken ct)
		{
			await brandPageRepository.EditPage(evt.Id, evt.Event.Title.Value, evt.Event.CardImgId.Value, evt.Event.HeaderImgId.Value, evt.Event.UiTemplateIdentifier.Value);
		}
        
		public async Task HandleAsync(MessageEnvelope<SectionAdded> evt, CancellationToken ct)
		{
			await brandPageRepository.AddSection(evt.Id, evt.Event.SectionId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<SectionRemoved> evt, CancellationToken ct)
		{
			await brandPageRepository.RemoveSection(evt.Id, evt.Event.SectionId.Value);
		}
	}
}
