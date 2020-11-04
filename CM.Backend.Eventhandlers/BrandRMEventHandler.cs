using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.Brand.Events;
using CM.Backend.Domain.Aggregates.BrandPage.Events;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using CM.Backend.EventHandlers.URLManager;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class BrandRMEventHandler : 
        IEventHandler<MessageEnvelope<BrandCreated>>,
        IEventHandler<MessageEnvelope<ChampagneCreated>>,
	    IEventHandler<MessageEnvelope<BrandPagePublished>>,
	    IEventHandler<MessageEnvelope<BrandPublishStatusChanged>>,
	    IEventHandler<MessageEnvelope<BrandInfoUpdated>>,
	    IEventHandler<MessageEnvelope<ChampagnePublishStatusChanged>>,
	    IEventHandler<MessageEnvelope<CellarSectionDeleted>>,
	    IEventHandler<MessageEnvelope<BrandImagesUpdated>>,
	    IEventHandler<MessageEnvelope<BrandSocialUpdated>>,
	    IEventHandler<MessageEnvelope<BrandCellarUpdated>>,
	    IEventHandler<MessageEnvelope<CellarSectionCreatedAndAdded>>,
	    IEventHandler<MessageEnvelope<CellarSectionUpdated>>

    {
        private readonly IBrandRepository _repo;
		private readonly IURLManager uRLManager;

        public BrandRMEventHandler(IBrandRepository repo)
        {
            _repo = repo;
			uRLManager = new URLDelegateManager();
        }

        public async Task HandleAsync(MessageEnvelope<BrandCreated> evt, CancellationToken ct)
        {
			await _repo.Insert(new Brand
	        {
		        Id = evt.Event.Id,
		        Published = evt.Event.IsPublished,
		        Name = evt.Event.Name.Value,
		        BrandProfileText = evt.Event.ProfileText,
		        BrandCoverImgId = evt.Event.BrandCoverImageId.Value,
		        BrandListImgId = evt.Event.BrandListImageId.Value,
		        BottleCoverImgId = evt.Event.BottleCoverImageId.Value,
		        LogoImgId = evt.Event.LogoImageId.Value,
		        Cellar = new BrandCellar
		        {
			        CardImgId = evt.Event.Cellar.CardImageId.Value,
			        CoverImgId = evt.Event.Cellar.CoverImageId.Value,
			        Sections = new List<CellarSection>(),
			        Title = evt.Event.Cellar.Title,
			        Url = uRLManager.CreateBrandCellarURL(evt.Id, evt.Id)
		        },
		        Social = new Social
		        {
			        FacebookUrl = evt.Event.FacebookUrl.Url,
			        InstagramUrl = evt.Event.InstagramUrl.Url,
			        PinterestUrl = evt.Event.PinterestUrl.Url,
			        TwitterUrl = evt.Event.TwitterUrl.Url,
			        WebsiteUrl = evt.Event.WebsiteUrl.Url
		        }
	        });
        }

        public async Task HandleAsync(MessageEnvelope<ChampagneCreated> evt, CancellationToken ct)
        {
            await _repo.AddChampagne(evt.Event.BrandId.Value, evt.Id);
        }
        
		public async Task HandleAsync(MessageEnvelope<BrandPagePublished> evt, CancellationToken ct)
		{
			if(evt.Event.Publish)
			{
				await _repo.AddBrandPage(evt.Event.BrandId.Value, evt.Id);
			}
			else
			{
				await _repo.RemoveBrandPage(evt.Event.BrandId.Value, evt.Id);
			}
		}

		public async Task HandleAsync(MessageEnvelope<BrandPublishStatusChanged> evt, CancellationToken ct)
		{
			await _repo.UpdatePublishingStatus(evt.Event.BrandId.Value, evt.Event.PublishStatus);
		}


	    public async Task HandleAsync(MessageEnvelope<ChampagnePublishStatusChanged> evt, CancellationToken ct)
	    {
		    if (evt.Event.IsPublished)
		    {
			    await _repo.AddChampagneToPublished(evt.Event.BrandId.Value, evt.Event.Id);
		    }
		    else
		    {
			    await _repo.RemoveChampagneFromPublished(evt.Event.BrandId.Value, evt.Event.Id);
		    }
	    }
	    
	    public async Task HandleAsync(MessageEnvelope<BrandInfoUpdated> evt, CancellationToken ct)
	    {
		    await _repo.UpdateBrandInfo(evt.Id, evt.Event.Name.Value, evt.Event.ProfileText);
	    }

	    public async Task HandleAsync(MessageEnvelope<CellarSectionDeleted> evt, CancellationToken ct)
	    {
		    await _repo.DeleteCellarSection(evt.Id, evt.Event.SectionId.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<BrandImagesUpdated> evt, CancellationToken ct)
	    {
		    await _repo.UpdateBrandImages(evt.Id, evt.Event.BrandCoverImageId.Value, evt.Event.BrandListImageId.Value,
			    evt.Event.BottleCoverImageId.Value, evt.Event.LogoImageId.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<BrandSocialUpdated> evt, CancellationToken ct)
	    {
		    await _repo.UpdateBrandSocial(evt.Id, new Social
		    {
				FacebookUrl = evt.Event.FacebookUrl.Url,
			    InstagramUrl = evt.Event.InstagramUrl.Url,
			    PinterestUrl = evt.Event.PinterestUrl.Url,
			    TwitterUrl = evt.Event.TwitterUrl.Url,
			    WebsiteUrl = evt.Event.WebsiteUrl.Url
		    });
	    }

	    public async Task HandleAsync(MessageEnvelope<BrandCellarUpdated> evt, CancellationToken ct)
	    {
		    await _repo.UpdateBrandCellar(evt.Id, evt.Event.CoverImageId.Value, evt.Event.CardImageId.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<CellarSectionCreatedAndAdded> evt, CancellationToken ct)
	    {
		    await _repo.AddCellarSection(evt.Id, new CellarSection
		    {
				Id = evt.Event.SectionId.Value,
			    Body = evt.Event.Body,
			    Title = evt.Event.Title,
			    Champagnes = new List<Guid>(evt.Event.Champagnes.ConvertToGuidList())
		    });
	    }

	    public async Task HandleAsync(MessageEnvelope<CellarSectionUpdated> evt, CancellationToken ct)
	    {
		    await _repo.UpdateCellarSection(evt.Id, evt.Event.SectionId.Value, new CellarSection
		    {
				Id = evt.Event.SectionId.Value,
			    Body = evt.Event.Body,
			    Title = evt.Event.Title,
			    Champagnes = new List<Guid>(evt.Event.Champagnes.ConvertToGuidList())
		    });
	    }
    }
}