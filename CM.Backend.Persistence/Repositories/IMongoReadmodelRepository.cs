using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using MongoDB.Driver;

namespace CM.Backend.Persistence.Repositories
{
    public interface IMongoReadmodelRepository<TEntity> where TEntity : IEntity
    {
        Task<bool> Delete(Guid id);
        Task<IEnumerable<TEntity>> GetAll(int skip = 0, int limit = 100);
        Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
            Expression<Func<TEntity, TNewProjection>> projection, 
            int skip = 0, int limit = 100);
        Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
            Expression<Func<TEntity, TNewProjection>> projection, 
            Expression<Func<TEntity, object>> sortDefiniton, 
            int skip = 0, int limit = 100);

        Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
            Expression<Func<TEntity, TNewProjection>> projection,
            Expression<Func<TEntity, object>> sortDefiniton, 
            Expression<Func<TEntity, bool>> filter, 
            int skip = 0, int limit = 100);

        Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
            Expression<Func<TEntity, TNewProjection>> projection,
            Expression<Func<TEntity, bool>> filter,
            int skip = 0,
            int limit = 100);
        
        Task<IEnumerable<TEntity>> GetPaged(int page, int pageSize);
        Task<TEntity> GetById(Guid id);
        Task<Guid> Insert(TEntity entity);
        Task<TEntity> GetByIndex(int index, int collectionSize);
    }
}