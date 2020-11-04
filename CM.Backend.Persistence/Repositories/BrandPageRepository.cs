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

	public interface IBrandPageRepository : IMongoReadmodelRepository<BrandPage>
	{
		Task AddSection(Guid brandPageId, Guid sectionId);
		Task<IEnumerable<BrandPage>> GetAllById(Guid brandId);
		Task UpdatePublishingStatus(Guid brandPageId, bool publish);
		Task RemoveSection(Guid pageId,Guid sectionId);
		Task EditPage(Guid pageId, string title, Guid cardImgId, Guid headerImgId, string uiTemplateIdentifier);
    }


	public class BrandPageRepository : MongoReadmodelRepository<BrandPage>, IBrandPageRepository
	{
		public BrandPageRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task AddSection(Guid brandPageId, Guid sectionId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(b => b.Id == brandPageId,
					Update.AddToSet(b => b.SectionIds, sectionId)),
				$"{nameof(AddSection)} - {nameof(BrandPageRepository)}");
		}

		public async Task EditPage(Guid pageId, string title, Guid cardImgId, Guid headerImgId, string uiTemplateIdentifier)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(p => p.Id == pageId,
					Update.Set(p => p.Title, title)
						.Set(p => p.CardImgId, cardImgId)
						.Set(p => p.HeaderImgId, headerImgId)
						.Set(p => p.UITemplateIdentifier, uiTemplateIdentifier)),
				$"{nameof(EditPage)} - {nameof(BrandPageRepository)}");
			
		}

		public async Task<IEnumerable<BrandPage>> GetAllById(Guid brandId)
		{
			return await ExecuteCmd(() =>
					DefaultCollection.Find(b => b.BrandId == brandId).ToListAsync(),
				$"{nameof(GetAllById)} - {nameof(BrandPageRepository)}");
		}
        
		public async Task RemoveSection(Guid pageId, Guid sectionId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(s => s.Id == pageId, Update.Pull(x => x.SectionIds, sectionId)),
				$"{nameof(RemoveSection)} - {nameof(BrandPageRepository)}");
		}

		public async Task UpdatePublishingStatus(Guid brandPageId, bool publish)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(b => b.Id == brandPageId, Update.Set(a => a.Published, publish)),
				$"{nameof(UpdatePublishingStatus)} - {nameof(BrandPageRepository)}");
		}
	}
}
