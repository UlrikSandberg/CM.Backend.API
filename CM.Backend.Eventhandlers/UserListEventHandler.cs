using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.UserLists.Events;
using CM.Backend.Persistence.Model.UserList;
using CM.Backend.Persistence.Model.UserList.Entities;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class UserListEventHandler :
        IEventHandler<MessageEnvelope<UserListCreated>>,
        IEventHandler<MessageEnvelope<UserListEdited>>,
        IEventHandler<MessageEnvelope<UserListDeleted>>,
        IEventHandler<MessageEnvelope<ItemAdded>>,
        IEventHandler<MessageEnvelope<ItemRemoved>>,
        IEventHandler<MessageEnvelope<ItemIndexUpdated>>,
        IEventHandler<MessageEnvelope<FeaturedScheduleSet>>,
        IEventHandler<MessageEnvelope<ListTypeUpdated>>

    {
        private readonly IUserListRepository _userListRepository;

        public UserListEventHandler(IUserListRepository userListRepository)
        {
            _userListRepository = userListRepository;
        }

        public async Task HandleAsync(MessageEnvelope<UserListCreated> evt, CancellationToken ct)
        {
            await _userListRepository.Insert(new UserList
            {
                Id = evt.Id,
                AuthorId = evt.Event.AuthorId.Value,
                AuthorType = evt.Event.AuthorType.Value,
                ContentType = evt.Event.ContentType.Value,
                Description = evt.Event.Description.Value,
                FeaturedSchedule = new FeaturedSchedule
                {
                    ApprovedForFeature = evt.Event.FeaturedSchedule.ApprovedForFeature,
                    FeatureEnd = evt.Event.FeaturedSchedule.FeatureEnd,
                    FeatureStart = evt.Event.FeaturedSchedule.FeatureStart
                },
                ImageId = evt.Event.ImageId.Value,
                ListContent = new List<Guid>(evt.Event.Content.ConvertToGuidList()),
                ListType = evt.Event.ListType.Value,
                Subtitle = evt.Event.Subtitle.Value,
                Title = evt.Event.Title.Value,
                CreatedAt = DateTime.UtcNow,
                LastEditedAt = DateTime.UtcNow
            });
        }

        public async Task HandleAsync(MessageEnvelope<UserListEdited> evt, CancellationToken ct)
        {
            await _userListRepository.EditUserList(evt.Id, evt.Event.Title.Value, evt.Event.Subtitle.Value, evt.Event.Description.Value,
                evt.Event.ImageId.Value);
        }

        public async Task HandleAsync(MessageEnvelope<UserListDeleted> evt, CancellationToken ct)
        {
            await _userListRepository.Delete(evt.Id);
        }

        public async Task HandleAsync(MessageEnvelope<ItemAdded> evt, CancellationToken ct)
        {
            await _userListRepository.AddItem(evt.Id, evt.Event.Item.Value);
        }

        public async Task HandleAsync(MessageEnvelope<ItemRemoved> evt, CancellationToken ct)
        {
            await _userListRepository.RemoveItem(evt.Id, evt.Event.Item.Value);
        }

        public async Task HandleAsync(MessageEnvelope<ItemIndexUpdated> evt, CancellationToken ct)
        {
            await _userListRepository.UpdateItemIndex(evt.Id, evt.Event.ItemId.Value, evt.Event.Index);
        }

        public async Task HandleAsync(MessageEnvelope<FeaturedScheduleSet> evt, CancellationToken ct)
        {
            await _userListRepository.SetFeaturedSchedule(evt.Id, new FeaturedSchedule
            {
                ApprovedForFeature = evt.Event.FeaturedSchedule.ApprovedForFeature,
                FeatureEnd = evt.Event.FeaturedSchedule.FeatureEnd,
                FeatureStart = evt.Event.FeaturedSchedule.FeatureStart
            });
        }

        public async Task HandleAsync(MessageEnvelope<ListTypeUpdated> evt, CancellationToken ct)
        {
            await _userListRepository.UpdateUserListType(evt.Id, evt.Event.UserListType.Value);
        }
    }
}