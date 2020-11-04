using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class EditUserList
    {
        public ImageId ImageId { get; }
        public NotEmptyString Title { get; }
        public LimitedString Subtitle { get; }
        public LimitedString Description { get; }

        public EditUserList(ImageId imageId, NotEmptyString title, LimitedString subtitle, LimitedString description)
        {
            ImageId = imageId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            
            foreach (var property in this.GetType().GetProperties())
            {
                if (property.GetValue(this) == null)
                {
                    throw new ArgumentException($"One or more values in {nameof(EditUserList)} constructor is null");
                }
            }
        }
    }
}