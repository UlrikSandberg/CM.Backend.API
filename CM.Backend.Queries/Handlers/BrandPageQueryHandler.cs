using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class BrandPageQueryHandler : 
    IQueryHandler<GetBrandPage, BrandPageExtendedSection>, 
    IQueryHandler<GetBrandPages, IEnumerable<BrandPage>>
    {
        private readonly IBrandPageRepository brandPageRepository;
        private readonly IBrandPageSectionRepository brandPageSectionRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IBrandImageRepository brandImageRepository;

        public BrandPageQueryHandler(IBrandPageRepository brandPageRepository, IBrandPageSectionRepository brandPageSectionRepository, IBrandRepository brandRepository, IBrandImageRepository brandImageRepository)
        {
            this.brandRepository = brandRepository;
            this.brandImageRepository = brandImageRepository;
            this.brandPageSectionRepository = brandPageSectionRepository;
            this.brandPageRepository = brandPageRepository;
        }

        public async Task<BrandPageExtendedSection> HandleAsync(GetBrandPage query, CancellationToken ct)
        {
            
            //Get respective brandPage from persistence
            var brandPage = await brandPageRepository.GetById(query.BrandPageId);

            var brandPageExtendedSection = new BrandPageExtendedSection();
			brandPageExtendedSection.Sections = new List<BrandPageSectionModel>();
                    
            //Foreach section in the brandPage get the section details.
			foreach(Guid sectionId in brandPage.SectionIds)
			{
				var brandPageSection = await brandPageSectionRepository.GetById(sectionId);
				brandPageExtendedSection.Sections.Add(await convertToBrandPageSectionModel((brandPageSection)));

			}

            //Sections has been found. Map rest of the data
            brandPageExtendedSection.BrandPageId = brandPage.Id;
            brandPageExtendedSection.BrandId = brandPage.BrandId;
            brandPageExtendedSection.Title = brandPage.Title;
            brandPageExtendedSection.BrandName = (await brandRepository.GetById(brandPage.BrandId)).Name;
            brandPageExtendedSection.uiTemplateIdentifier = brandPage.UITemplateIdentifier;
            brandPageExtendedSection.Published = brandPage.Published;
            brandPageExtendedSection.CardImgId = brandPage.CardImgId;
            brandPageExtendedSection.HeaderImgId = brandPage.HeaderImgId;
            brandPageExtendedSection.HeaderImg = await brandImageRepository.GetById(brandPage.HeaderImgId);
            brandPageExtendedSection.Url = brandPage.Url;


            return brandPageExtendedSection;
        }

        public async Task<IEnumerable<BrandPage>> HandleAsync(GetBrandPages query, CancellationToken ct)
        {
            return await brandPageRepository.GetAllById(query.BrandId);
        }

        private async Task<BrandPageSectionModel> convertToBrandPageSectionModel(BrandPageSection section)
        {
            var brandPageSectionModel = new BrandPageSectionModel
            {
                Id = section.Id,
                BrandId = section.BrandId,
                Title = section.Title,
                SubTitle = section.SubTitle,
                Body = section.Body,
                Champagnes = section.Champagnes,
                ImageIds = section.Images
            };


            foreach (var imageId in brandPageSectionModel.ImageIds)
            {
                var image = await brandImageRepository.GetById(imageId);
                brandPageSectionModel.Images.Add(image);
            }

            return brandPageSectionModel;
        }
    }
}
