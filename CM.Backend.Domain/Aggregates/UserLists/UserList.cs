using System;
using System.Collections.Generic;
using System.Linq;
using CM.Backend.Domain.Aggregates.User.Commands;
using CM.Backend.Domain.Aggregates.UserLists.Commands;
using CM.Backend.Domain.Aggregates.UserLists.Entities;
using CM.Backend.Domain.Aggregates.UserLists.Events;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists
{
    public class UserList : Aggregate
    {   
        public AggregateId AuthorId { get; private set; }
        
        public ImageId ImageId { get; private set; }
        
        public NotEmptyString Title { get; private set; }
        
        public LimitedString Subtitle { get; private set; }
        
        public LimitedString Description { get; private set; }
        
        public FeaturedSchedule FeaturedSchedule { get; private set; }
        
        public UserListContentType ContentType { get; private set; }
        
        public UserListType ListType { get; private set; }
        
        public UserListAuthorType AuthorType { get; private set; }
        
        public OrderedSet<AggregateId> ListContent { get; private set; }
        
        public bool IsDeleted { get; private set; }


        public void Execute(CreateUserList cmd)
        {
            //Throw exception if this aggregate already have an idea, thus CreateUserList can not be called
            if (!Id.Equals(Guid.Empty))
            {
                throw new DomainException($"Executing {nameof(CreateUserList)} on an already created userList is not allowed");
            }
            CheckForDeletion();

            if (cmd.ListType.Value.Equals(UserListType.Top10List) && cmd.ListContent.Count > 10)
            {
                throw new DomainException($"Executing {nameof(CreateUserList)} with {nameof(UserListType)}:{UserListType.Top10List} is not possible when adding more than 10 bottles thus violating top 10.");
            }
            
            RaiseEvent(new UserListCreated(cmd.Id.Value, cmd.AuthorId, cmd.ImageId, cmd.Title, cmd.Subtitle,
                cmd.Description, cmd.FeaturedSchedule, cmd.ContentType, cmd.ListType, cmd.AuthorType, cmd.ListContent,
                cmd.IsDeleted));
        }

        public void Execute(EditUserList cmd)
        {
            CheckForDeletion();
            
            RaiseEvent(new UserListEdited(Id, cmd.ImageId, cmd.Title, cmd.Subtitle, cmd.Description));
        }

        public void Execute(DeleteUserList cmd)
        {
            CheckForDeletion();
            
            RaiseEvent(new UserListDeleted(Id, cmd.IsDeleted));
        }

        public void Execute(AddItem cmd)
        {
            CheckForDeletion();
            //At this level we dont care if the item is already there, at this point if the cmd is executing with a duplicate id no state will be altered,
            //If behaviour for duplicates should be produced this should be handled in the handler logic.
            
            //If list type is top10 then make sure that we will not overflow the cap of 10
            if (ListType.Value.Equals(UserListType.Top10List))
            {
                if (ListContent.Count == 10)
                {
                    throw new DomainException($"Executing {nameof(AddItem)} on a userList of {nameof(UserListType)}={ListType.Value}, with a listContent count of {ListContent.Count} is not allowed seeing as we would violate the top10 constraint");
                }
            }
            
            RaiseEvent(new ItemAdded(Id, cmd.Item));
        }

        public void Execute(RemoveItem cmd)
        {
            CheckForDeletion();
            //At this leve lwe dont care if the item is not here, at this point if the cmd is executing with a id not present, no state will be altered,
            //And thus if behaviour for non-present id, is expected this should be handled in the handler logic.
            
            RaiseEvent(new ItemRemoved(Id, cmd.Item));
        }

        public void Execute(UpdateListType cmd)
        {
            CheckForDeletion();
            
            //If changing to ListType.Top10 make sure that the list does not contain more than 10 champagnes
            if (cmd.UserListType.Value.Equals(UserListType.Top10List) && ListContent.Count > 10)
            {
                throw new DomainException($"Updating UserListType from:{ListType.Value} --> {cmd.UserListType.Value} is not allowed while listContent count is above 10, thus violating top 10 constraint");
            }
            
            RaiseEvent(new ListTypeUpdated(Id, cmd.UserListType));
        }

        public void Execute(SetFeaturedSchedule cmd)
        {
            CheckForDeletion();
            
            RaiseEvent(new FeaturedScheduleSet(Id, cmd.FeaturedSchedule));
        }

        public void Execute(UpdateItemIndex cmd)
        {
            CheckForDeletion();
            
            //Make sure that the new index to update is not out of bounds
            if (cmd.IndexPosition < 0 || cmd.IndexPosition > ListContent.Count - 1)
            {
                throw new ArgumentException($"Executing {nameof(UpdateItemIndex)} with new indexPosition:{cmd.IndexPosition} is invalid because the new indexPosition is out of bounds");
            }
            
            //At this level we dont care if the item is not present, no state will be altered, and thus if behaviour for non-present id, is expected this should be handled in the logic.
            RaiseEvent(new ItemIndexUpdated(Id, cmd.ItemId, cmd.IndexPosition));
        }

        public void CheckForDeletion()
        {
            if (IsDeleted)
            {
                throw new DomainException($"Executing commands on aggregate:{nameof(UserList)} is not possible when property:{nameof(IsDeleted)}={IsDeleted}");
            }
        }
        
        protected override void RegisterHandlers()
        {
            Handle<UserListCreated>(evt =>
            {
                Id = evt.Id;
                AuthorId = evt.AuthorId;
                ImageId = evt.ImageId;
                Title = evt.Title;
                Subtitle = evt.Subtitle;
                Description = evt.Description;
                FeaturedSchedule = evt.FeaturedSchedule;
                ContentType = evt.ContentType;
                ListType = evt.ListType;
                AuthorType = evt.AuthorType;
                ListContent = evt.Content;
                IsDeleted = evt.IsDeleted;
            });

            Handle<UserListEdited>(evt =>
            {
                ImageId = evt.ImageId;
                Title = evt.Title;
                Subtitle = evt.Subtitle;
                Description = evt.Description;
            });

            Handle<UserListDeleted>(evt => { IsDeleted = evt.IsDeleted; });

            Handle<ItemAdded>(evt => { ListContent.Add(evt.Item); });

            Handle<ItemRemoved>(evt => { ListContent.Remove(evt.Item); });

            Handle<ListTypeUpdated>(evt => { ListType = evt.UserListType; });

            Handle<FeaturedScheduleSet>(evt => { FeaturedSchedule = evt.FeaturedSchedule; });

            ///Running time --> O(n)
            Handle<ItemIndexUpdated>(evt =>
            {
                //Content as array
                var list = ListContent.ToArray();

                var key = ListContent.SingleOrDefault(x => x.Value.Equals(evt.ItemId.Value));
                var keyIndex = ListContent.IndexOf(evt.ItemId);
                
                AggregateId temp = null;

                if (key == null) //If key is null something is wrong, dont do anything in fear of messing up the state, rather do nothing
                {
                    return;
                }

                if (keyIndex > evt.Index) //Forwards linear scan from newIndex and swap too previous position
                {
                    for (int i = evt.Index; i < list.Length; i++)
                    {
                        if (list[i].Value.Equals(evt.ItemId.Value))
                        {
                            list[i] = key;
                            break;
                        }

                        temp = list[i];
                        list[i] = key;
                        key = temp;
                    }
                }
                else //Backwards linear scan
                {
                    for (int i = evt.Index; i >= 0; i--)
                    {
                        if (list[i].Value.Equals(evt.ItemId.Value))
                        {
                            list[i] = key;
                            break;
                        }

                        temp = list[i];
                        list[i] = key;
                        key = temp;
                    }
                }
                
                //Convert the updated array into an orderedSet again.
                var orderedSet = new OrderedSet<AggregateId>();

                foreach (var aggregateId in list)
                {
                    orderedSet.Add(aggregateId);
                }

                ListContent = orderedSet;
            });
        }
    }
}