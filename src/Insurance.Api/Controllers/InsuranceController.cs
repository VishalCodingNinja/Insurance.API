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
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IMapper mapper, IInsuranceService insuranceService)
        {
            _mapper = mapper;
            _insuranceService = insuranceService;
        }

        #region Insurance Calculation for Single Product

        /// <summary>
        /// Calculates insurance for a single product.
        /// </summary>
        /// <param name="insuranceRequest">The insurance request for the product.</param>
        /// <returns>The insurance details for the specified product.</returns>
        [HttpPost("product")]
        [SwaggerOperation(
            Summary = "Calculate insurance for a single product",
            Description = "Calculates the insurance value for a specific product based on its ID and other details.",
            OperationId = "CalculateInsuranceForProduct",
            Tags = new[] { "Insurance" }
        )]
        [SwaggerResponse(200, "Insurance details successfully calculated.", typeof(InsuranceDto))]
        [SwaggerResponse(400, "Invalid product ID.")]
        [SwaggerResponse(404, "Product not found.")]
        public async Task<IActionResult> CalculateInsuranceAsync([FromBody] InsuranceDto insuranceRequest)
        {
            if (!IsValidProductId(insuranceRequest.ProductId))
                return BadRequest(FormatInvalidProductMessage(insuranceRequest.ProductId));

            var insuranceModel = _mapper.Map<InsuranceModel>(insuranceRequest);
            var insuredProduct = await _insuranceService.CalculateInsuranceAsync(insuranceModel);

            if (insuredProduct == null)
                return NotFound(FormatProductNotFoundMessage(insuranceRequest.ProductId));

            var insuredDto = _mapper.Map<InsuranceDto>(insuredProduct);
            return Ok(insuredDto);
        }

        #endregion

        #region Insurance Calculation for Cart

        /// <summary>
        /// Calculates total insurance for a list of cart items.
        /// </summary>
        /// <param name="cartItems">The list of cart items to calculate insurance for.</param>
        /// <returns>The total insurance value for the cart.</returns>
        [HttpPost("cart")]
        [SwaggerOperation(
            Summary = "Calculate total insurance for a cart",
            Description = "Calculates the total insurance value for all items in a cart.",
            OperationId = "CalculateInsuranceForCart",
            Tags = new[] { "Insurance" }
        )]
        [SwaggerResponse(200, "Total insurance value successfully calculated.", typeof(TotalInsuranceResponse))]
        [SwaggerResponse(400, "Shopping cart is empty or insurance value is zero.")]

        public async Task<IActionResult> CalculateCartInsuranceAsync([FromBody] List<InsuranceDto> cartItems)
        {
            if (IsCartEmpty(cartItems))
                return BadRequest(ApiConstants.ShoppingCartEmpty);

            var insuranceModels = _mapper.Map<List<InsuranceModel>>(cartItems);
            var totalInsuranceValue = await _insuranceService.CalculateTotalInsuranceForCartAsync(insuranceModels);
            if (totalInsuranceValue <= 0)
            {
                return BadRequest(ApiConstants.ShoppingCartProductAreNotEligible);
            }
            return Ok(new TotalInsuranceResponse { TotalInsurance = totalInsuranceValue });

        }

        #endregion

        #region Helper Methods

        private bool IsValidProductId(int productId) => productId > 0;

        private string FormatInvalidProductMessage(int productId) =>
            string.Format(ApiConstants.InvalidProductData, productId);

        private string FormatProductNotFoundMessage(int productId) =>
            string.Format(ApiConstants.ProductNotFound, productId);

        private bool IsCartEmpty(List<InsuranceDto> cartItems) =>
            cartItems == null || cartItems.Count == 0;

        #endregion
    }
}
