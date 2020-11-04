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

	public interface IBrandPageSectionRepository : IMongoReadmodelRepository<BrandPageSection>
	{
		Task EditSection(Guid sectionId, string title, string subTitle, string body, Guid[] images, Guid[] champagnes);
		Task<IEnumerable<BrandPageSection>> GetSections(Guid brandId);
	}

	public class BrandPageSectionRepository : MongoReadmodelRepository<BrandPageSection>, IBrandPageSectionRepository
	{
		public BrandPageSectionRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task EditSection(Guid sectionId, string title, string subTitle, string body, Guid[] images, Guid[] champagnes)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(s => s.Id == sectionId,
					Update.Set(s => s.Title, title)
						.Set(s => s.Body, body)
						.Set(s => s.Images, images)
						.Set(s => s.Champagnes, champagnes)
						.Set(s => s.SubTitle, subTitle)),
				$"{nameof(EditSection)} - {nameof(BrandPageSectionRepository)}");
		}

		public async Task<IEnumerable<BrandPageSection>> GetSections(Guid brandId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(s => s.BrandId == brandId)
					.ToListAsync(),
				$"{nameof(GetSections)} - {nameof(BrandPageSectionRepository)}");
		}
	}
}
