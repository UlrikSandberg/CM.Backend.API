using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class CreateUser
    {
		public AggregateId Id { get; private set; }
		public Email Email { get; private set; }
		public Name Name { get; private set; }
		public string ProfileName { get; private set; }
		public string Biography { get; private set; }
	    public SleepSettings SleepSettings { get; private set; }
	    public ImageId ProfilePictureImgId { get; private set; }
	    public ImageId ProfileCoverImgId { get; private set; }
	    public ImageId CellarCardImgId { get; private set; }
	    public ImageId CellarHeaderImgId { get; private set; }
	    public CountryCode CountryCode { get; private set; }
	    public Language LanguageCode { get; private set; }
	    public bool IsEmailVerified { get; private set; }

	    public HashSet<AggregateId> BookmarkedChampagnes { get; private set; }
		public HashSet<AggregateId> Following { get; private set; }
		public HashSet<AggregateId> FollowingBrands { get; private set; }
	    public HashSet<AggregateId> TastedChampagnes { get; private set; }
	    public HashSet<AggregateId> LikedEntities { get; private set; }

		public CreateUser(AggregateId id, Email email, Name name, string profileName, string biography, SleepSettings sleepSettings, ImageId profilePictureImgId, ImageId profileCoverImgId, ImageId cellarCardImgId, ImageId cellarHeaderImgId, CountryCode countryCode, Language languageCode, bool isEmailVerified)
        {
	        if (id == null || email == null || name == null || sleepSettings == null || profilePictureImgId == null || profileCoverImgId == null || cellarCardImgId == null || cellarHeaderImgId == null)
	        {
		        throw new ArgumentException(nameof(CreateUser) + ": Parameter values may not be null");
	        }
	        
			Id = id;
			Biography = biography;
	        SleepSettings = sleepSettings;
	        ProfilePictureImgId = profilePictureImgId;
	        ProfileCoverImgId = profileCoverImgId;
	        CellarCardImgId = cellarCardImgId;
	        CellarHeaderImgId = cellarHeaderImgId;
	        CountryCode = countryCode;
	        LanguageCode = languageCode;
	        IsEmailVerified = isEmailVerified;
	        ProfileName = profileName;
			Name = name;
			Email = email;

	        BookmarkedChampagnes = new HashSet<AggregateId>();
	        Following = new HashSet<AggregateId>();
	        FollowingBrands = new HashSet<AggregateId>();
	        TastedChampagnes = new HashSet<AggregateId>();
	        LikedEntities = new HashSet<AggregateId>();
        }
    }
}
