using System;
using System.Collections.Specialized;
using CM.Backend.Domain.Aggregates.Brand.Commands;
using CM.Backend.Domain.Aggregates.Brand.Entities;
using CM.Backend.Domain.Aggregates.Brand.Events;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand
{
    public class Brand : Aggregate
    {
        public BrandName Name { get; private set; }

        public string ProfileText { get; private set; }

        public ImageId BrandCoverImageId { get; private set; }
	    
	    public ImageId BrandListImageId { get; private set; }

        public ImageId BottleCoverImageId { get; private set; }
	    
	    public ImageId LogoImageId { get; private set; }

		public Cellar Cellar { get; private set; }

        public Social Social { get; private set; }

		public bool IsPublished { get; private set; }
	    
	    public MigrationSource MigrationSource { get; private set; }
		
        public void Execute(CreateBrand cmd)
        {
	        RaiseEvent(new BrandCreated(cmd.Id.Value, cmd.Name, cmd.ProfileText, cmd.BrandCoverImageId,
		        cmd.BrandListImageId, cmd.BottleCoverImageId, cmd.LogoImageId, cmd.InstagramUrl, cmd.FacebookUrl,
		        cmd.PinterestUrl, cmd.TwitterUrl, cmd.WebsiteUrl, cmd.Cellar, cmd.IsPublished));
        }

	    public void Execute(UpdateBrandInfo cmd)
	    {
		    var didNameChange = !Name.Value.Equals(cmd.Name);
		    
		    RaiseEvent(new BrandInfoUpdated(Id, cmd.Name, cmd.ProfileText, didNameChange));
	    }

	    public void Execute(UpdateBrandImages cmd)
	    {
		    var didLogoChange = !LogoImageId.Value.Equals(cmd.LogoImageId.Value);
		    
		    RaiseEvent(new BrandImagesUpdated(Id, cmd.BrandCoverImageId, cmd.BrandListImageId, cmd.BottleCoverImageId,
			    cmd.LogoImageId, didLogoChange));
	    }

	    public void Execute(UpdateSocial cmd)
	    {
		    RaiseEvent(new BrandSocialUpdated(Id, cmd.InstagramUrl, cmd.FacebookUrl, cmd.PinterestUrl, cmd.TwitterUrl,
			    cmd.WebsiteUrl));
	    }

	    public void Execute(UpdateCellar cmd)
	    {
		    RaiseEvent(new BrandCellarUpdated(Id, cmd.CardImageId, cmd.CoverImageId));
	    }

	    public void Execute(CreateAndAddCellarSection cmd)
	    {
		    RaiseEvent(new CellarSectionCreatedAndAdded(Id, cmd.Id, cmd.Title, cmd.Body, cmd.Champagnes));
	    }

	    public void Execute(DeleteCellarSection cmd)
	    {
		    RaiseEvent(new CellarSectionDeleted(Id, cmd.SectionId));
	    }

	    public void Execute(UpdateCellarSection cmd)
	    {
		    RaiseEvent(new CellarSectionUpdated(Id, cmd.SectionId, cmd.Title, cmd.Body, cmd.Champagnes));
	    }
	    
	    public void Execute(SetPublishingStatusBrand cmd)
	    {
		    RaiseEvent(new BrandPublishStatusChanged(
			    Id,
			    cmd.BrandId,
			    cmd.PublishStatus
		    ));
	    }

	    public void Execute(SetMigrationSource cmd)
	    {
		    RaiseEvent(new MigrationSourceSet(Id, cmd.MigrationSource));
	    }
	    
	    

        protected override void RegisterHandlers()
        {
	        //***** New handlers ******
	        Handle<BrandCreated>(evt =>
	        {
		        Id = evt.Id;
		        Name = evt.Name;
		        ProfileText = evt.ProfileText;
		        BrandCoverImageId = evt.BrandCoverImageId;
		        BottleCoverImageId = evt.BottleCoverImageId;
		        BrandListImageId = evt.BrandListImageId;
		        LogoImageId = evt.LogoImageId;
		        Cellar = evt.Cellar;
		        IsPublished = evt.IsPublished;
		        Social = new Social
		        {
			        FacebookUrl = evt.FacebookUrl,
			        InstagramUrl = evt.InstagramUrl,
			        PinterestUrl = evt.PinterestUrl,
			        TwitterUrl = evt.TwitterUrl,
			        WebsiteUrl = evt.WebsiteUrl
		        };
	        });
	        
	        Handle<BrandInfoUpdated>(evt =>
	        {
		        Name = evt.Name;
		        ProfileText = evt.ProfileText;
	        });

	        Handle<BrandImagesUpdated>(evt =>
	        {
		        BrandCoverImageId = evt.BrandCoverImageId;
		        BrandListImageId = evt.BrandListImageId;
		        BottleCoverImageId = evt.BottleCoverImageId;
		        LogoImageId = evt.LogoImageId;
	        });

	        Handle<BrandSocialUpdated>(evt =>
	        {
		        Social.PinterestUrl = evt.PinterestUrl;
		        Social.FacebookUrl = evt.FacebookUrl;
		        Social.InstagramUrl = evt.InstagramUrl;
		        Social.TwitterUrl = evt.TwitterUrl;
		        Social.WebsiteUrl = evt.WebsiteUrl;
	        });

	        Handle<BrandCellarUpdated>(evt =>
	        {
		        Cellar.CardImageId = evt.CardImageId;
		        Cellar.CoverImageId = evt.CoverImageId;
	        });

	        Handle<CellarSectionCreatedAndAdded>(evt =>
	        {
		        Cellar.Sections.Add(new CellarSection(evt.SectionId, evt.Title, evt.Body, evt.Champagnes));
	        });

	        Handle<CellarSectionDeleted>(evt =>
	        {
		        CellarSection toBeDeleted = null;

		        foreach (var section in Cellar.Sections)
		        {
			        if (!section.Id.Value.Equals(evt.SectionId.Value))
			        {
				        toBeDeleted = section;
				        break;
			        }
		        }

		        if (toBeDeleted != null)
		        {
			        Cellar.Sections.Remove(toBeDeleted);
		        }
	        });

	        Handle<CellarSectionUpdated>(evt =>
	        {
		        foreach (var section in Cellar.Sections)
		        {
			        if (section.Id.Value.Equals(evt.SectionId.Value))
			        {
				        section.Title = evt.Title;
				        section.Body = evt.Body;
				        section.Champagnes = evt.Champagnes;
			        }
		        }
	        });
	        
	        Handle<BrandPublishStatusChanged>(evt => { IsPublished = evt.PublishStatus; });

	        Handle<MigrationSourceSet>(evt => { MigrationSource = evt.MigrationSource; });
        }
    }
}