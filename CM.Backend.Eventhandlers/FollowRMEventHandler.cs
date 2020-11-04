using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.Brand.Events;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
	public class FollowRMEventHandler :
    IEventHandler<MessageEnvelope<UserFollowed>>,
	IEventHandler<MessageEnvelope<UserUnfollowed>>,
	IEventHandler<MessageEnvelope<BrandFollowed>>,
	IEventHandler<MessageEnvelope<BrandUnfollowed>>,
	IEventHandler<MessageEnvelope<UserInfoEdited>>,
	IEventHandler<MessageEnvelope<UserImageCustomizationEdited>>,
	IEventHandler<MessageEnvelope<BrandInfoUpdated>>,
	IEventHandler<MessageEnvelope<BrandImagesUpdated>>
	{
		private readonly IFollowRepository followRepository;
		private readonly IFollowBrandRepository followBrandRepository;

		public FollowRMEventHandler(IFollowRepository followRepository, IFollowBrandRepository followBrandRepository)
        {
            this.followBrandRepository = followBrandRepository;
			this.followRepository = followRepository;
		}

		public async Task HandleAsync(MessageEnvelope<UserFollowed> evt, CancellationToken ct)
		{
			await followRepository.Insert(new Follow
			{
				Id = Guid.NewGuid(),
				Key = new Follow.PrimaryKey {FollowById = evt.Id, FollowToId = evt.Event.FollowToId.Value},
				FollowById = evt.Id,
				FollowByName = evt.Event.FollowByName.Value,
				FollowByProfileImgId = evt.Event.FollowByImageId.Value,

				FollowToId = evt.Event.FollowToId.Value,
				FollowToName = evt.Event.FollowToName.Value,
				FollowToProfileImgId = evt.Event.FollowToImageId.Value
			});
		}

		public async Task HandleAsync(MessageEnvelope<UserUnfollowed> evt, CancellationToken ct)
		{         
			var key = new Follow.PrimaryKey();
			key.FollowById = evt.Id;
			key.FollowToId = evt.Event.FollowToId.Value;

			await followRepository.DeleteFollow(key);
		}

		public async Task HandleAsync(MessageEnvelope<BrandFollowed> evt, CancellationToken ct)
		{
			await followBrandRepository.Insert(new FollowBrand
			{
				Id = Guid.NewGuid(),
				Key = new FollowBrand.PrimaryKey {FollowByUserId = evt.Id, FollowToBrandId = evt.Event.BrandId.Value},
				FollowByUserId = evt.Id,
				FollowByUserName = evt.Event.FollowByName.Value,
				FollowByUserProfileImgId = evt.Event.FollowByImageId.Value,
				FollowToBrandId = evt.Event.BrandId.Value,
				FollowToBrandName = evt.Event.BrandName.Value,
				FollowToBrandLogoImgId = evt.Event.BrandLogoImgId.Value            
			});
		}

		public async Task HandleAsync(MessageEnvelope<BrandUnfollowed> evt, CancellationToken ct)
		{
			var key = new FollowBrand.PrimaryKey();
			key.FollowByUserId = evt.Id;
			key.FollowToBrandId = evt.Event.BrandId.Value;

			await followBrandRepository.DeleteFollow(key);
		}

		public async Task HandleAsync(MessageEnvelope<UserInfoEdited> evt, CancellationToken ct)
		{
			if (evt.Event.DidNameChange)
			{
				await followRepository.UpdateNameBatchAsync(evt.Id, evt.Event.Name.Value);
				await followBrandRepository.UpdateFollowByNameBatchAsync(evt.Id, evt.Event.Name.Value);
			}
		}

		public async Task HandleAsync(MessageEnvelope<UserImageCustomizationEdited> evt, CancellationToken ct)
		{
			if (evt.Event.ProfileImageChanged)
			{
				await followRepository.UpdateProfileImageBatchAsync(evt.Id, evt.Event.ProfileImageId.Value);
				await followBrandRepository.UpdateFollowByProfileImageBatchAsync(evt.Id, evt.Event.ProfileImageId.Value);
			}
		}

		public async Task HandleAsync(MessageEnvelope<BrandInfoUpdated> evt, CancellationToken ct)
		{
			if (evt.Event.DidNameChange)
			{
				await followBrandRepository.UpdateBrandNameBatchAsync(evt.Id, evt.Event.Name.Value);
			}
		}

		public async Task HandleAsync(MessageEnvelope<BrandImagesUpdated> evt, CancellationToken ct)
		{
			if (evt.Event.DidLogoImageIdChange)
			{
				await followBrandRepository.UpdateBrandLogoImageIdBatchAsync(evt.Id, evt.Event.LogoImageId.Value);
			}
		}
	}
}
