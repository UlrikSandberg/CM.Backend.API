using System;

namespace CM.Backend.Commands.Commands.TastingCommands
{
    public class CreateTasting : CommandWithIdResponse
    {
        public Guid UserId { get; private set; }
        public Guid ChampagneId { get; private set; }
        public string Review { get; private set; }
        public double Rating { get; private set; }
        public DateTime TimeStamp { get; private set; }


        public CreateTasting(Guid userId, Guid champagneId, string review, double rating, DateTime timeStamp)
        {
            UserId = userId;
            ChampagneId = champagneId;
            Review = review;
            Rating = rating;
            TimeStamp = timeStamp;
        }
    }
}