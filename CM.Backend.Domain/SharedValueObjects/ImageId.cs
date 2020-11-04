using System;

namespace CM.Backend.Domain.SharedValueObjects
{
    /// <summary>
    /// A valueobject representation of a Guid, that allows for the image to be of a empty guid but never a null guid.
    /// </summary>
    public class ImageId : SingleValueObject<Guid>
    {
        public ImageId(Guid value) : base(value)
        {
        }
    }
}