using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Events;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects;
using CM.Backend.Domain.Aggregates.Notification.Events;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.EventHandlers.Helpers;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Persistence.Repositories;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Serilog;
using SimpleSoft.Mediator;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CM.Backend.EventHandlers
{
	public class UserRMEventHandler :
    IEventHandler<MessageEnvelope<UserCreated>>,
	IEventHandler<MessageEnvelope<UserSettingsEdited>>,
	IEventHandler<MessageEnvelope<UserInfoEdited>>,
	IEventHandler<MessageEnvelope<ProfileCoverUpdated>>,
	IEventHandler<MessageEnvelope<ProfileImgUpdated>>,
	IEventHandler<MessageEnvelope<CellarCardImgUpdated>>,
	IEventHandler<MessageEnvelope<CellarHeaderImgUpdated>>,
	IEventHandler<MessageEnvelope<UserFollowed>>,
	IEventHandler<MessageEnvelope<UserUnfollowed>>,
	IEventHandler<MessageEnvelope<BrandFollowed>>,
	IEventHandler<MessageEnvelope<BrandUnfollowed>>,
	IEventHandler<MessageEnvelope<ChampagneBookmarked>>,
	IEventHandler<MessageEnvelope<ChampagneUnbookmarked>>,
	IEventHandler<MessageEnvelope<TastingCreated>>,
	IEventHandler<MessageEnvelope<TastingDeleted>>,
	IEventHandler<MessageEnvelope<NotificationSettingsAdded>>,
	IEventHandler<MessageEnvelope<UserImageCustomizationEdited>>,
	IEventHandler<MessageEnvelope<UserNotificationSettingsUpdated>>,
	IEventHandler<MessageEnvelope<DeviceInstallationRegistered>>,
	IEventHandler<MessageEnvelope<DeviceInstallationDeregistered>>,
	IEventHandler<MessageEnvelope<NotificationMarkedAsRead>>,
	IEventHandler<MessageEnvelope<LatestNotificationSeenMarked>>,
	IEventHandler<MessageEnvelope<SleepSettingsUpdated>>,
	IEventHandler<MessageEnvelope<UserEmailUpdated>>,
	IEventHandler<MessageEnvelope<EmailConfirmed>>,
	IEventHandler<MessageEnvelope<UserDeleted>>
	{
		private readonly IUserRepository userRepository;
		private readonly IOptions<ProjectionsPersistenceConfiguration> config;
		private readonly ImageResizer imageResizer;
		private readonly IOptions<NotificationHubConfiguration> notificationConfig;
		private readonly ILogger _logger;

		private const string Logo = "Logo";
		private const string Cover = "Cover";
		private const string Card = "Card";

		public UserRMEventHandler(IUserRepository userRepository, IOptions<ProjectionsPersistenceConfiguration> config, ImageResizer imageResizer, IOptions<NotificationHubConfiguration> notificationConfig, ILogger logger)
		{
			this.userRepository = userRepository;
			this.config = config;
			this.imageResizer = imageResizer;
			this.notificationConfig = notificationConfig;
			_logger = logger;
		}

		public async Task HandleAsync(MessageEnvelope<UserCreated> evt, CancellationToken ct)
		{
			var user = new User
			{
				Id = evt.Id,
				Email = evt.Event.Email.Value,
				Name = evt.Event.Name.Value,
				ProfileName = evt.Event.ProfileName,
				Biography = evt.Event.Biography,
				BookmarkedChampagnes = new Guid[0],
				TastedChampagnes = new Guid[0],
				Following = new Guid[0],
				ReadNotifications = new HashSet<Guid>(),
				AvailableNotifications = new HashSet<Guid>(),
				FollowingBrands = new Guid[0],
				NotificationsUpdateOn = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow,
				SleepSettings = new SleepSettings //add sleep settings
				{
					NotifyFrom = new Time(evt.Event.SleepSettings.NotifyFrom.Value),
					NotifyTo = new Time(evt.Event.SleepSettings.NotifyTo.Value),
					utcOffset = evt.Event.SleepSettings.UTCOffset.Value
				},
				ImageCustomization = new ImageCustomization
				{
					CellarCardImgId = evt.Event.CellarCardImgId.Value,
					CellarHeaderImgId = evt.Event.CellarHeaderImgId.Value,
					ProfileCoverImgId = evt.Event.ProfileCoverImgId.Value,
					ProfilePictureImgId = evt.Event.ProfilePictureImgId.Value
				}
			};
			user.Settings = new UserSettings();

			if (evt.Event.CountryCode != null)
			{
				user.Settings.CountryCode = evt.Event.CountryCode.Value;
			}

			if (evt.Event.LanguageCode != null)
			{
				user.Settings.Language = evt.Event.LanguageCode.Value;
			}

			user.Settings.IsEmailVerified = evt.Event.IsEmailVerified;

			await userRepository.Insert(user);
		}

		public async Task HandleAsync(MessageEnvelope<UserSettingsEdited> evt, CancellationToken ct)
		{
			var pUserSettings = new UserSettings();
			pUserSettings.CountryCode = evt.Event.CountryCode.Value;
			pUserSettings.Language = evt.Event.Language.Value;

			await userRepository.UpdateUserSettings(evt.Id, pUserSettings);
		}

		public async Task HandleAsync(MessageEnvelope<UserInfoEdited> evt, CancellationToken ct)
		{
			await userRepository.UpdateUserInfo(evt.Id, evt.Event.Name.Value, evt.Event.ProfileName, evt.Event.Biography);
		}

		public async Task HandleAsync(MessageEnvelope<ProfileCoverUpdated> evt, CancellationToken ct)
		{
			//Maybe the images should be resized here but likewise we could also resize the image pre API at client side.
			//For now just update the model

			await userRepository.UpdateProfileCover(evt.Id, evt.Event.ProfileCoverImgId.Value);         
		}

		public async Task HandleAsync(MessageEnvelope<ProfileImgUpdated> evt, CancellationToken ct)
		{
			//Maybe the images should be resized here but likewise we could also resize the image pre API at client side.
			//For now just update the model

			await userRepository.UpdateProfileImg(evt.Id, evt.Event.ProfileImgId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<CellarCardImgUpdated> evt, CancellationToken ct)
		{
			await userRepository.UpdateCellarCardImg(evt.Id, evt.Event.CellarCardImgId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<CellarHeaderImgUpdated> evt, CancellationToken ct)
		{
			await userRepository.UpdateCellarHeaderImg(evt.Id, evt.Event.CellarHeaderImgId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<UserFollowed> evt, CancellationToken ct)
		{
			await userRepository.FollowUser(evt.Id, evt.Event.FollowToId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<UserUnfollowed> evt, CancellationToken ct)
		{
			await userRepository.UnfollowUser(evt.Id, evt.Event.FollowToId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<BrandFollowed> evt, CancellationToken ct)
		{
			await userRepository.FollowBrand(evt.Id, evt.Event.BrandId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<BrandUnfollowed> evt, CancellationToken ct)
		{
			await userRepository.UnfollowBrand(evt.Id, evt.Event.BrandId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneBookmarked> evt, CancellationToken ct)
		{
			await userRepository.BookmarkChampagne(evt.Id, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneUnbookmarked> evt, CancellationToken ct)
		{
			await userRepository.UnbookmarkChampagne(evt.Id, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
		{
			await userRepository.AddTasting(evt.Event.AuthorId.Value, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<TastingDeleted> evt, CancellationToken ct)
		{
			await userRepository.RemoveTasting(evt.Event.AuthorId.Value, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<NotificationSettingsAdded> evt, CancellationToken ct)
		{
			await userRepository.AddNotificationSettings(evt.Id, new NotificationSettings
			{
				ReceiveCMNotifications = evt.Event.ReceiveCmNotifications,
				NotifyNewFollower = evt.Event.NotifyNewFollower,
				NotifyNewComment = evt.Event.NotifyNewComment,
				NotifyActivityInThread = evt.Event.NotifyActivityInThread,
				NotifyLikeTasting = evt.Event.NotifyLikeTasting,
				NotifyLikeComment = evt.Event.NotifyLikeComment,
				ReceiveNewsLetter = evt.Event.ReceiveNewsLetter,
				NotifyChampagneTasted = evt.Event.NotifyChampagneTasted,
				NotifyBrandNews = evt.Event.NotifyBrandNews
			});
		}

		public async Task HandleAsync(MessageEnvelope<UserImageCustomizationEdited> evt, CancellationToken ct)
		{
			var user = await userRepository.GetById(evt.Id);

			var imagesToChange = new Dictionary<Guid, string>();
			
			//Check which images has been changed - Only upload resized images for changes ones.
			if (!user.ImageCustomization.ProfilePictureImgId.Equals(evt.Event.ProfileImageId.Value))
			{
				imagesToChange.Add(evt.Event.ProfileImageId.Value, Logo);
			}

			if (!user.ImageCustomization.ProfileCoverImgId.Equals(evt.Event.ProfileCoverImageId.Value))
			{
				imagesToChange.Add(evt.Event.ProfileCoverImageId.Value, Cover);
			}

			if (!user.ImageCustomization.CellarCardImgId.Equals(evt.Event.ProfileCellarCardImageId.Value))
			{
				imagesToChange.Add(evt.Event.ProfileCellarCardImageId.Value, Card);
			}

			if (!user.ImageCustomization.CellarHeaderImgId.Equals(evt.Event.ProfileCellarHeaderImageId.Value))
			{
				imagesToChange.Add(evt.Event.ProfileCellarHeaderImageId.Value, Cover);
			}
			
			await userRepository.EditUserImageCustomization(evt.Id, new ImageCustomization
			{
				ProfilePictureImgId = evt.Event.ProfileImageId.Value,
				ProfileCoverImgId = evt.Event.ProfileCoverImageId.Value,
				CellarCardImgId = evt.Event.ProfileCellarCardImageId.Value,
				CellarHeaderImgId = evt.Event.ProfileCellarHeaderImageId.Value
			});

			foreach (var changeImage in imagesToChange)
			{
				await ResizeImage(evt.Id, changeImage.Key, changeImage.Value);
			}
		}
		
		private async Task ResizeImage(Guid userId, Guid imageId, string imageType)
        {

            CloudStorageAccount cloudStorageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            
            var storageConnectionString = config.Value.AzureStorageAccountConnectionString;
            
            //Establish connection to the blob storage
            if(CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount))
            {
                try
                {
                    //Init blobClient
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("user-files");
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() +"/"+ "images" + "/"+ imageId.ToString() + "/" + "original.jpg");

                    using(var ms = new MemoryStream())
                    {
                        await cloudBlockBlob.DownloadToStreamAsync(ms);

                        var file = ms.ToArray();

						if (imageType.Equals(Cover))
						{
							//Cover images are set to aspectFill in the app which means that the images should be resized according to original ratio
							//This means we can only control either the H or W. Since covers are represented as horizontal images
							//the width has been locked to pre defined sizes, whereas the height is calculated based on image orignal ratio.

							double imageRatio = 0;

							//Determine ratio of uploaded cover img
							using (Image<Rgba32> image = Image.Load(file))
							{
								imageRatio = (double)image.Height / (double)image.Width;
							}
                            
							double smallWidth = 500;
							double largeWidth = 800;

							var smallImg = imageResizer.Resize(file, (int)smallWidth, (int)(smallWidth * imageRatio));
							var largeImg = imageResizer.Resize(file, (int)largeWidth, (int)(largeWidth * imageRatio));

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "small.jpg");
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "large.jpg");

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
						else if (imageType.Equals(Card))
						{

							//Card images are set to aspectFill in the app which means that the images should be resized according to original ratio
							//This means we can only control either the H or W. Since cards are represented as vertical images
							//the Height has been locked to pre defined sizes, whereas the width is calculated based on image orignal ratio.
                            
							double imageRatio = 0;
							//Determine ratio of uploaded cover img
							using (Image<Rgba32> image = Image.Load(file))
							{
								imageRatio = (double)image.Width / (double)image.Height;
							}
                            
							double smallHeight = 600;
							double largeHeight = 800;
                            
							var smallImg = imageResizer.Resize(file, (int)(smallHeight * imageRatio), (int)(smallHeight));
							var largeImg = imageResizer.Resize(file, (int)(largeHeight * imageRatio), (int)(largeHeight));

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "small.jpg");
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "large.jpg");

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
						else if (imageType.Equals(Logo))
						{
							//A logo is allways resized according to W:H ratio of 1:1 = quadratic.
							var smallImg = imageResizer.Resize(file, 400, 400);
							var largeImg = imageResizer.Resize(file, 800, 800);

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "small.jpg");
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + imageId.ToString() + "/" + "large.jpg");

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
                    }               
                } 
                catch(StorageException ex)
                {
                    Console.WriteLine(ex.Message);
	                _logger.Fatal(ex, "Internal error, failed to upload .small and .large image resizing to user images folder for {UserId}, {ImageId}, {FileType}", userId, imageId, imageType);
                }
            }         
        }

		public async Task HandleAsync(MessageEnvelope<UserNotificationSettingsUpdated> evt, CancellationToken ct)
		{
			var pUserNotificationSettings = new NotificationSettings
			{
				NotifyActivityInThread = evt.Event.NotifyActivityInThread,
				NotifyLikeComment = evt.Event.NotifyLikeComment,
				NotifyLikeTasting = evt.Event.NotifyLikeTasting,
				NotifyNewComment = evt.Event.NotifyNewComment,
				NotifyNewFollower = evt.Event.NotifyNewFollower,
				ReceiveCMNotifications = evt.Event.ReceiveCmNotifications,
				ReceiveNewsLetter = evt.Event.ReceiveNewsLetter,
				NotifyChampagneTasted = evt.Event.NotifyChampagneTasted,
				NotifyBrandNews = evt.Event.NotifyBrandNews
			};

			await userRepository.UpdateUserNotificationSettings(evt.Id, pUserNotificationSettings);
		}

		public async Task HandleAsync(MessageEnvelope<DeviceInstallationRegistered> evt, CancellationToken ct)
		{
			//Find user, in order to read which settings and thereby tags should be registered along side the user installation
			var user = await userRepository.GetById(evt.Id);

			//Which tags should be associated with current deviceInstallation
			var tags = new List<string>();

			if (user.Notifications != null)
			{
				if (user.Notifications.ReceiveCMNotifications)
				{
					tags.Add(NotificationTags.CMNotification.ToString());
				}
			}

			//Create new detailed deciveInstallation based on user!
			var deviceInstallation = new Installation();
			deviceInstallation.InstallationId = evt.Event.DeviceInstallation.InstallationId.Value;
			deviceInstallation.PushChannel = evt.Event.DeviceInstallation.PushChannel.Value;
			deviceInstallation.Tags = tags;
			deviceInstallation.Templates = new Dictionary<string, InstallationTemplate>();
			//Set platform and platform specific template

			switch (evt.Event.DeviceInstallation.Platform.Value)
			{
				case "mpns":
					deviceInstallation.Platform = NotificationPlatform.Mpns;
					break;
				case "wns":
					deviceInstallation.Platform = NotificationPlatform.Wns;
					break;
				case "apns":
					deviceInstallation.Platform = NotificationPlatform.Apns;
					/*deviceInstallation.Templates["genericMessage"] = new InstallationTemplate
					{
						Body = NotificationPlatformTemplates.templateBodyAPNS
						
					}; Only use the genericUrlMessage template, since this features both message and contextUrl for app routing */
					deviceInstallation.Templates["genericUrlMessage"] = new InstallationTemplate
					{
						Body = NotificationPlatformTemplates.templateBodyAPNSWithURlAndIDAndSound
					};
					break;
				case "gcm":
					deviceInstallation.Platform = NotificationPlatform.Gcm;
					break;
				case "fcm":
					deviceInstallation.Platform = NotificationPlatform.Gcm;
					break;
				default:
					throw new Exception("Handle(MessageEnvelope<DeviceInstallationRegistered>). () -> Exception -> Registered notification platform: " + evt.Event.DeviceInstallation.Platform + " is not supported. Only mpns, wns, apns, gcm and fcm are valid notfication platform parameters");
			}
			
			await userRepository.RegisterDeviceInstallation(evt.Id, deviceInstallation);
			
			//Upload deviceInstallation to notificationHub.
			
			var client = NotificationHubClient.CreateClientFromConnectionString(
				notificationConfig.Value.NotificationHubConnectionString,
				notificationConfig.Value.NotificationHubName);

			
			await client.CreateOrUpdateInstallationAsync(deviceInstallation);
			
		}

		public async Task HandleAsync(MessageEnvelope<DeviceInstallationDeregistered> evt, CancellationToken ct)
		{
			await userRepository.DeregisterDeviceInstallation(evt.Id, evt.Event.PushChannel.Value);
		}

		public async Task HandleAsync(MessageEnvelope<NotificationMarkedAsRead> evt, CancellationToken ct)
		{
			await userRepository.MarkNotificationAsRead(evt.Id, evt.Event.NotificationId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<LatestNotificationSeenMarked> evt, CancellationToken ct)
		{
			await userRepository.MarkLatestNotificationSeen(evt.Id, evt.Event.NotificationId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<SleepSettingsUpdated> evt, CancellationToken ct)
		{
			await userRepository.UpdateSleepSettings(evt.Id, new SleepSettings
			{
				utcOffset = evt.Event.UTCOffset.Value,
				NotifyFrom = new Time(evt.Event.NotifyFrom.Value),
				NotifyTo = new Time(evt.Event.NotifyTo.Value)
			});
		}

		public async Task HandleAsync(MessageEnvelope<UserEmailUpdated> evt, CancellationToken ct)
		{
			var user = await userRepository.GetById(evt.Id);
			
			var settings = new UserSettings();
			if (user.Settings != null)
			{
				settings.Language = user.Settings.Language;
				settings.CountryCode = user.Settings.CountryCode;
				settings.IsEmailVerified = false;
			}
			
			await userRepository.UpdateEmail(evt.Id, evt.Event.Email.Value, settings);
		}

		public async Task HandleAsync(MessageEnvelope<EmailConfirmed> evt, CancellationToken ct)
		{
			var user = await userRepository.GetById(evt.Id);
			
			var settings = new UserSettings();
			if (user.Settings != null)
			{
				settings.Language = user.Settings.Language;
				settings.CountryCode = user.Settings.CountryCode;
				settings.IsEmailVerified = evt.Event.IsEmailVerified;
			}

			await userRepository.UpdateIsEmailVerified(evt.Id, settings);
		}

		public async Task HandleAsync(MessageEnvelope<UserDeleted> evt, CancellationToken ct)
		{
			await userRepository.Delete(evt.Id);
		}
	}
}
