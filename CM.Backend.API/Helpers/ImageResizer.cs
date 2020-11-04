using System.IO;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace CM.Backend.API.Helpers
{
    public interface IImageResizer
    {
        Stream Resize(byte[] inputBytes, int width, int height);
    }

    public class ImageResizer : IImageResizer
    {
        private readonly ILogger<ImageResizer> _logger;

        public ImageResizer(ILogger<ImageResizer> logger)
        {
            _logger = logger;
        }

        public Stream Resize(byte[] inputBytes, int width, int height)
        {
            using (Image<Rgba32> image = Image.Load(inputBytes))
            {
                image.Mutate(op => op.Resize(new Size(width, height)));
                
                Stream ms = new MemoryStream();
                image.Save(ms, Image.DetectFormat(inputBytes));
                ms.Seek(0, SeekOrigin.Begin);
                
                
                _logger.LogInformation("Image resized");
                return ms;
            }
        }
    }
}