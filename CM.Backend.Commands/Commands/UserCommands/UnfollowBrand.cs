using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UnfollowBrand : Command
    {
		public Guid UserId { get; private set; }
		public Guid BrandId { get; private set; }

		public UnfollowBrand(Guid userId, Guid brandId)
        {
            BrandId = brandId;
			UserId = userId;
		}
    }
}
