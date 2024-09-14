using AutoMapper;
using Insurance.Api.Consts;
using Insurance.Api.Dtos;
using Insurance.BusinessLayer.Models;
using Insurance.BusinessLayer.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Api.Controllers
{
    /// <summary>
    /// API Controller to handle surcharge-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SurchargeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISurchargeService _surchargeService;

        /// <summary>
        /// Constructor to initialize the controller with required services.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="surchargeService">The surcharge service.</param>
        public SurchargeController(IMapper mapper, ISurchargeService surchargeService)
        {
            _mapper = mapper;
            _surchargeService = surchargeService;
        }

        /// <summary>
        /// Uploads surcharge rates for product types.
        /// </summary>
        /// <param name="surchargeRates">The list of surcharge rates to be uploaded.</param>
        /// <returns>A result indicating whether the upload was successful or not.</returns>
        [HttpPost("upload")]
        [SwaggerOperation(
            Summary = "Uploads surcharge rates for product types",
            Description = "Uploads a list of surcharge rates for product types.",
            OperationId = "UploadSurchargeRates",
            Tags = new[] { "Surcharge" }
        )]
        [SwaggerResponse(200, "Surcharge rates successfully saved.")]
        [SwaggerResponse(400, "Invalid surcharge data.")]
        public async Task<IActionResult> UploadSurchargeRatesAsync([FromBody] List<ProductSurchargeDto> surchargeRates)
        {
            // Validate the input data.
            if (surchargeRates == null || surchargeRates.Count == 0)
                return BadRequest(ApiConstants.InvalidSurchargeData);

            // Map DTOs to service models.
            var surchargeRateModels = _mapper.Map<List<SurchargeModel>>(surchargeRates);

            // Call the surcharge service to upload the rates.
            await _surchargeService.UploadSurchargeRatesAsync(surchargeRateModels);

            // Return a success message.
            return Ok(ApiConstants.SurchargeSuccessfulSaved);
        }

        /// <summary>
        /// Retrieves the surcharge for a given product type by its ID.
        /// </summary>
        /// <param name="productTypeId">The ID of the product type.</param>
        /// <returns>The surcharge for the given product type.</returns>
        [HttpGet("{productTypeId}")]
        [SwaggerOperation(
            Summary = "Gets the surcharge for a product type",
            Description = "Retrieves the surcharge for a specific product type by its ID.",
            OperationId = "GetSurchargeForProductType",
            Tags = new[] { "Surcharge" }
        )]
        [SwaggerResponse(200, "Surcharge successfully retrieved.", typeof(ProductSurchargeDto))]
        [SwaggerResponse(400, "Product ID cannot be empty.")]
        public async Task<IActionResult> GetSurchargeForProductTypeAsync([FromRoute] int productTypeId)
        {
            // Validate the product type ID.
            if (productTypeId <= 0)
                return BadRequest(ApiConstants.ProductIdCanNotBeEmpty);

            // Call the surcharge service to retrieve the surcharge.
            var surcharge = await _surchargeService.GetSurchargeForProductTypeAsync(productTypeId);

            // Create the response DTO and return the result.
            var productSurchargeDto = new ProductSurchargeDto
            {
                ProductTypeId = productTypeId,
                Surcharge = surcharge
            };

            return Ok(productSurchargeDto);
        }
    }
}
