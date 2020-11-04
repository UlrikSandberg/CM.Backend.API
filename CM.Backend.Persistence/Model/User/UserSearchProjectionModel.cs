using System;

namespace CM.Backend.Persistence.Model
{
    public class UserSearchProjectionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public Guid ImageId { get; set; }
    }
}