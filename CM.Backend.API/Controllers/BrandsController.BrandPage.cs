using System;
using System.Collections.Generic;
using CM.Backend.API.RequestModels;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CM.Backend.API.EnumOptions;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

//using CM.Backend.API.EntityTypeOfRegistry;

namespace CM.Backend.API.Controllers
{
	public partial class BrandsController
	{

		/// <summary>
		/// Creates a brand info page for the respective brandId. These pages will be visible besides the cellar card on the brands profile.
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="uiTemplate"></param>
		/// <param name="brandPage"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{brandId}/pages")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> CreateBrandPage(Guid brandId, PageTypes.BrandPageUITemplate uiTemplate, [FromBody]CreateBrandPageRequest brandPage)
		{

			var result = await CommandRouter.RouteAsync<CreateBrandPage, IdResponse>(new CreateBrandPage(
				brandId,
				brandPage.Title,
				brandPage.CardImgId,
				brandPage.HeaderImgId,
				uiTemplate.ToString()
				));

			if (!result.IsSuccessful)
			{
				Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(500, result.Message);
			}

			return CreatedAtAction(nameof(GetBrandPage), new { pageId = result.Id }, brandPage);
		}

		/// <summary>
		/// Toggle the publish status of the brands different brand info pages. This way you can publish and unpublish a brandInfo page. Only published pages are visible in the app
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <param name="setPublished"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{brandId}/pages/{pageId}/publishStatus")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> SetPublishingStatusBrandPage(Guid brandId, Guid pageId, bool setPublished = false)
		{

			var result = await CommandRouter.RouteAsync<SetPublishingStatusBrandPage, Response>(new SetPublishingStatusBrandPage(brandId, pageId, setPublished));

			if (!result.IsSuccessful)
			{
				Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(500, result.Message);
			}

			return StatusCode(200, result.IsSuccessful);
		}

		/// <summary>
		/// Returns a list of all published and unpublished brandpages for this brandId
		/// </summary>
		/// <param name="brandId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{brandId}/pages")]
		public async Task<IActionResult> GetBrandPages(Guid brandId)
		{

			var result = await QueryRouter.QueryAsync<GetBrandPages, IEnumerable<BrandPage>>(new GetBrandPages(brandId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Returns a specific brandInfo page respective to the pageId. Will return notFound if the pageId is not located within the provided brandId
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{brandId}/pages/{pageId}")]
		public async Task<IActionResult> GetBrandPage(Guid brandId, Guid pageId)
		{
			var result = await QueryRouter.QueryAsync<GetBrandPage, BrandPageExtendedSection>(new GetBrandPage(pageId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Update details around the infoPage itself
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <param name="editBrandPageRequest"></param>
		/// <param name="uiTemplateIdentifier"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("{brandId}/pages/{pageId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> EditPage(Guid brandId, Guid pageId, [FromBody]EditBrandPageRequest editBrandPageRequest, PageTypes.BrandPageUITemplate uiTemplateIdentifier)
		{
			var response = await CommandRouter.RouteAsync<EditBrandPage, IdResponse>(new EditBrandPage(pageId, editBrandPageRequest.Title, editBrandPageRequest.CardImgId, editBrandPageRequest.HeaderImgId, uiTemplateIdentifier.ToString()));

			if (!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(500);
			}

			return CreatedAtAction(nameof(GetBrandPage), new { brandId = brandId, pageId = pageId }, editBrandPageRequest);
		}

		/// <summary>
		/// Creates a brandInfoPage section for the provided brandId and adds it directly to the provided pageId
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{brandId}/pages/{pageId}/sections")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> CreateAndAddSection(Guid brandId, Guid pageId, [FromBody]CreateBrandPageSectionRequest section)
		{
			var result = await CommandRouter.RouteAsync<CreateAndAddSection, IdResponse>(new CreateAndAddSection(
				brandId,
				pageId,
				section.Title,
				section.SubTitle,
				section.Body,
				section.Champagnes,
				section.Images
			));

			if (result == null)
			{
				return NotFound();
			}

			return CreatedAtAction(nameof(GetBrandPageSection), new { sectionId = result.Id }, section);
		}

		/// <summary>
		/// Returns the specific brandInfoPage section specified by the sectionId
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="sectionId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{brandId}/sections/{sectionId}")]
		public async Task<IActionResult> GetBrandPageSection(Guid brandId, Guid sectionId)
		{

			var result = await QueryRouter.QueryAsync<GetBrandPageSection, BrandPageSection>(new GetBrandPageSection(sectionId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}
		/*
		[HttpDelete]
		[Route("{brandId}/pages/{pageId}/sections/{sectionId}")]
		public async Task<IActionResult> DeletePageSection(Guid brandId, Guid pageId, Guid sectionId)
		{
            
			var result = await CommandRouter.RouteAsync<DeleteSection, Response>(new DeleteSection(brandId, pageId, sectionId));
            
			if(!result.IsSuccessful)
			{
				return StatusCode(500);
			}

			return StatusCode(200, result.IsSuccessful);

		}
*/
		/// <summary>
		/// Edit a specific brandInfoPageSection identified by the sectionId
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="sectionId"></param>
		/// <param name="editSectionRequest"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("{brandId}/sections/{sectionId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> EditSection(Guid brandId, Guid sectionId, [FromBody]EditSectionRequest editSectionRequest)
		{
			var response = await CommandRouter.RouteAsync<EditSection, IdResponse>(new EditSection(sectionId, editSectionRequest.Title, editSectionRequest.SubTitle, editSectionRequest.Body, editSectionRequest.Images, editSectionRequest.Champagnes));

			if (!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(500);
			}

			return CreatedAtAction(nameof(GetBrandPageSection), new { brandId = brandId, sectionId = sectionId }, editSectionRequest);
		}

		/// <summary>
		/// Returns all brandInfoPageSections, accross all brandpages
		/// </summary>
		/// <param name="brandId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{brandId}/sections")]
		public async Task<IActionResult> GetSections(Guid brandId)
		{

			var result = await QueryRouter.QueryAsync<GetSections, IEnumerable<BrandPageSection>>(new GetSections(brandId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Create a brandInfoPage section, but it is not assigned to any brandInfoPage yet. For this you should either use the CreateAndAdd, or extract the id result from this method and add to brandInfoPage manually
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="createBrandPageSection"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{brandId}/sections")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> CreateSection(Guid brandId, [FromBody]CreateBrandPageSectionRequest createBrandPageSection)
		{

			var result = await CommandRouter.RouteAsync<CreateSection, IdResponse>(new CreateSection(brandId, createBrandPageSection.Title, createBrandPageSection.SubTitle, createBrandPageSection.Body, createBrandPageSection.Champagnes, createBrandPageSection.Images));

			if (!result.IsSuccessful)
			{
				return StatusCode(200, result.Message);
			}

			return CreatedAtAction(nameof(GetBrandPageSection), new { brandId = brandId, sectionId = result.Id }, createBrandPageSection);

		}

		/// <summary>
		/// Add an existing brandInfoPageSection identified by the sectionId to the identified pageId. This should be used for adding section ids from infoPages. Remeber this does not create the section itself
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <param name="sectionId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{brandId}/pages/{pageId}/sections/{sectionId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> AddSection(Guid brandId, Guid pageId, Guid sectionId)
		{

			var response = await CommandRouter.RouteAsync<AddSection, Response>(new AddSection(brandId, pageId, sectionId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(500, response.Message);
			}

			return StatusCode(200, response.Message);

		}

		/// <summary>
		/// Remove an existing brandInfopageSection identified by the sectionId from the identified pageId. This should be used for removing sections ids from infoPages. Remeber this does not delete the section itself
		/// </summary>
		/// <param name="brandId"></param>
		/// <param name="pageId"></param>
		/// <param name="sectionId"></param>
		/// <returns></returns>
		[HttpDelete]
		[Route("{brandId}/pages/{pageId}/sections/{sectionId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> RemoveSection(Guid brandId, Guid pageId, Guid sectionId)
		{

			var response = await CommandRouter.RouteAsync<RemoveSection, Response>(new RemoveSection(brandId, pageId, sectionId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error, {Message}", response.Message);
				return StatusCode(500, response.Message);
			}

			return StatusCode(200, response.Message);
		}
	}
}
