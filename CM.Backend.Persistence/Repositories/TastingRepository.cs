using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{

    public interface ITastingRepository : IMongoReadmodelRepository<Tasting>
    {
        Task<Tasting> GetTasting(Guid userId, Guid champagneId);
        Task UpdateTasting(Guid tastingId, string review, double rating);
        Task<int> CountTastingsForBrand(Guid brandId);
        Task<IEnumerable<Tasting>> GetTastingsPagedByChampagneIdAsync(Guid champagneId, int page, int pageSize,
            bool orderAscendingByDate = false);
        Task<IEnumerable<Tasting>> GetTastingsPagedByChampagneIdFilteredAsync(Guid champagneId, int page, int pageSize,
            TastingOrderByOption.OrderBy orderBy);
        Task<Dictionary<Guid, int>> CountTastingsForBrandIdsAsync(List<Guid> brandIds);
    }
    
    public class TastingRepository : MongoReadmodelRepository<Tasting>, ITastingRepository
    {
        public TastingRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<Tasting> GetTasting(Guid userId, Guid champagneId)
        {
            var query = await ExecuteCmd(() =>
                (from e in DefaultCollection.AsQueryable()
                    where e.AuthorId == userId
                    where e.ChampagneId == champagneId
                    select e)
                .SingleOrDefaultAsync()
            );

            return query;
        }

        public async Task UpdateTasting(Guid tastingId, string review, double rating)
        {
            await ExecuteCmd(() =>
                DefaultCollection.FindOneAndUpdateAsync(t => t.Id == tastingId, Update
                    .Set(t => t.Review, review)
                    .Set(t => t.Rating, rating)));
        }

        public async Task<int> CountTastingsForBrand(Guid brandId)
        {
            var count = await ExecuteCmd(() =>
                DefaultCollection.CountDocumentsAsync(t => t.BrandId == brandId)
                , $"{nameof(CountTastingsForBrand)} - {nameof(TastingRepository)}");
            
            return (int) count;
        }

        /// <summary>
        /// Returns a list of tastings respective to the provided champagneId.
        /// Set orderAscendingBydate to true=descending order
        /// Set orderAscendingToByDate to false=ascending order
        ///
        /// These are flipped but can't currently be revoked since the flip is currently d.12-1-2019 active in the app
        /// 
        /// </summary>
        /// <param name="champagneId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderAscendingByDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Tasting>> GetTastingsPagedByChampagneIdAsync(Guid champagneId, int page, int pageSize, bool orderAscendingByDate = false)
        {
            SortDefinition<Tasting> sortDefinition = null;

            if (orderAscendingByDate)
            {
                sortDefinition = Sort.Descending(e => e.TastedOnDate);
            }
            else
            {
                sortDefinition = Sort.Ascending(e => e.TastedOnDate);
            }

            return await ExecuteCmd(() =>
                DefaultCollection
                    .Find(f => f.ChampagneId == champagneId && f.Review != null)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .Sort(sortDefinition)
                    .ToListAsync()
            );
        }

        /// <summary>
        /// Returns a list of tastings respective to the champagneId
        ///
        /// Remarks Ascending and descending has been flipped. This have not been changed seeing as it is currently 12-1-2019 in effect in the app.
        /// 
        /// </summary>
        /// <param name="champagneId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Tasting>> GetTastingsPagedByChampagneIdFilteredAsync(Guid champagneId, int page, int pageSize, TastingOrderByOption.OrderBy orderBy)
        {
            SortDefinition<Tasting> sortDefinition = null;
            
            switch (orderBy)
            {
                case TastingOrderByOption.OrderBy.AcendingByDate:
                    sortDefinition = Sort.Descending(e => e.TastedOnDate);
                    break;
                case TastingOrderByOption.OrderBy.DecendingByDate:
                    sortDefinition = Sort.Ascending( e => e.TastedOnDate);
                    break;
                case TastingOrderByOption.OrderBy.AcendingByRating:
                    sortDefinition = Sort.Ascending(e => e.Rating);
                    break;
                case TastingOrderByOption.OrderBy.DecendingByRating:
                    sortDefinition = Sort.Descending(e => e.Rating);
                    break;
            }

            return await ExecuteCmd(() =>
                DefaultCollection
                    .Find(f => f.ChampagneId == champagneId && f.Review != null)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .Sort(sortDefinition)
                    .ToListAsync(),
                $"{nameof(GetTastingsPagedByChampagneIdFilteredAsync)} - {nameof(TastingRepository)}"
                );
        }

        public async Task<Dictionary<Guid, int>> CountTastingsForBrandIdsAsync(List<Guid> brandIds)
        {
            var result = await ExecuteCmd(() =>
                    DefaultCollection.AsQueryable()
                        .Where(x => brandIds.Contains(x.BrandId))
                        .GroupBy(x => x.BrandId)
                        .Select(n => new {n.Key, Count = n.Count()})
                        .ToListAsync(),
                $"{nameof(CountTastingsForBrandIdsAsync)} - {nameof(TastingRepository)}");

            return result.ToDictionary(x => x.Key, x => x.Count);
        }
    }
}