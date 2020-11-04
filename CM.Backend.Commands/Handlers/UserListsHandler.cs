using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.UserListCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.UserLists;
using CM.Backend.Domain.Aggregates.UserLists.Entities;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Handlers
{
    public class UserListsHandler : CommandHandlerBase,
        ICommandHandler<CreateUserList, IdResponse>,
        ICommandHandler<EditUserList, Response>,
        ICommandHandler<DeleteUserList, Response>,
        ICommandHandler<AddItem, Response>,
        ICommandHandler<RemoveItem, Response>,
        ICommandHandler<SetFeaturedSchedule, Response>,
        ICommandHandler<UpdateListType, Response>,
        ICommandHandler<UpdateItemIndex, Response>
    {
        
        public UserListsHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }
        
        public async Task<IdResponse> HandleAsync(CreateUserList cmd, CancellationToken ct)
        {
            var userList = new UserList();
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.CreateUserList(
                new AggregateId(Guid.NewGuid()),
                new AggregateId(cmd.AuthorId),
                new ImageId(cmd.ImageId),
                new NotEmptyString(cmd.Title),
                new LimitedString(cmd.Subtitle,50),
                new LimitedString(cmd.Description, 150),
                new FeaturedSchedule(false, DateTime.Now, DateTime.Now),
                new UserListContentType(UserListContentType.ChampagneContent),
                new UserListType(cmd.ListType),
                new OrderedSet<AggregateId>(),
                new UserListAuthorType(cmd.AuthorType)));

            await AggregateRepo.StoreAsync(userList);

            return new IdResponse(userList.Id);
        }

        public async Task<Response> HandleAsync(EditUserList cmd, CancellationToken ct)
        {
            var userlist = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);

            if (!userlist.AuthorId.Value.Equals(cmd.AuthorId))
            {
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }
            
            userlist.Execute(new Domain.Aggregates.UserLists.Commands.EditUserList(
                new ImageId(cmd.ImageId),
                new NotEmptyString(cmd.Title),
                new LimitedString(cmd.Subtitle, 50),
                new LimitedString(cmd.Description, 150)));

            await AggregateRepo.StoreAsync(userlist);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(DeleteUserList cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            if (!userList.AuthorId.Value.Equals(cmd.AuthorId))
            {
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.DeleteUserList(true));

            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(AddItem cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            if (!userList.AuthorId.Value.Equals(cmd.AuthorId))
            {
                //Critical, a person who shouldn't have access to this list just tried to update by adding an item without the priviliges to do it
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }

            if (userList.ListContent.ConvertToGuidList().Contains(cmd.ItemId))
            {
                //Indicate that the champagne is already a part of that list.
                return Response.Unsuccessful($"Item has already been added to this list");
            }
            
            if (userList.ListContent.Count > 10 && userList.ListType.Value.Equals(UserListType.Top10List))
            {
                //Error
                return Response.Unsuccessful($"A Top-10 list can not hold more than 10 champagnes. In order to add more champagnes to this list, convert to from a Top-10 list to a normal list");
            }
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.AddItem(new AggregateId(cmd.ItemId)));
            
            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(RemoveItem cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            if (!userList.AuthorId.Value.Equals(cmd.AuthorId))
            {
                //Critical, a person who shouldn't have access to this list just tried to update by adding an item without the priviliges to do it
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.RemoveItem(new AggregateId(cmd.ItemId)));

            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(SetFeaturedSchedule cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.SetFeaturedSchedule(new FeaturedSchedule(
                cmd.IsApprovedForFeature,
                cmd.FeatureStart,
                cmd.FeatureEnd)));

            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateListType cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            if (!userList.AuthorId.Value.Equals(cmd.AuthorId))
            {
                //Critical, a person who shouldn't have access to this list just tried to update by adding an item without the priviliges to do it
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }

            if (userList.ListContent.Count > 10 && cmd.ListType.Equals(UserListType.Top10List))
            {
                return Response.Unsuccessful($"Can not convert list to Top-10, because the list has more than 10 elements. Try to remove {userList.ListContent.Count - 10} items and then convert it again.");
            }
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.UpdateListType(new UserListType(cmd.ListType)));

            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateItemIndex cmd, CancellationToken ct)
        {
            var userList = await AggregateRepo.LoadAsync<UserList>(cmd.ListId);
            
            if (!userList.AuthorId.Value.Equals(cmd.AuthorId))
            {
                //Critical, a person who shouldn't have access to this list just tried to update by adding an item without the priviliges to do it
                return Response.Unsuccessful("Editing priviliges is reserved for the author only");
            }
            
            //Check if id of which to move index is present in the list
            if (!userList.ListContent.Contains(new AggregateId(cmd.ItemId)))
            {
                //Fatal --> This means that the state has been corrupted
                return Response.Unsuccessful($"A error has occured, it seems as if the itemId to move is not present in the list. Try again");   
            }
            
            userList.Execute(new Domain.Aggregates.UserLists.Commands.UpdateItemIndex(new AggregateId(cmd.ItemId), cmd.ItemIndex));

            await AggregateRepo.StoreAsync(userList);
            
            return Response.Success();
        }
    }
}