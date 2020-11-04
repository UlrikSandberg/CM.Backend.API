using System;

namespace CM.Backend.Queries.Model.UserModels
{
    public class UserSearchModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}