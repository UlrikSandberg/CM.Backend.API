using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.RatingModel;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.HelperQueries;
using CM.Backend.Queries.Queries.HelperQueries;
using CM.Backend.Queries.Queries.HelperQueries.HelperModels;
using Microsoft.Extensions.Caching.Memory;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class HelperHandler : 
        IQueryHandler<GetSubmittedFeedbackQuery, IEnumerable<BugAndFeedback>>,
        IQueryHandler<GetEmailList, IEnumerable<string>>,
        IQueryHandler<GetEmailListVerbose, IEnumerable<VerboseEmailListModel>>,
        IQueryHandler<MigrateChampagneRatings, IEnumerable<Guid>>,
        IQueryHandler<UpdateChampagneRatingStatus, bool>,
        IQueryHandler<UpdateChampagneFolderWithDiscoverVisibilityBoolean, bool>,
        IQueryHandler<ClearInMemoryCache, bool>
    {
        private readonly IBugAndFeedbackRepository _bugAndFeedbackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChampagneRepository _champagneRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly ITastingRepository _tastingRepository;
        private readonly IChampagneFolderRepository _champagneFolderRepository;
        private readonly IMemoryCache _memoryCache;

        
        public HelperHandler(IBugAndFeedbackRepository bugAndFeedbackRepository, IUserRepository userRepository, IChampagneRepository champagneRepository, IRatingRepository ratingRepository, ITastingRepository tastingRepository, IChampagneFolderRepository champagneFolderRepository, IMemoryCache memoryCache)
        {
            _bugAndFeedbackRepository = bugAndFeedbackRepository;
            _userRepository = userRepository;
            _champagneRepository = champagneRepository;
            _ratingRepository = ratingRepository;
            _tastingRepository = tastingRepository;
            _champagneFolderRepository = champagneFolderRepository;
            _memoryCache = memoryCache;
        }
        
        
        public async Task<IEnumerable<BugAndFeedback>> HandleAsync(GetSubmittedFeedbackQuery query, CancellationToken ct)
        {
            return await _bugAndFeedbackRepository.GetSubmittedBugAndFeedback(query.Page, query.PageSize,
                query.FromDate, query.ToDate);
        }

        public async Task<IEnumerable<string>> HandleAsync(GetEmailList query, CancellationToken ct)
        {
            return await _userRepository.GetAll(
                c => c.Email,
                c => c.CreatedAt.CompareTo(query.FromDate) > 0 && c.CreatedAt.CompareTo(query.ToDate) < 0, 
                query.Page,
                query.PageSize);
        }

        public async Task<IEnumerable<VerboseEmailListModel>> HandleAsync(GetEmailListVerbose query, CancellationToken ct)
        {
            return await _userRepository.GetAll(c => new VerboseEmailListModel
            {
                CreatedAt = c.CreatedAt,
                Email = c.Email,
                Id = c.Id,
                Username = c.Name
            }, c => c.CreatedAt.CompareTo(query.FromDate) > 0 && c.CreatedAt.CompareTo(query.ToDate) < 0,
                query.Page,
                query.PageSize);
        }

        public async Task<IEnumerable<Guid>> HandleAsync(MigrateChampagneRatings query, CancellationToken ct)
        {
            //Prep - Data
            var result = await _champagneRepository.GetPaged(0, 1000);

            var tastingIds = new List<Guid>();
            result.ToList().ForEach(x => x.RatingDictionary.Keys.ToList().ForEach(y => tastingIds.Add(Guid.Parse(y))));
                        
            //Fetch all the userIds respective to the tastingsIds
            var tastings = await _tastingRepository.GetAll(p => new
            {
                tastingId = p.Id,
                authorId = p.AuthorId,
                RatingValue = p.Rating
            }, f => tastingIds.Contains(f.Id), 0, 100000);

            var distictGuids = new List<Guid>();

            foreach (var ch in tastingIds)
            {
                var isDistinct = true;
                foreach (var VARIABLE in tastings)
                {
                    if (VARIABLE.tastingId.Equals(ch))
                    {
                        isDistinct = false;
                        break;
                    }
                }
                
                if(isDistinct)
                    distictGuids.Add(ch);
            }
            
            //Update ratingRepository with all the stored ratings under each respective champagne
            foreach (var champagne in result)
            {
                foreach (var dic in champagne.RatingDictionary)
                {
                    if(distictGuids.Contains(Guid.Parse(dic.Key)))
                        continue;
                    
                    await UpdateRatingModel(
                        champagne.Id,
                        tastings.SingleOrDefault(x => x.tastingId.Equals(Guid.Parse(dic.Key))).authorId,
                        Guid.Parse(dic.Key),
                        "Champagne",
                        dic.Value);
                }
            }
            
            return distictGuids;
        }

        private int index = 0;
        private async Task UpdateRatingModel(Guid champagneId, Guid authorId, Guid contextId, string type, double ratingValue)
        {
            //The key should be consist of a combined key from both the Id of the thing rated as well as the UserId.¨
            //Check if the tasting has already been migrated

            var result = await _ratingRepository.GetByKey(new RatingModel.PrimaryKey
            {
                UserId = authorId,
                EntityId = champagneId
            });

            if (result == null)
            {
                await _ratingRepository.Insert(new RatingModel
                {
                    Id = champagneId,
                    Key = new RatingModel.PrimaryKey {EntityId = champagneId, UserId = authorId},
                    ContextId = contextId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    Rating = ratingValue,
                    Type = type,
                    UserId = authorId
                });
            }
            else
            {
                Console.WriteLine("Champagne was already migrated");
            }

            index++;
            Console.WriteLine($"Created {index} ratingModels");
        }

        public async Task<bool> HandleAsync(UpdateChampagneRatingStatus query, CancellationToken ct)
        {
            var updatedChampagnes = 0;
            
            var result = await _champagneRepository.GetPaged(0, 1000);

            if (query.UpdateStatus)
            {
                foreach (var champagne in result)
                {
                    if (champagne.IsUpdated)
                    {
                        continue;
                    }
                    
                    var newUpdate = await _ratingRepository.GetEntityAverageRatingAndCount(champagne.Id);
                    if (newUpdate == null)
                    {
                        if (champagne.RatingDictionary.Count > 1)
                        {
                            await _champagneRepository.UpdateChampagneAverageRatingAndRatingCount(champagne.Id,
                                0,
                                0,
                                0);
                        }
                        
                        await _champagneRepository.SetChampagneRatingUpdateStatus(champagne.Id, query.UpdateStatus);
                        updatedChampagnes++;
                        Console.WriteLine($"Update Champagnes: {updatedChampagnes}");
                        continue;
                    }

                    await _champagneRepository.UpdateChampagneAverageRatingAndRatingCount(champagne.Id,
                        newUpdate.AverageRating,
                        newUpdate.RatingCount, newUpdate.RatingValue);
                    await _champagneRepository.SetChampagneRatingUpdateStatus(champagne.Id, query.UpdateStatus);
                    updatedChampagnes++;
                    Console.WriteLine($"Update Champagnes: {updatedChampagnes}");
                }
            }
            else
            {
                foreach (var champagne in result)
                {
                    await _champagneRepository.SetChampagneRatingUpdateStatus(champagne.Id, query.UpdateStatus);
                    updatedChampagnes++;
                    Console.WriteLine($"Update Champagnes: {updatedChampagnes}");
                }
            }

            return true;
        }

        public async Task<bool> HandleAsync(UpdateChampagneFolderWithDiscoverVisibilityBoolean query, CancellationToken ct)
        {
            var folders = await _champagneFolderRepository.GetAll(0, 1000);
            
            //Update each folder

            await _champagneFolderRepository.SetDiscoverVisibilityForIds(new HashSet<Guid>(folders.Select(x => x.Id)),
                true);

            return true;
        }

        public async Task<bool> HandleAsync(ClearInMemoryCache query, CancellationToken ct)
        {
            foreach (var cmd in query.Cmds)
            {
                _memoryCache.Remove(cmd);
            }

            return true;
        }
    }
}