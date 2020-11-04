using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Helpers;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.UserModels;
using CM.Backend.Queries.Queries.BrandQueries;
using CM.Backend.Queries.Queries.UserQueries;
using SimpleSoft.Mediator;
using CM.Backend.Queries.Queries.UserCreationQuerires;
using MongoDB.Driver;

namespace CM.Backend.Queries.Handlers
{
    public class UserQueryHandler :
    IQueryHandler<GetUser, UserQueryModel>,
    IQueryHandler<GetPublicUser, PublicUserQueryModel>,
    IQueryHandler<GetAllUsers, IEnumerable<UserQueryLight>>,
    IQueryHandler<GetUserFollowers, IEnumerable<FollowersQueryModel>>,
    IQueryHandler<GetUserFollowing, IEnumerable<FollowingQueryModel>>,
    IQueryHandler<GetUserBrandFollowing, IEnumerable<FollowingQueryModel>>,
    IQueryHandler<GetUserCellarPaged, IEnumerable<UserCellarChampagneQueryModel>>,
    IQueryHandler<GetUserCellarSavedPaged, IEnumerable<ChampagneLight>>,
    IQueryHandler<GetUserInfo, UserInfoQueryModel>,
    IQueryHandler<GetUserSettings, UserSettingsModel>,
    IQueryHandler<SearchUsersByUsername, IEnumerable<FollowingQueryModel>>,
    IQueryHandler<SearchUsersPagedAsync, IEnumerable<FollowingQueryModel>>,
    IQueryHandler<SearchUsersByProfilename, IEnumerable<FollowingQueryModel>>,
    IQueryHandler<GetPersistenceUserModel, User>,
	IQueryHandler<CheckUsernameAvailabillity, bool>,
	IQueryHandler<CheckEmailAvailabillity, bool>,
    IQueryHandler<SearchUsersByProfilenameAndUsernamePagedAsync, IEnumerable<UserSearchProjectionModel>>,
    IQueryHandler<SearchUsersPagedAsyncLight, IEnumerable<UserSearchProjectionModel>>
    {
        private readonly IUserRepository userRepository;
        private readonly IFollowRepository followRepository;
        private readonly IFollowBrandRepository followBrandRepository;
        private readonly IUserCellarRepository userCellarRepository;
        private readonly IChampagneRepository champagneRepository;
        private readonly IBrandRepository brandRepository;
        private readonly ITastingRepository tastingRepository;

        public UserQueryHandler(IUserRepository userRepository, IFollowRepository followRepository, IFollowBrandRepository followBrandRepository, IUserCellarRepository userCellarRepository, IChampagneRepository champagneRepository, IBrandRepository brandRepository, ITastingRepository tastingRepository)
        {
            this.userRepository = userRepository;
            this.followRepository = followRepository;
            this.followBrandRepository = followBrandRepository;
            this.userCellarRepository = userCellarRepository;
            this.champagneRepository = champagneRepository;
            this.brandRepository = brandRepository;
            this.tastingRepository = tastingRepository;
        }

        public async Task<UserQueryModel> HandleAsync(GetUser query, CancellationToken ct)
        {
            var result = await userRepository.GetById(query.UserId);

            if (result == null)
            {
                return null;
            }
            //Map into new object
            var userQueryModel = new UserQueryModel();
            userQueryModel.Id = result.Id;
            userQueryModel.Email = result.Email;
            userQueryModel.Name = result.Name;
            userQueryModel.ProfileName = result.ProfileName;
            userQueryModel.Biography = result.Biography;
            
            userQueryModel.BookmarkedChampagnes = result.BookmarkedChampagnes.Length;
            userQueryModel.TastedChampagnes = result.TastedChampagnes.Length;
            userQueryModel.Following = result.Following.Length;
            userQueryModel.Followers = await followRepository.CountFollowers(query.UserId);
            
            if(result.ImageCustomization != null)
            {
                userQueryModel.ImageCustomization = new UserQueryModel.UserImageCustomization
                {
                    ProfileCoverImgId = result.ImageCustomization.ProfileCoverImgId,
                    ProfilePictureImgId = result.ImageCustomization.ProfilePictureImgId,
                    CellarCardImgId = result.ImageCustomization.CellarCardImgId,
                    CellarHeaderImgId = result.ImageCustomization.CellarHeaderImgId
                };    
            }
            if(result.Settings != null)
            {
                userQueryModel.Settings = new UserQueryModel.UserSettings
                {
                    CountryCode = result.Settings.CountryCode,
                    Language = result.Settings.Language,
                    IsEmailVerified = result.Settings.IsEmailVerified
                };    
            }

            return userQueryModel;

        }

        public async Task<IEnumerable<UserQueryLight>> HandleAsync(GetAllUsers query, CancellationToken ct)
        {
            var result = await userRepository.GetUsersAscendingByDatePagedAsync(query.Page, query.PageSize);

            //Map into
            var newResultMap = new List<UserQueryLight>();
            
            foreach(User model in result)
            {
                var userQueryLight = new UserQueryLight();
                userQueryLight.Id = model.Id;
                userQueryLight.Name = model.Name;
                userQueryLight.ProfileName = model.ProfileName;
                if (model.ImageCustomization != null)
                {
                    userQueryLight.ProfilePictureImgId = model.ImageCustomization.ProfileCoverImgId;
                }
                newResultMap.Add(userQueryLight);
            }

            return newResultMap;
        }

        public async Task<PublicUserQueryModel> HandleAsync(GetPublicUser query, CancellationToken ct)
        {
            var result = await userRepository.GetById(query.UserId);

            if (result == null)
            {
                return null;
            }
            //Map into new object
            var userQueryModel = new PublicUserQueryModel();
            userQueryModel.Id = result.Id;
            userQueryModel.Email = result.Email;
            userQueryModel.Name = result.Name;
            userQueryModel.ProfileName = result.ProfileName;
            userQueryModel.Biography = result.Biography;
            
            userQueryModel.BookmarkedChampagnes = result.BookmarkedChampagnes.Length;
            userQueryModel.TastedChampagnes = result.TastedChampagnes.Length;
            userQueryModel.Following = result.Following.Length;
            userQueryModel.Followers = await followRepository.CountFollowers(query.UserId);
            
            if(result.ImageCustomization != null)
            {
                userQueryModel.imageCustomization = new PublicUserQueryModel.UserImageCustomization
                {
                    ProfileCoverImgId = result.ImageCustomization.ProfileCoverImgId,
                    ProfilePictureImgId = result.ImageCustomization.ProfilePictureImgId,
                    CellarCardImgId = result.ImageCustomization.CellarCardImgId,
                    CellarHeaderImgId = result.ImageCustomization.CellarHeaderImgId
                };    
            }
            
            //Check if the requester is following
            if (query.RequestingUserId == Guid.Empty)
            {
                userQueryModel.IsRequesterFollowing = false;
            }
            else
            {
                var follow = await followRepository.GetFollowByKey(new Follow.PrimaryKey
                {
                    FollowById = query.RequestingUserId,
                    FollowToId = query.UserId
                });

                if (follow != null)
                {
                    userQueryModel.IsRequesterFollowing = true;
                }
                else
                {
                    userQueryModel.IsRequesterFollowing = false;
                }
            }

            return userQueryModel;
        }

        public async Task<IEnumerable<FollowersQueryModel>> HandleAsync(GetUserFollowers query, CancellationToken ct)
        {
            var followers = await followRepository.GetFollowersOfUserId(query.UserId);

            var followersList = new List<Follow>(followers);
            
            //Since there were no followers return empty list
            if (followers.Count() < 1)
            {
                return new List<FollowersQueryModel>();
            }

            var pagedList = new List<Follow>();
            //Only return entities that matches the range specified in the query.
            for (int i = (query.Page * query.PageSize); i < followersList.Count(); i++)
            {
                if (i < (query.Page * query.PageSize) + query.PageSize)
                {
                    pagedList.Add(followersList[i]);
                }
            }

            return await ConvertUserFollowers(pagedList, query.ReqUserId);
        }

        public async Task<IEnumerable<FollowingQueryModel>> HandleAsync(GetUserFollowing query, CancellationToken ct)
        {
            var following = await followRepository.GetFollowingOfUserId(query.UserId);

            var followingList = new List<Follow>(following);

            if (following.Count() < 1)
            {
                return new List<FollowingQueryModel>();
            }

            var pagedList = new List<Follow>();
            //Only return entities that matches the range specified in the query.
            for (int i = (query.Page * query.PageSize); i < followingList.Count(); i++)
            {
                if(i < (query.Page * query.PageSize) + query.PageSize)
                {
                    pagedList.Add(followingList[i]);
                }
            }

            return await ConvertUserFollowing(pagedList, query.ReqUserId);

        }
        
        public async Task<IEnumerable<FollowingQueryModel>> HandleAsync(GetUserBrandFollowing query, CancellationToken ct)
        {
            var following = await followBrandRepository.GetFollowingFromUserId(query.UserId);

            var followingList = new List<FollowBrand>(following);

            if (following.Count() < 1)
            {
                return new List<FollowingQueryModel>();
            }

            var pagedList = new List<FollowBrand>();
            
            for (int i = (query.Page * query.PageSize); i < followingList.Count(); i++)
            {
                if(i < (query.Page * query.PageSize) + query.PageSize)
                {
                    pagedList.Add(followingList[i]);
                }
            }


            return await ConvertUserBrandFollowing(pagedList, query.ReqUserId);

        }

        private async Task<IEnumerable<FollowingQueryModel>> ConvertUserBrandFollowing(IEnumerable<FollowBrand> followBrands, Guid requesterId)
        {
            var convertedList = new List<FollowingQueryModel>();

            if (requesterId == Guid.Empty)
            {
                foreach (var followingBrand in followBrands)
                {
                    var followingQueryModel = new FollowingQueryModel
                    {
                        Id = followingBrand.Id,
                        FollowToId = followingBrand.FollowToBrandId,
                        FollowToName = followingBrand.FollowToBrandName,
                        FollowToProfileImg = followingBrand.FollowToBrandLogoImgId,
                        IsRequesterFollowing = false
                    };
                    convertedList.Add(followingQueryModel);
                }
            }
            else
            {
                foreach (var followingBrand in followBrands)
                {
                    var requesterFollowingEntity =
                        await followBrandRepository.FindFollowByKey(new FollowBrand.PrimaryKey
                        {
                            FollowByUserId = requesterId,
                            FollowToBrandId = followingBrand.FollowToBrandId
                        });
                    //OBJECT TO MAP INTO
                    var followingQueryModel = new FollowingQueryModel();

                    followingQueryModel.Id = followingBrand.Id;
                    followingQueryModel.FollowToId = followingBrand.FollowToBrandId;
                    followingQueryModel.FollowToName = followingBrand.FollowToBrandName;
                    followingQueryModel.FollowToProfileImg = followingBrand.FollowToBrandLogoImgId;
                    if (requesterFollowingEntity != null)
                    {
                        followingQueryModel.IsRequesterFollowing = true;
                    }
                    else
                    {
                        followingQueryModel.IsRequesterFollowing = false;
                    }
                    convertedList.Add(followingQueryModel);
                }
            }
           

            return convertedList;
        }
        
        /// <summary>
        /// Converts a list of follow entities into followingQueryModels. This is used when asking for the people which a specific user is following
        /// </summary>
        /// <param name="userFollowing"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<FollowingQueryModel>> ConvertUserFollowing(IEnumerable<Follow> userFollowing,
            Guid requesterId)
        {
            var convertedList = new List<FollowingQueryModel>();

            if (requesterId == Guid.Empty)
            {
                foreach (var followingUser in userFollowing)
                {
                    var followingQueryModel = new FollowingQueryModel
                    {
                        Id = followingUser.Id,
                        FollowToId = followingUser.FollowToId,
                        FollowToName = followingUser.FollowToName,
                        FollowToProfileImg = followingUser.FollowToProfileImgId,
                        IsRequesterFollowing = false
                    };
                    convertedList.Add(followingQueryModel);
                }
            }
            else
            {
                foreach (var followingUser in userFollowing)
                {
                    var requesterFollowingEntity =
                        await followRepository.GetFollowByKey(new Follow.PrimaryKey
                        {
                            FollowById = requesterId,
                            FollowToId = followingUser.FollowToId
                        });
                    //OBJECTS TO MAP INTO
                    var followingQueryModel = new FollowingQueryModel();

                    followingQueryModel.Id = followingUser.Id;
                    followingQueryModel.FollowToId = followingUser.FollowToId;
                    followingQueryModel.FollowToName = followingUser.FollowToName;
                    followingQueryModel.FollowToProfileImg = followingUser.FollowToProfileImgId;
                    if (requesterFollowingEntity != null)
                    {
                        followingQueryModel.IsRequesterFollowing = true;
                    }
                    else
                    {
                        followingQueryModel.IsRequesterFollowing = false;
                    }
                    convertedList.Add(followingQueryModel);
                }
            } 
            return convertedList;
        }

        /// <summary>
        /// Converts a list of follow entities to a list of followersQueryModels. This is used when asking for a specific users followers
        /// </summary>
        /// <param name="userFollowers"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<FollowersQueryModel>> ConvertUserFollowers(IEnumerable<Follow> userFollowers,
            Guid requesterId)
        {
            var convertedList = new List<FollowersQueryModel>();

            if (requesterId == Guid.Empty)
            {
                //Mapping
                foreach (var followUser in userFollowers)
                {
                    var followersQueryModel = new FollowersQueryModel
                    {
                        Id = followUser.Id,
                        FollowById = followUser.FollowById,
                        FollowByName = followUser.FollowByName,
                        FollowByProfileImgId = followUser.FollowByProfileImgId,
                        IsRequesterFollowing = false
                    };
                    convertedList.Add(followersQueryModel);
                }
            }
            else
            {
                //Foreach userFollower, check if the requester is following them or not
                foreach (var followUser in userFollowers)
                {
                    var requesterFollowingEntity =
                        await followRepository.GetFollowByKey(new Follow.PrimaryKey
                        {
                            FollowById = requesterId,
                            FollowToId = followUser.FollowById
                        });
                    //OBJECT TO MAP INTO
                    var followersQueryModel = new FollowersQueryModel();

                    followersQueryModel.Id = followUser.Id;
                    followersQueryModel.FollowById = followUser.FollowById;
                    followersQueryModel.FollowByName = followUser.FollowByName;
                    followersQueryModel.FollowByProfileImgId = followUser.FollowByProfileImgId;
                    if (requesterFollowingEntity != null)
                    {
                        followersQueryModel.IsRequesterFollowing = true;
                    }
                    else
                    {
                        followersQueryModel.IsRequesterFollowing = false;
                    }

                    convertedList.Add(followersQueryModel);
                }
            }

            return convertedList;
        }

        public async Task<IEnumerable<UserCellarChampagneQueryModel>> HandleAsync(GetUserCellarPaged query, CancellationToken ct)
        {
            var result = await userCellarRepository.GetUserCellarPaged(query.UserId, query.Page, query.PageSize);
            
            //Cache information
            var brandCache = new Dictionary<Guid, Brand>();

            var convertedList = new List<UserCellarChampagneQueryModel>();
                
            //Map into light champagne representation
            foreach (var tastedChampagne in result)
            {
                var userCellarChampagneQueryModel = new UserCellarChampagneQueryModel();

                userCellarChampagneQueryModel.Id = tastedChampagne.Id;
                userCellarChampagneQueryModel.UserId = tastedChampagne.UserId;
                userCellarChampagneQueryModel.TastingId = tastedChampagne.TastingId;
                userCellarChampagneQueryModel.ChampagneId = tastedChampagne.ChampagneId;
                
                //Check if brand information has been collected for 
                var champagne = await champagneRepository.GetById(tastedChampagne.ChampagneId);
                userCellarChampagneQueryModel.BottleName = champagne.BottleName;
                userCellarChampagneQueryModel.BottleImgId = champagne.BottleImgId;
                userCellarChampagneQueryModel.IsVintage = champagne.vintageInfo.IsVintage;
                userCellarChampagneQueryModel.Year = champagne.vintageInfo.Year;
                userCellarChampagneQueryModel.Dosage = champagne.champagneProfile.DisplayDosage;
                
                if (brandCache.ContainsKey(champagne.BrandId))
                {
                    var brand = brandCache[champagne.BrandId];

                    userCellarChampagneQueryModel.BrandId = champagne.BrandId;
                    userCellarChampagneQueryModel.BrandName = brand.Name;
                }
                else
                {
                    var brand = await brandRepository.GetById(champagne.BrandId);
                    brandCache.Add(brand.Id, brand);

                    userCellarChampagneQueryModel.BrandId = brand.Id;
                    userCellarChampagneQueryModel.BrandName = brand.Name;
                }

                var tasting = await tastingRepository.GetById(tastedChampagne.TastingId);

                if (tasting != null)
                {
                    userCellarChampagneQueryModel.PersonalRating = tasting.Rating;
                }
                
                convertedList.Add(userCellarChampagneQueryModel);

            }

            return convertedList;

        }

        public async Task<IEnumerable<ChampagneLight>> HandleAsync(GetUserCellarSavedPaged query, CancellationToken ct)
        {
            var user = await userRepository.GetById(query.UserId);
            
            //Create a page list from the requested user

            var searchList = new List<Guid>();

            for (int i = query.Page * query.PageSize; i < query.Page * query.PageSize + query.PageSize; i++)
            {
                if (!(user.BookmarkedChampagnes.Length - 1 < i))
                {
                    searchList.Add(user.BookmarkedChampagnes[i]);
                }
            }

            var bookmarkedChampagnes = await champagneRepository.GetChampagneByIdFromListAsync(searchList);

            var convertedList = new List<ChampagneLight>();
            var brandCache = new Dictionary<Guid, Brand>();
            
            //Map into ChampagneLight
            foreach (var champagne in bookmarkedChampagnes)
            {
                var champagneLight = new ChampagneLight();

                champagneLight.Id = champagne.Id;
                champagneLight.BottleName = champagne.BottleName;
                    
                if (brandCache.ContainsKey(champagne.BrandId))
                {
                    var brand = brandCache[champagne.BrandId];

                    champagneLight.BrandId = champagne.BrandId;
                    champagneLight.BrandName = brand.Name;
                }
                else
                {
                    var brand = await brandRepository.GetById(champagne.BrandId);
                    brandCache.Add(brand.Id, brand);

                    champagneLight.BrandId = brand.Id;
                    champagneLight.BrandName = brand.Name;
                }

                champagneLight.BottleImgId = champagne.BottleImgId;
                champagneLight.IsPublished = champagne.IsPublished;
                champagneLight.ChampagneRootId = Guid.Empty; //TODO : champagne.ChampagneRootId;
                champagneLight.NumberOfTastings = champagne.RateCount;
                champagneLight.RatingSumOfTastings = champagne.RateValue;
                    //MappingResources.CalculateRatingSum(champagne.RatingDictionary);
                champagneLight.GetVintageInfo = new ChampagneLight.VintageInfo
                {
                    IsVintage = champagne.vintageInfo.IsVintage,
                    Year = champagne.vintageInfo.Year
                };
                
                convertedList.Add(champagneLight);
            }

            return convertedList;
        }

        public async Task<UserInfoQueryModel> HandleAsync(GetUserInfo query, CancellationToken ct)
        {
            var user = await userRepository.GetById(query.UserId);
            
            
            var userInfo = new UserInfoQueryModel();

            if (user != null)
            {
                userInfo.Id = user.Id;
                userInfo.Name = user.Name;
                userInfo.ProfileName = user.ProfileName;
            }
            else
            {
                return null;
            }

            return userInfo;

        }

        public async Task<UserSettingsModel> HandleAsync(GetUserSettings query, CancellationToken ct)
        {
            var user = await userRepository.GetById(query.UserId);
            
            var userSettingsModel = new UserSettingsModel();

            userSettingsModel.Id = user.Id;
            userSettingsModel.Email = user.Email;
            userSettingsModel.Name = user.Name;
            userSettingsModel.ProfileName = user.ProfileName;
            userSettingsModel.Biography = user.Biography;

            if (user.Settings != null)
            {
                userSettingsModel.IsEmailVerified = user.Settings.IsEmailVerified;
            }
            
            if (user.ImageCustomization != null)
            {
                userSettingsModel.ImageCustomization = user.ImageCustomization;
            }

            if (user.Notifications != null)
            {
                userSettingsModel.NotificationSettings = user.Notifications;
            }
            else
            {
                userSettingsModel.NotificationSettings = new NotificationSettings();
            }

            return userSettingsModel;

        }

        public async Task<IEnumerable<FollowingQueryModel>> HandleAsync(SearchUsersByUsername query, CancellationToken ct)
        {
            var result = await userRepository.SearchUsersByUsername(query.Username, query.Page, query.PageSize);

            return await ConvertUserToFollowingQueryModel(result, query.RequestingUserId); 
        }

        public async Task<IEnumerable<FollowingQueryModel>> HandleAsync(SearchUsersPagedAsync query, CancellationToken ct)
        {
            var result = await userRepository.GetUsersAscendingByDatePagedAsync(query.Page, query.PageSize);
            
            return await ConvertUserToFollowingQueryModel(result, query.RequestingUserId); 
        }

        public async Task<IEnumerable<FollowingQueryModel>> HandleAsync(SearchUsersByProfilename query, CancellationToken ct)
        {
            var result = await userRepository.SearchusersByProfilename(query.Profilename, query.Page, query.PageSize);
            
            return await ConvertUserToFollowingQueryModel(result, query.RequestingUserId); 
        }

        private async Task<IEnumerable<FollowingQueryModel>> ConvertUserToFollowingQueryModel(
            IEnumerable<User> listToConvert, Guid reqUserId)
        {
            var convertedList = new List<FollowingQueryModel>();

            User requestingUser = null;

            if (!reqUserId.Equals(Guid.Empty))
            {
                requestingUser = await userRepository.GetById(reqUserId);
            }
            
            foreach (var user in listToConvert)
            {
                var userModel = new FollowingQueryModel();

                userModel.FollowToId = user.Id;
                userModel.FollowToName = user.Name;
                userModel.FollowToProfileImg = user.ImageCustomization.ProfilePictureImgId;
                if (requestingUser != null)
                {
                    if (requestingUser.Following.Contains(user.Id))
                    {
                        userModel.IsRequesterFollowing = true;
                    }
                }
                
                convertedList.Add(userModel);
            }

            return convertedList;
        }

        public async Task<User> HandleAsync(GetPersistenceUserModel query, CancellationToken ct)
        {
            return await userRepository.GetById(query.UserId);
        }

		public async Task<bool> HandleAsync(CheckEmailAvailabillity query, CancellationToken ct)
		{
			var result = await userRepository.GetUserByEmail(query.Email);

			if(result == null)
			{
				return true;
			}

			return false;
			
		}

		public async Task<bool> HandleAsync(CheckUsernameAvailabillity query, CancellationToken ct)
		{
			var result = await userRepository.GetUserByUsername(query.Username);

			if(result == null)
			{
				return true;
			}

			return false;
		}

        public async Task<IEnumerable<UserSearchProjectionModel>> HandleAsync(SearchUsersByProfilenameAndUsernamePagedAsync query, CancellationToken ct)
        {
            return
                await userRepository.SearchUsersByUsernameProfilenamePagedAsync(query.SearchText, query.Page,
                    query.PageSize);

        }

        public async Task<IEnumerable<UserSearchProjectionModel>> HandleAsync(SearchUsersPagedAsyncLight query, CancellationToken ct)
        {
            return
                await userRepository.GetAll(c =>
                        new UserSearchProjectionModel
                        {
                            Id = c.Id,
                            ImageId = c.ImageCustomization.ProfilePictureImgId,
                            Name = c.Name,
                            ProfileName = c.ProfileName
                        },
                    c => c.CreatedAt,
                    query.Page,
                    query.PageSize);
        }
    }
}
