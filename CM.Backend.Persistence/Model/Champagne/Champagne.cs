using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace CM.Backend.Persistence.Model
{
	public class Champagne : IEntity
	{
		[BsonId]
		public Guid Id { get; set; }

		public string BottleName { get; set; }
		public Guid BrandId { get; set; }
		public Guid BottleImgId { get; set; }
		public bool IsPublished { get; set; }
		
		//Controls the rating
		//[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<string, double> RatingDictionary { get; set; }
		//Deprecated
		public double AverageRating { get; set; }
        
		/// <summary>
		/// IMPORTANT!!! BEFORE UPDATING THIS... CHECK IF THE RESPECTIVE EVENT IS NOT A DUPLICATE BY CHECKING IF THE USER HAS ALREADY RATED THIS CHAMPAGNE AND IN THAT CASE DO NOT UPDATE THIS VALUE
		/// </summary>
		public double RateCount { get; set; }
		/// <summary>
		/// IMPORTANT!!! BEFORE UPDATING THIS... CHECK IF THE RESPECTIVE EVENT IS NOT A DUPLICATE BY CHECKING IF THE USER HAS ALREADY RATED THIS CHAMPAGNE AND IN THAT CASE DO NOT UPDATE THIS VALUE
		/// </summary>
		public double RateValue { get; set; }

		public bool IsUpdated { get; set; } = false;
		
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public Dictionary<string, string> ChampagneFolderDependencies { get; set; } = new Dictionary<string, string>();
		
		//Entities
		public VintageInfo vintageInfo { get; set; }
		public ChampagneProfile champagneProfile { get; set; }
	}
}