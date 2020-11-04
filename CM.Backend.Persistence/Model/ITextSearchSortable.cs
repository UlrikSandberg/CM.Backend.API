using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public interface ITextSearchSortable
    {
        [BsonIgnoreIfNull]
        double? TextMatchScore { get; set; }
    }
}