using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.Entities;
using Microsoft.Azure.NotificationHubs;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class User : IEntity
	{
        [BsonId]
		public Guid Id { get; set; }
        
		public string Email { get; set; }
		public string Name { get; set; }
		public string ProfileName { get; set; }
		public string Biography { get; set; }

        //List of saved champagnes
		public Guid[] BookmarkedChampagnes { get; set; }

        //List of tasted champagnes or more specific a list of tasting.cs
		public Guid[] TastedChampagnes { get; set; }

        //List of people whom this user is following
		//This is kept since it gives access to quick toggles throughout all usecases where we need to check if this user is following them, way faster in memory and takes the load from database calls.
		//If not here this data should be retreived through the FollowRepository for each
		public Guid[] Following { get; set; }
		
		public HashSet<Installation> DeviceInstallations { get; set; } = new HashSet<Installation>();
		
		//The the last notification which the user has seen
		public Guid LastNotificationSeen { get; set; } = Guid.Empty;
		//The user needs a list of read push-notifications
		public HashSet<Guid> ReadNotifications { get; set; } = new HashSet<Guid>();
		//The user needs a list of push-notifications to which he is eligible to read
		public HashSet<Guid> AvailableNotifications { get; set; } = new HashSet<Guid>();
		//Indicates when the user last checked his list of notification and thereby emptied the notification badge.
		//[BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance)] --> This is deprecated...
		public DateTime NotificationsUpdateOn { get; set; } = DateTime.MinValue;
        
		//List of brands whom this user is following
		public Guid[] FollowingBrands { get; set; } 
		
		public DateTime CreatedAt { get; set; } = DateTime.MinValue.AddYears(2018);
		
        //User image customization
		public ImageCustomization ImageCustomization { get; set; }

        //Users settings
		public UserSettings Settings { get; set; }
		
		//Notification settings
		public NotificationSettings Notifications { get; set; }
		
		public SleepSettings SleepSettings { get; set; }
		
	}
}
