using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
	public interface IBrandImageRepository : IMongoReadmodelRepository<BrandImage>
	{
		Task<IEnumerable<BrandImage>> GetAllByBrandId(Guid brandId); 
	}

	public class BrandImageRepository : MongoReadmodelRepository<BrandImage>, IBrandImageRepository
	{
		public BrandImageRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task<IEnumerable<BrandImage>> GetAllByBrandId(Guid brandId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(i => i.BrandId == brandId)
					.ToListAsync(),
				$"{nameof(GetAllByBrandId)} - {nameof(BrandImageRepository)}");
		}
	}
}
