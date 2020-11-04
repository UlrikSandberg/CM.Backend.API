using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Queries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
	public class BrandPageImageQueryHandler : 
	IQueryHandler<GetBrandImages, IEnumerable<BrandImage>>,
	IQueryHandler<GetBrandImage, BrandImage>
    {
		private readonly IBrandImageRepository brandImageRepository;

		public BrandPageImageQueryHandler(IBrandImageRepository brandImageRepository)
        {
            this.brandImageRepository = brandImageRepository;
		}

		public async Task<IEnumerable<BrandImage>> HandleAsync(GetBrandImages query, CancellationToken ct)
		{
			return await brandImageRepository.GetAllByBrandId(query.BrandId);
		}

		public async Task<BrandImage> HandleAsync(GetBrandImage query, CancellationToken ct)
		{
			return await brandImageRepository.GetById(query.Id);
		}
	}
}
