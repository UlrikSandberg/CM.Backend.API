using System;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;

namespace CM.Backend.Queries.Model.UserModels
{
    public class UserSettingsModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Biography { get; set; }
        
        public ImageCustomization ImageCustomization { get; set; }
        
        public NotificationSettings NotificationSettings { get; set; }
        
    }
}