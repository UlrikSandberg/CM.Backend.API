using System;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.UserList;
using CM.Backend.Persistence.Model.UserList.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IUserListRepository : IMongoReadmodelRepository<UserList>
    {
        Task EditUserList(Guid listId, string title, string subtitle, string descriptipn, Guid imageId);
        Task AddItem(Guid listId, Guid itemId);
        Task RemoveItem(Guid listId, Guid itemId);
        Task SetFeaturedSchedule(Guid listId, FeaturedSchedule featuredSchedule);

        Task UpdateUserListType(Guid listId, string listType);

        Task UpdateItemIndex(Guid listId, Guid itemId, int itemIndex);
    }
    
    public class UserListRepository : MongoReadmodelRepository<UserList>, IUserListRepository
    {
        public UserListRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task EditUserList(Guid listId, string title, string subtitle, string descriptipn, Guid imageId)
        {
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .Set(u => u.Title, title)
                        .Set(u => u.Subtitle, subtitle)
                        .Set(u => u.Description, descriptipn)
                        .Set(u => u.ImageId, imageId)),
                $"{nameof(EditUserList)} - {nameof(UserListRepository)}");
        }

        public async Task AddItem(Guid listId, Guid itemId)
        {
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .AddToSet(u => u.ListContent, itemId)),
                $"{nameof(AddItem)} - {nameof(UserListRepository)}");
        }

        public async Task RemoveItem(Guid listId, Guid itemId)
        {
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .Pull(u => u.ListContent, itemId)),
                $"{nameof(RemoveItem)} - {nameof(UserListRepository)}");
        }

        public async Task SetFeaturedSchedule(Guid listId, FeaturedSchedule featuredSchedule)
        {
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .Set(u => u.FeaturedSchedule, featuredSchedule)),
                $"{nameof(SetFeaturedSchedule)} - {nameof(UserListRepository)}");
        }

        public async Task UpdateUserListType(Guid listId, string listType)
        {
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .Set(u => u.ListType, listType)),
                $"{nameof(UpdateUserListType)} - {nameof(UserListRepository)}");
        }

        public async Task UpdateItemIndex(Guid listId, Guid itemId, int itemIndex)
        {
            var userList = await ExecuteCmd(() => 
                    DefaultCollection.Find(u => u.Id == listId).SingleOrDefaultAsync(),
                $"{nameof(UpdateItemIndex)} - {nameof(UserListRepository)}");
            
            //Sort the list, with new itemIndex and insert again
            
            //Check if the key is present in the array 
            var key = userList.ListContent.SingleOrDefault(x => x.Equals(itemId));
            if (key == null)
            {
                return;
            }
            
            //userList content as arr
            var list = userList.ListContent.ToArray();
            
            //Key was present
            var keyIndex = userList.ListContent.IndexOf(itemId);
            Guid temp = Guid.Empty;
            
            if (keyIndex > itemIndex) //Forwards linear scan from newIndex and swap too previous position
            {
                for (int i = itemIndex; i < list.Length; i++)
                {
                    if (list[i].Equals(itemId))
                    {
                        list[i] = key;
                        break;
                    }

                    temp = list[i];
                    list[i] = key;
                    key = temp;
                }
            }
            else //Backwards linear scan
            {
                for (int i = itemIndex; i >= 0; i--)
                {
                    if (list[i].Equals(itemId))
                    {
                        list[i] = key;
                        break;
                    }

                    temp = list[i];
                    list[i] = key;
                    key = temp;
                }
            }
            
            //Insert new ordered list
            await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndUpdateAsync(u => u.Id == listId, Update
                        .Set(u => u.ListContent, list.ToList())),
                $"{nameof(UpdateItemIndex)} - {nameof(UserListRepository)}");
        }
    }
}