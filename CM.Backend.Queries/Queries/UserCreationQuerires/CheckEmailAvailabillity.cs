using System;
using SimpleSoft.Mediator;
using CM.Backend.Persistence.Model;
using MongoDB.Bson;
namespace CM.Backend.Queries.Queries.UserCreationQuerires
{
	public class CheckEmailAvailabillity : Query<bool>
    {
        public string Email { get; set; }

		public CheckEmailAvailabillity(string email)
        {
            Email = email;
		}
    }
}
