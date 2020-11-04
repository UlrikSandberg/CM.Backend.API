using System;
namespace CM.Backend.API.Helpers
{
    public class ImageUploadResponse
    {
        public bool IsSuccesfull { get; private set; }
		public string Message { get; private set; }
		public int StatusCode { get; private set; }
		public Guid ImageId { get; private set; }

		public ImageUploadResponse(bool isSuccesfull, Guid imageId, string message = null, int statusCode = 0)
        {
            ImageId = imageId;
			StatusCode = statusCode;
			Message = message;
			IsSuccesfull = isSuccesfull;
		}
    }
}
