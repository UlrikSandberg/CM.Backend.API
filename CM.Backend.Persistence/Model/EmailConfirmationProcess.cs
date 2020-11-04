using System;

namespace CM.Backend.Persistence.Model
{
    public class EmailConfirmationProcess : IEntity
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public string Email { get; set; }
        
        public string ConfirmationToken { get; set; }
        
        public DateTime ConfirmationEmailInitiatedAt { get; set; }
        
        public bool IsActive { get; set; }
    }
}