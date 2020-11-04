using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class FollowBrand : Command
    {
		public Guid UserId { get; private set; }
		public Guid BrandId { get; private set; }

		public FollowBrand(Guid userId, Guid brandId)
        {
            BrandId = brandId;
			UserId = userId;
		}
    }
}
