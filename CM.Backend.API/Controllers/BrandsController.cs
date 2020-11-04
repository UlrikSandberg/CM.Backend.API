using System;
using System.Collections.Generic;
using CM.Backend.API.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using CM.Backend.API.ActionFilters;
using SixLabors.ImageSharp;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;
using CM.Backend.API.Helpers;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.RequestModels.BrandRequestModels;
using CM.Backend.Commands.Commands.BrandCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Queries.Queries.BrandQueries;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace CM.Backend.API.Controllers
{
	
    [Route("api/v1/brands")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
    public partial class BrandsController : ControllerBase
    {
	    private readonly IOptions<ProjectionsPersistenceConfiguration> _projectionsConfig;
	    private readonly IBrandTypes brandTypes;

        public BrandsController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, IOptions<ProjectionsPersistenceConfiguration> projectionsConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
	        _projectionsConfig = projectionsConfig;
	        this.brandTypes = new BrandTypes();
        }

	    //***** QUERY SECTION *****

	    [HttpGet]
	    [Route("search")]
	    public async Task<IActionResult> SearchBrands(string searchText, int page, int pageSize)
	    {
		    var result =
			    await QueryRouter.QueryAsync<SearchBrands, IEnumerable<BrandSearchProjectionModel>>(new SearchBrands(searchText, page,
				    pageSize));

		    if (result == null)
		    {
			    return NotFound();
		    }
		    
		    return new ObjectResult(result);
	    }
	    
	    [HttpGet]
	    [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId", "page", "pageSize"})]
	    [Route("{brandId}/followers")]
	    public async Task<IActionResult> GetBrandFollowers(Guid brandId, int page = 0, int pageSize = 50)
	    {
		    var result = await QueryRouter.QueryAsync<GetBrandFollowers, IEnumerable<FollowersQueryModel>>(new GetBrandFollowers(RequestingUserId, brandId, page, pageSize));

		    if (result == null)
		    {
			    return NotFound();
		    }
		    
		    return new ObjectResult(result);

	    }

        [HttpGet]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId"})]
        [Route("{brandId}")]
        public async Task<IActionResult> GetBrand(Guid brandId)
        {
            var brand = await QueryRouter.QueryAsync<GetBrandLight, BrandLight>(new GetBrandLight(brandId));

            if (brand == null)
            {
                return NotFound();
            }

            return new ObjectResult(brand);
        }
	    
	    [HttpGet]
	    //[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId"})] TODO : This should be cached but since there are some user specific details we can't move the user specific details to its own endpoint
	    [Route("{brandId}/profile")]
	    public async Task<IActionResult> GetBrandProfile(Guid brandId)
	    {
		    var brand = await QueryRouter.QueryAsync<GetBrand, BrandProfileExtendedBrandPage>(new GetBrand(brandId, RequestingUserId));

		    if(brand == null)
		    {
			    return NotFound();
		    }

		    return new ObjectResult(brand);
	    }

        [HttpGet]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"page", "pageSize", "includeUnpublished", "sortAscending"})]
        [Route("")]
        public async Task<IActionResult> GetAll(int page, int pageSize, bool includeUnpublished, bool sortAscending)
        {
            var result = await QueryRouter.QueryAsync<GetAllBrands, IEnumerable<BrandLight>>(new GetAllBrands(page, pageSize, includeUnpublished, sortAscending));

            if (result == null)
            {
                return NotFound();
            }

            return new ObjectResult(result);
        }

	    [HttpGet]
	    [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId"})]
	    [Route("{brandId}/cellar")]
	    public async Task<IActionResult> GetCellar(Guid brandId)
	    {
		    var result = await QueryRouter.QueryAsync<GetCellar, Cellar>(new GetCellar(brandId));

		    if (result == null)
		    {
			    return NotFound();
		    }

		    return new ObjectResult(result);
	    }
	    
	    [HttpGet]
	    [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId"})]
	    [Route("{brandId}/cellar/cellarSections")]
	    public async Task<IActionResult> GetCellarSections(Guid brandId)
	    {
		    var result = await QueryRouter.QueryAsync<GetBrandCellarSections, IEnumerable<CellarSection>>(new GetBrandCellarSections(brandId));

		    if (result == null)
		    {
			    return NotFound();
		    }
		    
		    return new ObjectResult(result); 
	    }

	    [HttpGet]
	    [ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId", "sectionId"})]
	    [Route("{brandId}/cellar/cellarSections/{sectionId}")]
	    public async Task<IActionResult> GetCellarSection(Guid brandId, Guid sectionId)
	    {
		    var result = await QueryRouter.QueryAsync<GetBrandCellarSection, CellarSection>(new GetBrandCellarSection(brandId, sectionId));

		    if (result == null)
		    {
			    return NotFound();
		    }
		    
		    return new ObjectResult(result);
	    }
	    
	    [HttpGet]
	    [Route("{brandId}/images/{imageId}")]
	    public async Task<IActionResult> GetImage(Guid brandId, Guid imageId)
	    {

		    var result = await QueryRouter.QueryAsync<GetBrandImage, BrandImage>(new GetBrandImage(imageId, brandId));

		    if(result == null)
		    {
			    return NotFound();
		    }

		    return new ObjectResult(result);
	    }

	    [HttpGet]
	    [Route("{brandId}/images")]
	    public async Task<IActionResult> GetImages(Guid brandId)
	    {

		    var result = await QueryRouter.QueryAsync<GetBrandImages, IEnumerable<BrandImage>>(new GetBrandImages(brandId));

		    if(result == null)
		    {
			    return NotFound();
		    }

		    return new ObjectResult(result);
                 
	    }

	    //********************* 
	    // COMMAND SECTION 
	    //*********************
	    
        [HttpPost]
        [Route("")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> CreateBrand([FromBody]CreateBrandRequest brand)
        {
            var response = await CommandRouter.RouteAsync<CreateBrand, IdResponse>
	            (new CreateBrand(
	            brand.Name,
	            brand.ProfileText,
	            brand.FacebookUrl,
	            brand.InstagramUrl,
	            brand.PinterestUrl,
	            brand.TwitterUrl,
	            brand.WebsiteUrl));

            if (!response.IsSuccessful)
                return StatusCode(500, response.Message);

            return StatusCode(201, response.Id);
        }
        
        [HttpPut]
        [Route("{brandId}")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> UpdateBrandInfo(Guid brandId, [FromBody]UpdateBrandInfoRequest request)
        {
	        var response =
		        await CommandRouter.RouteAsync<UpdateBrandInfo, Response>(new UpdateBrandInfo(brandId, request.Name,
			        request.BrandProfileText));

            if(!response.IsSuccessful)
            {
                return StatusCode(500, response.Message);
            }

	        return StatusCode(200);

        }

	    [HttpPut]
	    [Route("{brandId}/brandImages")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> UpdateBrandImages(Guid brandId, [FromBody]UpdateBrandImagesRequest request)
	    {
		    var response = await CommandRouter.RouteAsync<UpdateBrandImages, Response>(new UpdateBrandImages(
				brandId,
			    request.BrandCoverImageId,
			    request.BrandListImageId,
			    request.BottleCoverImageId,
			    request.BrandLogoImageId));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(201);

	    }

	    [HttpPut]
	    [Route("{brandId}/brandSocial")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> UpdateBrandSocial(Guid brandId, [FromBody] UpdateBrandSocialRequest request)
	    {
		    var response = await CommandRouter.RouteAsync<UpdateBrandSocial, Response>(new UpdateBrandSocial(
			    brandId,
			    request.FacebookUrl,
			    request.InstagramUrl,
			    request.TwitterUrl,
			    request.PinterestUrl,
			    request.WebsiteUrl));
		    
		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(201);
	    }
	    
	    [HttpPut]
	    [Route("{brandId}/cellar")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> UpdateCellar(Guid brandId, [FromBody]UpdateBrandCellarRequest request)
	    {

		    var response =
			    await CommandRouter.RouteAsync<UpdateBrandCellar, Response>(
				    new UpdateBrandCellar(brandId, request.CardImgId, request.HeaderImgId));
			    
		    if(!response.IsSuccessful)
		    {
			    return StatusCode(500);
		    }

		    return StatusCode(201, response.Message);

	    }

        [HttpPost]
        [Route("{brandId}/publishStatus")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> SetPublishingStatusBrand(Guid brandId, BrandPublishingRequest.PublishingStatusEnum publishingStatus)
        {

            bool publishStatus = publishingStatus == BrandPublishingRequest.PublishingStatusEnum.Published;

	        var response = await CommandRouter.RouteAsync<SetPublishingStatusBrand, Response>(new SetPublishingStatusBrand(brandId, publishStatus));

            if(!response.IsSuccessful)
            {
                return StatusCode(500, response.Message);
            }

	        return StatusCode(200);
        }

	    [HttpPost]
	    [Route("{brandId}/cellar/cellarSections")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> CreateAndAddCellarSection(Guid brandId, [FromBody]CreateAndAddCellarSectionRequest request)
	    {
		    var response = await CommandRouter.RouteAsync<CreateAndAddBrandCellarSection, IdResponse>(
			    new CreateAndAddBrandCellarSection(
				    brandId,
				    request.Title,
				    request.Body,
				    request.Champagnes));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }
		    
		    return new ObjectResult(response.Id);
	    }

	    [HttpPut]
	    [Route("{brandId}/cellar/cellarSections/{sectionId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> UpdateCellarSection(Guid brandId, Guid sectionId,
		    [FromBody]UpdateCellarSectionRequest request)
	    {
		    var response = await CommandRouter.RouteAsync<UpdateBrandCellarSection, Response>(
			    new UpdateBrandCellarSection(
				    brandId,
				    sectionId,
				    request.Title,
				    request.Body,
				    request.Champagnes));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }
		    
		    return StatusCode(201, response.Message);
	    }

	    [HttpDelete]
	    [Route("{brandId}/cellar/cellarSections/{sectionId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
	    public async Task<IActionResult> DeleteCellarSection(Guid brandId, Guid sectionId)
	    {
		    var response =
			    await CommandRouter.RouteAsync<DeleteBrandCellarSection, Response>(
				    new DeleteBrandCellarSection(brandId, sectionId));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(201, response.Message);
	    }

        /// <summary>
        /// Uploads the images to the brand container. Uploaded file must be of type .jpg or .png
        /// </summary>
        /// <returns>The images.</returns>
        /// <param name="brandId">Brand identifier.</param>
        /// <param name="imageName">Image name.</param>
        /// <param name="typeOfBrandImage">Type of brand image. Requested for image resizing strategy later</param>
        /// <param name="maintainFileFormatRequest"> All images except champagnes are converted to jpeg to save disk space and optimize load performance by default. Toggle to maintain filetype anyway. Does not affect champagnes. Notice that this option only has in app effect on brandInfoPages.</param>
        /// <param name="image">Image file.</param>
        [HttpPost]
		[AddSwaggerFileUploadButton]
        [Route("{brandId}/images")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> UploadImages(IFormFile file, Guid brandId, string imageName, BrandTypes.TypeOfBrandImage typeOfBrandImage, bool maintainFileFormatRequest)
        {
           
	        //Since the new global exception filter has been implemented try - catch blocks has been removed. Instead the exception will wander till they a captured in the global exception middleware where they are logged
	        
            string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');
            
            if (!fileExtension.Equals("jpg") && !fileExtension.Equals("jpeg") && !fileExtension.Equals("png"))
            {
                return StatusCode(415, "Incompatible file format. File must be of type .jpg/.jpeg or .png");
            }

            var isSuccesfull = false;
            CloudStorageAccount cloudStorageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
	        var storageConnectionString = _projectionsConfig.Value.AzureStorageAccountConnectionString;
            Guid fileId = Guid.Empty;

			//Establish connection to the blob storage
			if (CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount))
			{
				
				//Init blobClient
				CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
				cloudBlobContainer = cloudBlobClient.GetContainerReference("brand-files");

				//Connection established, read file stream and upload file to cloudBlobContainer.
				using (var ms = new MemoryStream())
				{
					
					await file.CopyToAsync(ms);

					fileId = Guid.NewGuid();
					var image = ms.ToArray();
					
					
					//If imageType is not champagne and file type is png convert to jpg.
					if (fileExtension.Equals("png") && typeOfBrandImage != BrandTypes.TypeOfBrandImage.Champagne && !maintainFileFormatRequest)
					{
						using (var conversionMS = new MemoryStream())
						{
							var jpg = Image.Load(image);
							jpg.SaveAsJpeg(conversionMS);
							image = conversionMS.ToArray();
							fileExtension = "jpg";
						}
					}
					
					//If imageType is champagne and fileType equal jpg convert champagne image to png.
					if (fileExtension.Equals("jpg") && typeOfBrandImage == BrandTypes.TypeOfBrandImage.Champagne)
					{
						using (var conversionMS = new MemoryStream())
						{
							var jpg = Image.Load(image);
							jpg.SaveAsPng(conversionMS);
							image = conversionMS.ToArray();
							fileExtension = "png";
						}
					}
					
					var fileName = "original." + fileExtension;

					CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(brandId.ToString() + "/" + "images" + "/" + fileId.ToString() + "/" + fileName);
					
					await cloudBlockBlob.UploadFromByteArrayAsync(image, 0, image.Length);
					
				}
			}
			
			var response = await CommandRouter.RouteAsync<UploadBrandImage, IdResponse>(new UploadBrandImage(fileId, brandId, imageName, typeOfBrandImage.ToString(), fileExtension));
			return StatusCode(200, fileId);
        }

		[HttpDelete]
		[Route("{brandId}/images/{imageId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> DeleteImage(Guid brandId, Guid imageId)
		{
         
            //We can't delete without verification since container settings are blob settings. Finish after app setup...

			CloudStorageAccount cloudStorageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

            var isSuccesfull = false;
			var storageConnectionString = _projectionsConfig.Value.AzureStorageAccountConnectionString;

			if(CloudStorageAccount.TryParse(storageConnectionString, out cloudStorageAccount))
			{
				try
				{
					//Init blobClient
					CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
					cloudBlobContainer = cloudBlobClient.GetContainerReference("brand-files");
					BlobContinuationToken blobContinuationToken = null;

					do
					{
						var blobs = await cloudBlobContainer.ListBlobsSegmentedAsync(brandId.ToString() +"/"+"images"+"/"+imageId.ToString()+"/", blobContinuationToken);
						blobContinuationToken = blobs.ContinuationToken;

						foreach (IListBlobItem blob in blobs.Results)
						{
							Console.WriteLine(blob.Uri);
							CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blob.Uri.ToString());
							await cloudBlockBlob.DeleteIfExistsAsync();
						}
					}
					while (blobContinuationToken != null);           
               
					isSuccesfull = true;
				}
				catch (StorageException ex)
				{
					Console.WriteLine(ex.StackTrace);
				}

				if(isSuccesfull)
				{
					return StatusCode(200);
				}
			}
			
			return StatusCode(200);
		}
    }
}
