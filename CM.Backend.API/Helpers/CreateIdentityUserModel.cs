using System;

namespace CM.Backend.API.Helpers
{
    public class CreateIdentityUserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}