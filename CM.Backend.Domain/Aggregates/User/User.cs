using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Aggregates.User.Commands;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User
{
	public class User : Aggregate
	{
		//Data 
		public Email Email { get; set; }
		public Name Name { get; set; } //This value would be useful for facebook integration, when extracting data from the GRAPH-API
		public string ProfileName { get; set; }
		public string Biography { get; set; }

		//List of saved champagnes
		public HashSet<AggregateId> BookmarkedChampagnes { get; set; }

		//List of people whom this user follows
		public HashSet<AggregateId> Following { get; set; }

		//List of brand whom this user follows
		public HashSet<AggregateId> FollowingBrands { get; set; }
		
		//List of tastedChampagnes needed so we can check if a duplicate tasting is incomming.
		public HashSet<AggregateId> TastedChampagnes { get; set; }
		
		//List of likedEntities
		public HashSet<AggregateId> LikedEntities { get; set; }

		//Object to hold user image customizations such as coverImg, profileImg etc...
		public UserImageCustomization ImageCustomization { get; set; }

		//This objects is supposed to hold other objects for various settings purposes such as Social media, notifications and so forth.
		public UserSettings UserSettings { get; set; }

		//Object to hold various meta data for each user, such as gender, age, locality etc...
		//public UserMetaData MetaData { get; set; }

		//Which access rights should a user have. Each access rights consists of a role(Free, payment) and a set of rights from 1-64
		//public AccessRights AccessRights { get; set; }

		//Taste profile, holds favorite champagne, brand, style etc -> Future this could hold auto generated profile based on user activity.
		//public UserTasteProfile TasteProfile { get; set; }
		public Notifications NotificationSettings { get; set; }
		
		public HashSet<DeviceInstallation> DeviceInstallations { get; set; }
		
		public HashSet<AggregateId> ReadNotifications { get; set; }
		
		public AggregateId LatestNotificationSeen { get; set; }

		public bool IsDeleted { get; set; }
		
		public MigrationSource MigrationSource { get; set; }
		
		public User()
		{
		}

		public void Execute(CreateUser cmd)
		{
			RaiseEvent(new UserCreated(cmd.Id.Value, cmd.Email, cmd.Name, cmd.ProfileName, cmd.Biography, cmd.BookmarkedChampagnes, cmd.Following, cmd.FollowingBrands, cmd.TastedChampagnes, cmd.LikedEntities, cmd.SleepSettings, cmd.ProfilePictureImgId, cmd.ProfileCoverImgId, cmd.CellarCardImgId, cmd.CellarHeaderImgId, cmd.CountryCode, cmd.LanguageCode, cmd.IsEmailVerified));
		}

		public void Execute(EditUserInfo cmd)
		{
			var didNameChange = !cmd.Name.Value.Equals(Name.Value);
			RaiseEvent(new UserInfoEdited(Id, cmd.Name, cmd.ProfileName, cmd.Biography, didNameChange));
		}
		
		public void Execute(UpdateProfileCover cmd)
		{
			if(ImageCustomization == null)
			{
				throw DomainException.CausedBy("Can't add userImageCustomization to non existing imageCustomiztion object");
			}

			RaiseEvent(new ProfileCoverUpdated(Id, cmd.ProfileCoverImgId));
		}

		public void Execute(UpdateProfileImg cmd)
		{
			if (ImageCustomization == null)
            {
                throw DomainException.CausedBy("Can't add userImageCustomization to non existing imageCustomiztion object");
            }
            
			RaiseEvent(new ProfileImgUpdated(Id, cmd.ProfileImgId));
		}

		public void Execute(UpdateCellarCardImg cmd)
		{
			if (ImageCustomization == null)
            {
                throw DomainException.CausedBy("Can't add userImageCustomization to non existing imageCustomiztion object");
            }

			RaiseEvent(new CellarCardImgUpdated(Id, cmd.CellarCardImgId));
		}

        public void Execute(UpdateCellarHeaderImg cmd)
		{
			if (ImageCustomization == null)
            {
                throw DomainException.CausedBy("Can't add userImageCustomization to non existing imageCustomiztion object");
            }

			RaiseEvent(new CellarHeaderImgUpdated(Id, cmd.CellarHeaderImgId));
		}

		public void Execute(EditUserSettings cmd)
		{
			if(UserSettings == null)
			{
				throw DomainException.CausedBy("Add user settings object before editing");
			}

			RaiseEvent(new UserSettingsEdited(Id, cmd.CountryCode, cmd.Language));
		}

		public void Execute(AddNotificationsSettings cmd)
		{
			if (NotificationSettings != null)
			{
				throw DomainException.CausedBy("Notification settings already exists");
			}
			
			RaiseEvent(new NotificationSettingsAdded(Id, cmd.ReceivedCMNotifications, cmd.NotifyNewFollower, cmd.NotifyNewComment, cmd.NotifyActivityInThread, cmd.NotifyLikeTasting, cmd.NotifyLikeComment, cmd.ReceiveNewsLetter, cmd.NotifyChampagneTasted, cmd.NotifyBrandNews));
		}

		public void Execute(UpdateUserNotificationSettings cmd)
		{
			RaiseEvent(new UserNotificationSettingsUpdated(Id, cmd.ReceiveCmNotifications, cmd.NotifyNewFollower, cmd.NotifyNewComment, cmd.NotifyActivityInThread, cmd.NotifyLikeTasting, cmd.NotifyLikeComment, cmd.ReceiveNewsLetter, cmd.NotifyChampagneTasted, cmd.NotifyBrandNews));
		}
        
        //Commands -> Follow, unfollow users and brands -> bookmark and unbookmark champagnes
		public void Execute(FollowUser cmd)
		{
			RaiseEvent(new UserFollowed(Id, cmd.FollowToId, cmd.FollowToName, cmd.FollowToImageId, new NotEmptyString(Name.Value), ImageCustomization.ProfilePictureImgId));
		}
      
		public void Execute(UnfollowUser cmd)
        {
			RaiseEvent(new UserUnfollowed(Id, cmd.FollowToId));
        }

		public void Execute(FollowBrand cmd)
        {
			RaiseEvent(new BrandFollowed(Id, new NotEmptyString(Name.Value), ImageCustomization.ProfilePictureImgId, cmd.BrandId, cmd.BrandName, cmd.BrandLogoId));
        }

		public void Execute(UnfollowBrand cmd)
        {
			RaiseEvent(new BrandUnfollowed(Id, cmd.BrandId));
        }

		public void Execute(BookmarkChampagne cmd)
        {
			RaiseEvent(new ChampagneBookmarked(Id, cmd.ChampagneId));
        }

		public void Execute(UnbookmarkChampagne cmd)
        {
			RaiseEvent(new ChampagneUnbookmarked(Id, cmd.ChampagneId));
        }

		public void Execute(AddTastedChampagne cmd)
		{
			RaiseEvent(new TastedChampagneAdded(Id, cmd.ChampagneId));
		}

		public void Execute(RemoveTastedChampagne cmd)
		{
			RaiseEvent(new TastedChampagneRemoved(Id, cmd.ChampagneId));
		}

		public void Execute(LikeEntity cmd)
		{
			RaiseEvent(new EntityLiked(Id, new NotEmptyString(Name.Value), ImageCustomization.ProfilePictureImgId, cmd.LikeToContextId, cmd.ContextType));
		}

		public void Execute(UnlikeEntity cmd)
		{
			RaiseEvent(new EntityUnliked(Id, cmd.LikeToContextId));
		}

		public void Execute(UpdateUserImageCustomization cmd)
		{
			var profileImageDidChange = !cmd.ProfileImageId.Value.Equals(ImageCustomization.ProfilePictureImgId.Value);
			RaiseEvent(new UserImageCustomizationEdited(Id, cmd.ProfileImageId, cmd.ProfileCoverImageId, cmd.ProfileCellarCardImageId, cmd.ProfileCellarHeaderImageId, profileImageDidChange));
		}

		public void Execute(MarkNotificationAsRead cmd)
		{
			RaiseEvent(new NotificationMarkedAsRead(Id, cmd.NotificationId));
		}

		/// <summary>
		/// This method is idempotent on pushChannel.
		/// </summary>
		/// <param name="cmd"></param>
		public void Execute(RegisterDeviceInstallation cmd)
		{
			RaiseEvent(new DeviceInstallationRegistered(Id, cmd.DeviceInstallation));
		}

		public void Execute(DeregisterDeviceInstallation cmd)
		{
			RaiseEvent(new DeviceInstallationDeregistered(Id, cmd.PushChannel));
		}

		public void Execute(MarkLatestNotificationSeen cmd)
		{
			RaiseEvent(new LatestNotificationSeenMarked(Id, cmd.NotificationId));
		}

		public void Execute(UpdateSleepSettings cmd)
		{
			RaiseEvent(new SleepSettingsUpdated(Id, cmd.UTCOffset, cmd.NotifyFrom, cmd.NotifyTo));
		}

		public void Execute(UpdateUserEmail cmd)
		{
			RaiseEvent(new UserEmailUpdated(Id, cmd.Email, Name));
		}

		public void Execute(ResendConfirmationEmail cmd)
		{
			RaiseEvent(new ConfirmationEmailResend(Id, Email, Name));
		}

		public void Execute(ConfirmEmail cmd)
		{
			if (!Email.Value.Equals(cmd.Email.Value))
			{
				throw new DomainException("The email to be confirmed does not match the users respective email");
			}
			RaiseEvent(new EmailConfirmed(Id, Email, true));
		}

		public void Execute(DeleteUser cmd)
		{
			RaiseEvent(new UserDeleted(Id));
		}

		public void Execute(SetMigrationSource cmd)
		{
			RaiseEvent(new MigrationSourceSet(Id, cmd.MigrationSource));
		}
		
		
		protected override void RegisterHandlers()
		{
			Handle<UserCreated>(evt =>
			{
				Id = evt.Id;
				Email = evt.Email;
				Name = evt.Name;
				ProfileName = evt.ProfileName;
				Biography = evt.Biography;

				Following = evt.Following;
				FollowingBrands = evt.FollowingBrands;
				BookmarkedChampagnes = evt.BookmarkedChampagnes;
				TastedChampagnes = evt.TastedChampagnes;
				LikedEntities = evt.LikedEntities;

				DeviceInstallations = evt.DeviceInstallations;
				
				ImageCustomization = new UserImageCustomization
				{
					CellarCardImgId = evt.CellarCardImgId,
					CellarHeaderImgId = evt.CellarHeaderImgId,
					ProfileCoverImgId = evt.ProfileCoverImgId,
					ProfilePictureImgId = evt.ProfilePictureImgId
				};
				
				UserSettings = new UserSettings();

				UserSettings.CountryCode = evt.CountryCode;
				UserSettings.Language = evt.LanguageCode;
				UserSettings.SleepSettings = evt.SleepSettings;

				IsDeleted = false;
			});

			Handle<UserInfoEdited>(evt =>
			{
				Name = evt.Name;
				ProfileName = evt.ProfileName;
				Biography = evt.Biography;
			});

			Handle<ProfileCoverUpdated>(evt =>
			{
				ImageCustomization.ProfileCoverImgId = evt.ProfileCoverImgId;
			});

			Handle<ProfileImgUpdated>(evt =>
			{
				ImageCustomization.ProfilePictureImgId = evt.ProfileImgId;
			});

			Handle<CellarCardImgUpdated>(evt =>
			{
				ImageCustomization.CellarCardImgId = evt.CellarCardImgId;
			});

			Handle<CellarHeaderImgUpdated>(evt =>
			{
				ImageCustomization.CellarHeaderImgId = evt.CellarHeaderImgId;
			});
			
			Handle<NotificationSettingsAdded>(evt =>
			{
				var notificationSettings = new Notifications
				{
					ReceiveCMNotifications = evt.ReceiveCmNotifications,
					ReceiveNewsLetter = evt.ReceiveNewsLetter,
					NotifyLikeTasting = evt.NotifyLikeTasting,
					NotifyNewComment = evt.NotifyNewComment,
					NotifyNewFollower = evt.NotifyNewFollower,
					NotifyLikeComment = evt.NotifyLikeComment,
					NotifyActivityInThread = evt.NotifyActivityInThread,
					NotifyBrandNews = evt.NotifyBrandNews,
					NotifyChampagneTasted = evt.NotifyChampagneTasted
				};

				NotificationSettings = notificationSettings;

			});

			Handle<UserSettingsEdited>(evt =>
			{
				UserSettings.CountryCode = evt.CountryCode;
				UserSettings.Language = evt.Language;
			});

			//Follow brand + user + bookmark
			Handle<UserFollowed>(evt =>
			{
				Following.Add(evt.FollowToId);
			});

			Handle<UserUnfollowed>(evt =>
            {
				Following.Remove(evt.FollowToId);
            });

			Handle<BrandFollowed>(evt =>
            {
				FollowingBrands.Add(evt.BrandId);
            });

			Handle<BrandUnfollowed>(evt =>
            {
				FollowingBrands.Remove(evt.BrandId);
            });

			Handle<ChampagneBookmarked>(evt =>
            {
				BookmarkedChampagnes.Add(evt.ChampagneId);
            });

			Handle<ChampagneUnbookmarked>(evt =>
            {
				BookmarkedChampagnes.Remove(evt.ChampagneId);
            });
			
			Handle<TastedChampagneAdded>(evt => { TastedChampagnes.Add(evt.ChampagneId); });
			
			Handle<TastedChampagneRemoved>(evt => { TastedChampagnes.Remove(evt.ChampagneId); });

			Handle<EntityLiked>(evt =>
			{
				LikedEntities.Add(evt.LikeToContextId);
			});

			Handle<UserImageCustomizationEdited>(evt =>
			{
				if (ImageCustomization == null)
				{
					ImageCustomization = new UserImageCustomization();
				}

				ImageCustomization.ProfilePictureImgId = evt.ProfileImageId;
				ImageCustomization.ProfileCoverImgId = evt.ProfileCoverImageId;
				ImageCustomization.CellarCardImgId = evt.ProfileCellarCardImageId;
				ImageCustomization.CellarHeaderImgId = evt.ProfileCellarHeaderImageId;
			});
			
			Handle<UserNotificationSettingsUpdated>(evt =>
			{
				if (NotificationSettings == null)
				{
					NotificationSettings = new Notifications();
				}

				NotificationSettings.NotifyActivityInThread = evt.NotifyActivityInThread;
				NotificationSettings.NotifyLikeComment = evt.NotifyLikeComment;
				NotificationSettings.NotifyLikeTasting = evt.NotifyLikeTasting;
				NotificationSettings.NotifyNewComment = evt.NotifyNewComment;
				NotificationSettings.NotifyNewFollower = evt.NotifyNewFollower;
				NotificationSettings.ReceiveCMNotifications = evt.ReceiveCmNotifications;
				NotificationSettings.ReceiveNewsLetter = evt.ReceiveNewsLetter;
				NotificationSettings.NotifyChampagneTasted = evt.NotifyChampagneTasted;
				NotificationSettings.NotifyBrandNews = evt.NotifyBrandNews;

			});
			
			Handle<EntityUnliked>(evt =>
			{
				LikedEntities.Remove(evt.UnlikeToContextId);
			});
			
			Handle<DeviceInstallationRegistered>(evt =>
			{
				if (DeviceInstallations == null)
				{
					DeviceInstallations = new HashSet<DeviceInstallation>();
				}

				bool installationAlreadyExist = false;
				foreach (var deviceInstallation in DeviceInstallations)
				{
					if (deviceInstallation.InstallationId.Value.Equals(evt.DeviceInstallation.InstallationId.Value))
					{
						installationAlreadyExist = true;
					}
				}

				if (!installationAlreadyExist)
				{
					DeviceInstallations.Add(evt.DeviceInstallation);
				}
			});

			Handle<DeviceInstallationDeregistered>(evt =>
			{
				if (DeviceInstallations == null)
				{
					DeviceInstallations = new HashSet<DeviceInstallation>();
				}

				DeviceInstallations.RemoveWhere(x => x.PushChannel == evt.PushChannel);
				
			});

			Handle<NotificationMarkedAsRead>(evt =>
			{
				if (ReadNotifications == null)
				{
					ReadNotifications = new HashSet<AggregateId>();
				}

				ReadNotifications.Add(evt.NotificationId);
			});
			
			Handle<LatestNotificationSeenMarked>(evt => { LatestNotificationSeen = evt.NotificationId; });

			Handle<SleepSettingsUpdated>(evt =>
			{
				if (UserSettings.SleepSettings == null)
				{
					UserSettings.SleepSettings = new SleepSettings(evt.UTCOffset, evt.NotifyFrom, evt.NotifyTo);
				}

				UserSettings.SleepSettings.UTCOffset = evt.UTCOffset;
				UserSettings.SleepSettings.NotifyFrom = evt.NotifyFrom;
				UserSettings.SleepSettings.NotifyTo = evt.NotifyTo;
			});

			Handle<UserEmailUpdated>(evt =>
			{
				Email = evt.Email;
				if (UserSettings != null)
				{
					UserSettings.IsEmailVerified = false;
				}
			});

			Handle<EmailConfirmed>(evt =>
			{
				if (UserSettings != null)
				{
					UserSettings.IsEmailVerified = evt.IsEmailVerified;
				}
			});

			Handle<ConfirmationEmailResend>(evt => { });

			Handle<UserDeleted>(evt => { IsDeleted = true; });

			Handle<MigrationSourceSet>(evt => { MigrationSource = evt.MigrationSource; });

		}
	}
}
