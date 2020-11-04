using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects
{
    public class FileType : SingleValueObject<string>
    {
        private const string JPG = "jpg";
        private const string JPEG = "jpeg";
        private const string PNG = "png";
        
        public FileType(string value) : base(value)
        {
            if (!string.Equals(value, JPG) && !string.Equals(value, JPEG) && !string.Equals(value, PNG))
            {
                throw new ArgumentException(nameof(value) + ": Filetype must be of type jpg/jpeg or png");
            }
        }
    }
}