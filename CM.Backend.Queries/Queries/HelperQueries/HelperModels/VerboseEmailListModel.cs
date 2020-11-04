using System;

namespace CM.Backend.Queries.Queries.HelperQueries.HelperModels
{
    public class VerboseEmailListModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}