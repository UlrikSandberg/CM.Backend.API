namespace CM.Backend.API.RequestModels.UserRequestModels
{
    public class CreateUserRequestModel
    {   
		public string Email { get; set; }
		public string Name { get; set; }
	    public string Password { get; set; }
		public string ProfileName { get; set; }
		public string Biography { get; set; }
	    public long UTFOffset { get; set; }
    }
}
