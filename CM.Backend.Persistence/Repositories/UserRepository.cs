using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Helpers;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{

	public interface IUserRepository : IMongoReadmodelRepository<User>
	{
		Task EditUserImageCustomization(Guid userId, ImageCustomization imageCustomization);
		Task UpdateProfileCover(Guid userId, Guid profileCoverImgId);
		Task UpdateProfileImg(Guid userId, Guid profileImgId);
		Task UpdateCellarCardImg(Guid userId, Guid cellarCardImgId);
		Task UpdateCellarHeaderImg(Guid userId, Guid cellarHeaderImgId);
		Task UpdateUserSettings(Guid userId, UserSettings userSettings);
		Task UpdateUserInfo(Guid userId, string name, string profileName, string biography);
		Task UpdateEmail(Guid userId, string email, UserSettings settings);
		Task UpdateIsEmailVerified(Guid userId, UserSettings settings);
		Task FollowUser(Guid followById, Guid followToId);
		Task UnfollowUser(Guid followById, Guid followToId);
		Task FollowBrand(Guid userId, Guid brandId);
		Task UnfollowBrand(Guid userId, Guid brandId);
		Task BookmarkChampagne(Guid userId, Guid champagneId);
		Task UnbookmarkChampagne(Guid userId, Guid champagneId);
		Task<bool> ChampagneIsBookmarkedByUser(Guid userId, Guid champagneId);
		Task<bool> ChampagneIsTastedByUser(Guid userId, Guid champagneId);
		Task AddTasting(Guid userId, Guid champagneId);
		Task RemoveTasting(Guid userId, Guid champagneId);
		Task AddNotificationSettings(Guid userId, NotificationSettings notificationSettings);
		Task<IEnumerable<User>> SearchUsersByUsername(string username, int page, int pageSize);
		Task<IEnumerable<User>> SearchusersByProfilename(string profilename, int page, int pageSize);
		Task UpdateUserNotificationSettings(Guid userId, NotificationSettings updatedSettings);
		Task<IEnumerable<User>> GetEligibleUsersForNotificationAsync(HashSet<Guid> receivers, NotificationMethod notificationType);
		Task RegisterDeviceInstallation(Guid userId, Installation deviceInstallation);
		Task DeregisterDeviceInstallation(Guid userId, string pushChannel);
		Task AddNotificationForSpecificUsersAsync(HashSet<Guid> userIds, Guid notificationId);
		Task AddNotificationForAllUsers(Guid notificationId);
		Task AddNotificationForBrandFollowers(Guid brandId, Guid notificationId);
		Task AddNotificationForUserFollowers(Guid followingUserId, Guid notificationId);
		Task MarkNotificationAsRead(Guid userId, Guid notificationId);
		Task MarkLatestNotificationSeen(Guid userId, Guid notificationId);
		Task UpdateSleepSettings(Guid userId, SleepSettings sleepSettings);
		Task<User> GetUserByUsername(string username);
		Task<User> GetUserByEmail(string email);
		Task<IEnumerable<User>> GetUsersAscendingByDatePagedAsync(int page, int pageSize);
		Task<IEnumerable<UserSearchProjectionModel>> SearchUsersByUsernameProfilenamePagedAsync(string searchText, int page, int pageSize);
		
		

	}

	public class UserRepository : MongoReadmodelRepository<User>, IUserRepository
	{
		public UserRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task AddUserImageCustomization(Guid userId, ImageCustomization imageCustomization)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.ImageCustomization, imageCustomization)),
				$"{nameof(AddUserImageCustomization)} - {nameof(UserRepository)}");
		}

		public async Task EditUserImageCustomization(Guid userId, ImageCustomization imageCustomization)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.ImageCustomization, imageCustomization)),
				$"{nameof(EditUserImageCustomization)} - {nameof(UserRepository)}");
		}

		public async Task AddUserSettings(Guid userId, UserSettings userSettings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update.Set(u => u.Settings, userSettings)),
				$"{nameof(AddUserSettings)} - {nameof(UserRepository)}");
		}

		public async Task BookmarkChampagne(Guid userId, Guid champagneId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.AddToSet(c => c.BookmarkedChampagnes, champagneId)),
				$"{nameof(BookmarkChampagne)} - {nameof(UserRepository)}");
		}

        /// <summary>
        /// Follows the brand. Not implemented because the FollowBrandRepository is used instead
        /// </summary>
        /// <returns>The brand.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="brandId">Brand identifier.</param>
		public async Task FollowBrand(Guid userId, Guid brandId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update.AddToSet(f => f.FollowingBrands, brandId)),
				$"{nameof(FollowBrand)} - {nameof(UserRepository)}");
		}

		public async Task UpdateEmail(Guid userId, string email, UserSettings settings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update
					.Set(u => u.Email, email)
					.Set(u => u.Settings, settings)),
				$"{nameof(UpdateEmail)} - {nameof(UserRepository)}");
		}

		public async Task UpdateIsEmailVerified(Guid userId, UserSettings settings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update.Set(u => u.Settings, settings)),
				$"{nameof(UpdateIsEmailVerified)} - {nameof(UserRepository)}");
		}

		public async Task FollowUser(Guid followById, Guid followToId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == followById,
					Update.AddToSet(f => f.Following, followToId)),
				$"{nameof(FollowUser)} - {nameof(UserRepository)}");
		}

		public async Task UnbookmarkChampagne(Guid userId, Guid champagneId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId,
					Update.Pull(x => x.BookmarkedChampagnes, champagneId)),
				$"{nameof(UnbookmarkChampagne)} - {nameof(UserRepository)}");
		}

		public async Task<bool> ChampagneIsBookmarkedByUser(Guid userId, Guid champagneId)
		{
			var user = await ExecuteCmd(() =>
				DefaultCollection.Find(u => u.Id == userId).SingleOrDefaultAsync(),
				$"{nameof(ChampagneIsBookmarkedByUser)} - {nameof(UserRepository)}");

			if (user == null)
			{
				return false;
			}

			return user.BookmarkedChampagnes.Contains(champagneId);
		}

		public async Task<bool> ChampagneIsTastedByUser(Guid userId, Guid champagneId)
		{
			var user = await ExecuteCmd(() =>
				DefaultCollection.Find(u => u.Id == userId).SingleOrDefaultAsync(),
				$"{nameof(ChampagneIsTastedByUser)} - {nameof(UserRepository)}");

			if (user == null)
			{
				return false;
			}

			return user.TastedChampagnes.Contains(champagneId);
		}

		public async Task AddTasting(Guid userId, Guid champagneId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.AddToSet(c => c.TastedChampagnes, champagneId)),
				$"{nameof(AddTasting)} - {nameof(UserRepository)}");
		}

		public async Task RemoveTasting(Guid userId, Guid champagneId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId, Update.Pull(x => x.TastedChampagnes, champagneId)),
				$"{nameof(RemoveTasting)} - {nameof(UserRepository)}");
		}

		public async Task AddNotificationSettings(Guid userId, NotificationSettings notificationSettings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.Notifications, notificationSettings)),
				$"{nameof(AddNotificationSettings)} - {nameof(UserRepository)}");
		}

		public async Task<IEnumerable<User>> SearchUsersByUsername(string username, int page, int pageSize)
		{
			var filter = new BsonDocument {{"name", new BsonDocument {{"$regex", username}, {"$options", "i"}}}};

			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(filter)
					.Skip(page * pageSize)
					.Limit(pageSize)
					.ToListAsync(),
				$"{nameof(SearchUsersByUsername)} - {nameof(UserRepository)}");

		}

		public async Task<IEnumerable<User>> SearchusersByProfilename(string profilename, int page, int pageSize)
		{
			var filter = new BsonDocument {{"profileName", new BsonDocument {{"$regex", profilename}, {"$options", "i"}}}};

			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(filter)
					.Skip(page * pageSize)
					.Limit(pageSize)
					.ToListAsync(),
				$"{nameof(SearchusersByProfilename)} - {nameof(UserRepository)}");
		}

		public async Task UpdateUserNotificationSettings(Guid userId, NotificationSettings updatedSettings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update
					.Set(u => u.Notifications, updatedSettings)),
				$"{nameof(UpdateUserNotificationSettings)} - {nameof(UserRepository)}");
		}

		public async Task<IEnumerable<User>> GetEligibleUsersForNotificationAsync(HashSet<Guid> receivers, NotificationMethod notificationType)
		{
			Expression<Func<User, bool>> p = null;
			
			if (notificationType.Equals(NotificationMethod.UserFollowed))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyNewFollower;
			}

			if (notificationType.Equals(NotificationMethod.TastingCommented))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyNewComment;
			}

			if (notificationType.Equals(NotificationMethod.TastingLiked))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyLikeTasting;
			}

			if (notificationType.Equals(NotificationMethod.CommentLiked))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyLikeComment;
			}

			if (notificationType.Equals(NotificationMethod.ChampagneTasted))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyChampagneTasted;
			}

			if (notificationType.Equals(NotificationMethod.ActivityInCommentThread))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyActivityInThread;
			}

			if (notificationType.Equals(NotificationMethod.BrandUpdate))
			{
				p = u =>
					receivers.Contains(u.Id)
					&& u.Notifications != null
					&& u.Notifications.NotifyBrandNews;
			}
			
			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(p)
					.ToListAsync(),
				$"{nameof(GetEligibleUsersForNotificationAsync)} - {nameof(UserRepository)}");
		}

		public async Task RegisterDeviceInstallation(Guid userId, Installation deviceInstallation)
		{
			var user = await base.GetById(userId);

			if (user != null)
			{
				HashSet<Installation> deviceInstallations = new HashSet<Installation>(new DeviceInstallationEqualityComparer());
				if (user.DeviceInstallations != null)
				{
					foreach (var installation in user.DeviceInstallations)
					{
						if (!installation.PushChannel.Equals(deviceInstallation.PushChannel))
						{
							deviceInstallations.Add(installation);
						}
					}
				}

				deviceInstallations.Add(deviceInstallation);

				await ExecuteCmd(() =>
					DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
						Update.Set(u => u.DeviceInstallations, deviceInstallations)),
					$"{nameof(RegisterDeviceInstallation)} - {nameof(UserRepository)}");
			}
		}

		public async Task DeregisterDeviceInstallation(Guid userId, string pushChannel)
		{
			var user = await base.GetById(userId);

			if (user != null)
			{
				HashSet<Installation> deviceInstallations = null;
				if (user.DeviceInstallations != null)
				{
					deviceInstallations = user.DeviceInstallations;
					deviceInstallations.RemoveWhere(x => x.PushChannel == pushChannel);
				}
				else
				{
					deviceInstallations = new HashSet<Installation>();
				}

				await ExecuteCmd(() =>
					DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
						Update.Set(u => u.DeviceInstallations, deviceInstallations)),
					$"{nameof(DeregisterDeviceInstallation)} - {nameof(UserRepository)}");
			}
		}

		//Specify a list of users whom should get the respective notification
		public async Task AddNotificationForSpecificUsersAsync(HashSet<Guid> userIds, Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(u => userIds.Contains(u.Id),
					Update.AddToSet(u => u.AvailableNotifications, notificationId)),
				$"{nameof(AddNotificationForSpecificUsersAsync)} - {nameof(UserRepository)}");
		}

		public async Task AddNotificationForAllUsers(Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(new BsonDocument(), 
					Update.AddToSet(u => u.AvailableNotifications, notificationId)),
				$"{nameof(AddNotificationForAllUsers)} - {nameof(UserRepository)}");
		}

		public async Task AddNotificationForBrandFollowers(Guid brandId, Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(u => u.FollowingBrands.Contains(brandId),
					Update.AddToSet(u => u.AvailableNotifications, notificationId)),
				$"{nameof(AddNotificationForBrandFollowers)} - {nameof(UserRepository)}");
		}

		public async Task AddNotificationForUserFollowers(Guid followingUserId, Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(u => u.Following.Contains(followingUserId),
					Update.AddToSet(u => u.AvailableNotifications, notificationId)),
				$"{nameof(AddNotificationForUserFollowers)} - {nameof(UserRepository)}");
		}

		public async Task MarkNotificationAsRead(Guid userId, Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId,
					Update.AddToSet(u => u.ReadNotifications, notificationId)),
				$"{nameof(MarkNotificationAsRead)} - {nameof(UserRepository)}");
		}

		public async Task MarkLatestNotificationSeen(Guid userId, Guid notificationId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId,
					Update.Set(u => u.LastNotificationSeen, notificationId)),
				$"{nameof(MarkLatestNotificationSeen)} - {nameof(UserRepository)}");
		}

		public async Task UpdateSleepSettings(Guid userId, SleepSettings sleepSettings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId,
					Update.Set(u => u.SleepSettings, sleepSettings)),
				$"{nameof(UpdateSleepSettings)} - {nameof(UserRepository)}");
		}

		/// <summary>
        /// Unfollows the brand. Not implemented because followbrandrepository is used instead
        /// </summary>
        /// <returns>The brand.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="brandId">Brand identifier.</param>
		public async Task UnfollowBrand(Guid userId, Guid brandId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == userId, Update.Pull(x => x.FollowingBrands, brandId)),
				$"{nameof(UnfollowBrand)} - {nameof(UserRepository)}");
		}

		public async Task UnfollowUser(Guid followById, Guid followToId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(u => u.Id == followById, Update.Pull(x => x.Following, followToId)),
				$"{nameof(UnfollowUser)} - {nameof(UserRepository)}");
		}

		public async Task UpdateCellarCardImg(Guid userId, Guid cellarCardImgId)
		{
			var user = await base.GetById(userId);

            var images = user.ImageCustomization;

			images.CellarCardImgId = cellarCardImgId;

			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update.Set(u => u.ImageCustomization, images)),
				$"{nameof(UpdateCellarCardImg)} - {nameof(UserRepository)}");
		}

		public async Task UpdateCellarHeaderImg(Guid userId, Guid cellarHeaderImgId)
		{
			var user = await base.GetById(userId);

            var images = user.ImageCustomization;

			images.CellarHeaderImgId = cellarHeaderImgId;

			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.ImageCustomization, images)),
				$"{nameof(UpdateCellarHeaderImg)} - {nameof(UserRepository)}");
		}

		public async Task UpdateProfileCover(Guid userId, Guid profileCoverImgId)
		{
			var user = await base.GetById(userId);

			var images = user.ImageCustomization;

			images.ProfileCoverImgId = profileCoverImgId;

			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.ImageCustomization, images)),
				$"{nameof(UpdateProfileCover)} - {nameof(UserRepository)}");
		}

		public async Task UpdateProfileImg(Guid userId, Guid profileImgId)
		{
			var user = await base.GetById(userId);

            var images = user.ImageCustomization;

			images.ProfilePictureImgId = profileImgId;

			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId,
					Update.Set(u => u.ImageCustomization, images)),
				$"{nameof(UpdateProfileImg)} - {nameof(UserRepository)}");
		}

		public async Task UpdateUserInfo(Guid userId, string name, string profileName, string biography)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update
					.Set(u => u.Name, name)
					.Set(u => u.ProfileName, profileName)
					.Set(u => u.Biography, biography)),
				$"{nameof(UpdateUserInfo)} - {nameof(UserRepository)}");
		}

		public async Task UpdateUserSettings(Guid userId, UserSettings userSettings)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(u => u.Id == userId, Update
					.Set(u => u.Settings, userSettings)),
				$"{nameof(UpdateUserSettings)} - {nameof(UserRepository)}");
		}

		public async Task<User> GetUserByUsername(string username)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(u => u.Name == username).SingleOrDefaultAsync(),
				$"{nameof(GetUserByUsername)} - {nameof(UserRepository)}");
		}

		public async Task<User> GetUserByEmail(string email)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(u => u.Email == email).SingleOrDefaultAsync(),
				$"{nameof(GetUserByEmail)} - {nameof(UserRepository)}");
		}

		public async Task<IEnumerable<User>> GetUsersAscendingByDatePagedAsync(int page, int pageSize)
		{	
			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(Filter.Empty)
					.Sort(Sort.Descending(u => u.CreatedAt))
					.Skip(page * pageSize)
					.Limit(pageSize)
					.ToListAsync(),
				$"{nameof(GetUsersAscendingByDatePagedAsync)} - {nameof(UserRepository)}");
		}

		public async Task<IEnumerable<UserSearchProjectionModel>> SearchUsersByUsernameProfilenamePagedAsync(string searchText, int page, int pageSize)
		{
			var f1 = new BsonDocument {{"name", new BsonDocument {{"$regex", searchText}, {"$options", "i"}}}};
			var f2 = new BsonDocument {{"profileName", new BsonDocument {{"$regex", searchText}, {"$options", "i"}}}};

			return await ExecuteCmd(() =>
					DefaultCollection
						.Find(Filter.Or(f1, f2))
						.Skip(page * pageSize)
						.Limit(pageSize)
						.Project(c => new UserSearchProjectionModel
						{
							Id = c.Id,
							ImageId = c.ImageCustomization.ProfilePictureImgId,
							Name = c.Name,
							ProfileName = c.ProfileName
						})
						.ToListAsync(),
				$"{nameof(SearchUsersByUsernameProfilenamePagedAsync)} - {nameof(UserRepository)}");
		}
	}
}
