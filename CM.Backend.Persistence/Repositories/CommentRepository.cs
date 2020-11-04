using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{

    public interface ICommentRepository : IMongoReadmodelRepository<Comment>
    {
        Task EditComment(Guid commentId, string content);
        Task<int> CountCommentsForContextId(Guid contextId);
        Task<bool> IsCommentedByUser(Guid contextId, Guid userId);
        Task<IEnumerable<Comment>> GetCommentsForContextIdPagedAsync(Guid contextId, int page, int pageSize,
            bool orderAcendingByDate = false);
        Task<IEnumerable<Comment>> GetCommentsForContextIdFromPeriodAsync(Guid contextId, TimeSpan daysBack);
        Task UpdateAuthorNameBatchAsync(Guid authorId, string authorName);
        Task UpdateAuthorProfileImageIdBatchAsync(Guid authorId, Guid authorProfileImageId);
    }
    
    public class CommentRepository : MongoReadmodelRepository<Comment>, ICommentRepository
    {
        public CommentRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task EditComment(Guid commentId, string content)
        {
            await ExecuteCmd(() =>
                DefaultCollection.FindOneAndUpdateAsync(c => c.Id == commentId,
                    Update.Set(c => c.Content, content)),
                $"{nameof(EditComment)} - {nameof(CommentRepository)}");
        }

        public async Task<int> CountCommentsForContextId(Guid contextId)
        {
            var count = await ExecuteCmd(() =>
                DefaultCollection.CountDocumentsAsync(c => c.ContextId == contextId),
                $"{nameof(CountCommentsForContextId)} - {nameof(CommentRepository)}");

            return (int) count;
        }

        public async Task<bool> IsCommentedByUser(Guid contextId, Guid userId)
        {
            var query = from e in DefaultCollection.AsQueryable<Comment>()
                where e.ContextId == contextId
                where e.AuthorId == userId
                select e;

            if (await ExecuteCmd(() =>
                    query.CountAsync(),
                    $"{nameof(IsCommentedByUser)} - {nameof(CommentRepository)}") > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Comment>> GetCommentsForContextIdPagedAsync(Guid contextId, int page, int pageSize, bool orderAcendingByDate = false)
        {
            SortDefinition <Comment> sortDefinition = Sort.Descending(e => e.Date);
            
            if (orderAcendingByDate)
            {
                sortDefinition = Sort.Ascending(e => e.Date);
            }

            return await ExecuteCmd(() =>
                    DefaultCollection
                        .Find(c => c.ContextId == contextId)
                        .Skip(page * pageSize)
                        .Limit(pageSize)
                        .Sort(sortDefinition)
                        .ToListAsync(),
                $"{nameof(GetCommentsForContextIdPagedAsync)} - {nameof(CommentRepository)}");
        }

        public async Task<IEnumerable<Comment>> GetCommentsForContextIdFromPeriodAsync(Guid contextId, TimeSpan daysBack)
        {
            return await ExecuteCmd(() =>
                DefaultCollection
                    .Find(
                        c => c.ContextId == contextId
                             && c.Date.CompareTo(new DateTime(DateTime.Now.Subtract(daysBack).Ticks)) > 0)
                    .ToListAsync(),
                $"{nameof(GetCommentsForContextIdFromPeriodAsync)} - {nameof(CommentRepository)}");
        }

        public async Task UpdateAuthorNameBatchAsync(Guid authorId, string authorName)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(c => c.AuthorId == authorId,
                    Update.Set(c => c.AuthorName, authorName)),
                $"{nameof(UpdateAuthorNameBatchAsync)} - {nameof(CommentRepository)}");
        }

        public async Task UpdateAuthorProfileImageIdBatchAsync(Guid authorId, Guid authorProfileImageId)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(c => c.AuthorId == authorId,
                    Update.Set(c => c.AuthorProfileImgId, authorProfileImageId)),
                $"{nameof(UpdateAuthorProfileImageIdBatchAsync)} - {nameof(CommentRepository)}");
        }
    }
}