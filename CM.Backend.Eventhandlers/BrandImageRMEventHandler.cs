using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using SixLabors.ImageSharp;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Events;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.EventHandlers.Helpers;
using CM.Backend.Persistence.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Serilog;
using SimpleSoft.Mediator;
using SixLabors.ImageSharp.PixelFormats;

namespace CM.Backend.EventHandlers
{
    public class BrandImageRMEventHandler : IEventHandler<MessageEnvelope<ImageUploaded>>
    {
        private readonly IBrandImageRepository brandImageRepository;
        private readonly IOptions<ProjectionsPersistenceConfiguration> config;
        private readonly ImageResizer imageResizer;
	    private readonly ILogger _logger;


	    private const string Champagne = "Champagne";
	    private const string Cover = "Cover";
	    private const string Card = "Card";
	    private const string Logo = "Logo";

	    public BrandImageRMEventHandler(IBrandImageRepository brandImageRepository, IOptions<ProjectionsPersistenceConfiguration> config, ImageResizer imageResizer, ILogger logger)
        {
            this.imageResizer = imageResizer;
	        _logger = logger;
	        this.config = config;
            this.brandImageRepository = brandImageRepository;
        }

        public async Task HandleAsync(MessageEnvelope<ImageUploaded> evt, CancellationToken ct)
        {
            await brandImageRepository.Insert(new BrandImage
            {
                Id = evt.Id,
                BrandId = evt.Event.BrandId.Value,
                Name = evt.Event.ImageName.Value,
                TypeOfBrandImage = evt.Event.TypeOfBrandImage.Value,
                FileType = evt.Event.FileType.Value
            });

            await ResizeImage(evt);
        }

        private async Task ResizeImage(MessageEnvelope<ImageUploaded> evt)
        {

            CloudStorageAccount cloudStorageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

	        var storageConnectionString = config.Value.AzureStorageAccountConnectionString;
            
            //Establish connection to the blob storage
            if(CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount))
            {
                try
                {
                    //Init blobClient
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("brand-files");
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value+"/"+ "images" + "/"+ evt.Id + "/" + "original."+evt.Event.FileType.Value);

                    using(var ms = new MemoryStream())
                    {
                        await cloudBlockBlob.DownloadToStreamAsync(ms);

                        var file = ms.ToArray();

						if (evt.Event.TypeOfBrandImage.Value.Equals(Champagne))
						{

							//The champagne are resized according to a W:H Ratio of 1:2,5. A champagne image resized is therefor 2,5 times higher than wide.
							var smallImg = imageResizer.Resize(file, 160, 400);
							var largeImg = imageResizer.Resize(file, 320, 800);

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "small." + evt.Event.FileType.Value);
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "large." + evt.Event.FileType.Value);

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();

						}
						else if (evt.Event.TypeOfBrandImage.Value.Equals(Cover))
						{
							//Cover images are set to aspectFill in the app which means that the images should be resized according to original ratio
							//This means we can only control either the H or W. Since covers are represented as horizontal images
							//the width has been locked to pre defined sizes, whereas the height is calculated based on image orignal ratio.

							double imageRatio = 0;

							//Determine ratio of uploaded cover img
							using (Image<Rgba32> image = Image.Load(file))
							{
								imageRatio = (double)image.Height / (double)image.Width;
							}
                            
							double smallWidth = 500;
							double largeWidth = 800;

							var smallImg = imageResizer.Resize(file, (int)smallWidth, (int)(smallWidth * imageRatio));
							var largeImg = imageResizer.Resize(file, (int)largeWidth, (int)(largeWidth * imageRatio));

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "small." + evt.Event.FileType.Value);
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "large." + evt.Event.FileType.Value);

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
						else if (evt.Event.TypeOfBrandImage.Value.Equals(Card))
						{

							//Card images are set to aspectFill in the app which means that the images should be resized according to original ratio
							//This means we can only control either the H or W. Since cards are represented as vertical images
							//the Height has been locked to pre defined sizes, whereas the width is calculated based on image orignal ratio.
                            
							double imageRatio = 0;
							//Determine ratio of uploaded cover img
							using (Image<Rgba32> image = Image.Load(file))
							{
								imageRatio = (double)image.Width / (double)image.Height;
							}
                            
							double smallHeight = 600;
							double largeHeight = 800;
                            
							var smallImg = imageResizer.Resize(file, (int)(smallHeight * imageRatio), (int)(smallHeight));
							var largeImg = imageResizer.Resize(file, (int)(largeHeight * imageRatio), (int)(largeHeight));

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "small." + evt.Event.FileType.Value);
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "large." + evt.Event.FileType.Value);

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
						else if (evt.Event.TypeOfBrandImage.Value.Equals(Logo))
						{
							//A logo is allways resized according to W:H ratio of 1:1 = quadratic.
							var smallImg = imageResizer.Resize(file, 400, 400);
							var largeImg = imageResizer.Resize(file, 800, 800);

							//Create new blobBlockReference and upload the new files!
							CloudBlockBlob smallBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "small." + evt.Event.FileType.Value);
							CloudBlockBlob largeBlockBlob = cloudBlobContainer.GetBlockBlobReference(evt.Event.BrandId.Value + "/" + "images" + "/" + evt.Id + "/" + "large." + evt.Event.FileType.Value);

							await smallBlockBlob.UploadFromStreamAsync(smallImg);
							await largeBlockBlob.UploadFromStreamAsync(largeImg);

                            smallImg.Dispose();
                            largeImg.Dispose();
						}
                    }               
                } 
                catch(StorageException ex)
                {
                    Console.WriteLine(ex.Message);
	                _logger.Fatal(ex , "Internal error trying to upload a small and large images from {@Evt}", evt);
                }
            }         
        }
    }
}