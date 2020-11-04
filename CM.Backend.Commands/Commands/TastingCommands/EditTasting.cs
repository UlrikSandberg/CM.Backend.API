using System;

namespace CM.Backend.Commands.Commands.TastingCommands
{
    public class EditTasting : Command
    {
        public Guid TastingId { get; private set; }
        public Guid UserId { get; private set; }
        public string Review { get; private set; }
        public double Rating { get; private set; }

        public EditTasting(Guid tastingId, Guid userId, string review, double rating)
        {
            TastingId = tastingId;
            UserId = userId;
            Review = review;
            Rating = rating;
        }
    }
}