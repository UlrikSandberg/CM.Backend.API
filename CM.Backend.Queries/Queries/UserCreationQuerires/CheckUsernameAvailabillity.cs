using System;
using SimpleSoft.Mediator;
namespace CM.Backend.Queries.Queries.UserCreationQuerires
{
	public class CheckUsernameAvailabillity : Query<bool>
    {
		public string Username { get; set; }

		public CheckUsernameAvailabillity(string username)
        {
            Username = username;
		}
    }
}
