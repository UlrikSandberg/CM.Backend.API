using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Queries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
	public class BrandPageSectionQueryHandler : 
		IQueryHandler<GetBrandPageSection, BrandPageSection>, 
		IQueryHandler<GetSections, IEnumerable<BrandPageSection>>
    {
		private readonly IBrandPageSectionRepository brandPageSectionRepository;

		public BrandPageSectionQueryHandler(IBrandPageSectionRepository brandPageSectionRepository)
        {
            this.brandPageSectionRepository = brandPageSectionRepository;
		}

		public async Task<BrandPageSection> HandleAsync(GetBrandPageSection query, CancellationToken ct)
		{
			return await brandPageSectionRepository.GetById(query.SectionId);
		}

		public async Task<IEnumerable<BrandPageSection>> HandleAsync(GetSections query, CancellationToken ct)
		{
			return await brandPageSectionRepository.GetSections(query.BrandId);
		}
	}
}
