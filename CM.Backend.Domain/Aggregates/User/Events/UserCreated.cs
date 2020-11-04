using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class UserCreated : DomainEvent
	{

		public Email Email { get; private set; }
		public Name Name { get; private set; }
		public string ProfileName { get; private set; }
		public string Biography { get; private set; }

		public HashSet<AggregateId> BookmarkedChampagnes { get; private set; }
		public HashSet<AggregateId> Following { get; private set; }
		public HashSet<AggregateId> FollowingBrands { get; private set; }
		public HashSet<AggregateId> TastedChampagnes { get; private set; }
		public HashSet<AggregateId> LikedEntities { get; private set; }
		public SleepSettings SleepSettings { get; private set; }
		public ImageId ProfilePictureImgId { get; private set; }
		public ImageId ProfileCoverImgId { get; private set; }
		public ImageId CellarCardImgId { get; private set; }
		public ImageId CellarHeaderImgId { get; private set; }
		public CountryCode CountryCode { get; private set; }
		public Language LanguageCode { get; private set; }
		public bool IsEmailVerified { get; private set; }
		public HashSet<DeviceInstallation> DeviceInstallations { get; private set; }

		public UserCreated(Guid id, Email email, Name name, string profileName, string biography, HashSet<AggregateId> bookmarkedChampagnes, HashSet<AggregateId> following, HashSet<AggregateId> followingBrands, HashSet<AggregateId> tastedChampagnes, HashSet<AggregateId> likedEntities, SleepSettings sleepSettings, ImageId profilePictureImgId, ImageId profileCoverImgId, ImageId cellarCardImgId, ImageId cellarHeaderImgId, CountryCode countryCode, Language languageCode, bool isEmailVerified) : base(id)
		{
			FollowingBrands = followingBrands;
			Following = following;
			BookmarkedChampagnes = bookmarkedChampagnes;
			TastedChampagnes = tastedChampagnes;
			LikedEntities = likedEntities;
			SleepSettings = sleepSettings;
			ProfilePictureImgId = profilePictureImgId;
			ProfileCoverImgId = profileCoverImgId;
			CellarCardImgId = cellarCardImgId;
			CellarHeaderImgId = cellarHeaderImgId;
			CountryCode = countryCode;
			LanguageCode = languageCode;
			IsEmailVerified = isEmailVerified;
			Biography = biography;
			ProfileName = profileName;
			Name = name;
			Email = email;
			DeviceInstallations = new HashSet<DeviceInstallation>();
		}
	}
}
