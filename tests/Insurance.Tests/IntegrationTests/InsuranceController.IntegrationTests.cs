using Insurance.Api.Consts;
using Insurance.Api.Dtos;
using Insurance.Tests.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Api.IntegrationTests
{
    public class InsuranceControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly HttpClient _client;

        public InsuranceControllerTests(ControllerTestFixture fixture)
        {
            _client = fixture.Client;
        }

        #region Insurance Calculation Tests

        // Task 1 [BUGFIX]
        [Fact]
        public async Task CalculateInsuranceAsync_GivenLaptopUnder500Euro_ShouldReturn1000EuroInsurance()
        {
            // Arrange
            var productId = 837856; // Lenovo Chromebook C330-11 81HY000MMH, correctly categorized as Laptop
            var requestData = new InsuranceDto { ProductId = productId };

            // Act
            var endpoint = $"{TestConstants.ProductInsuranceRoute}";
            var response = await SendRequestAsync(endpoint, requestData);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<InsuranceDto>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000, result.InsuranceValue); // 500 (insurance) + 500 (additional laptop insurance)
        }

        [Fact]
        public async Task CalculateInsuranceAsync_GivenSmartPhoneMoreThan500Euro_ShouldReturn1500EuroInsurance()
        {
            // Arrange
            var productId = 827074; // Samsung Galaxy S10 Plus 128 GB Black, correctly categorized as SmartPhone
            var requestData = new InsuranceDto { ProductId = productId };

            // Act
            var endpoint = $"{TestConstants.ProductInsuranceRoute}";
            var response = await SendRequestAsync(endpoint, requestData);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<InsuranceDto>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1500, result.InsuranceValue); // 1000 (insurance) + 500 (additional smartphone insurance charges)
        }

        [Fact]
        public async Task CalculateInsurance_ValidProductId_ReturnsInsuranceValueZero()
        {
            // Arrange
            var productId = 572770;// sales price is lesser then 500 and not a laptop sp it should return 0
            var endpoint = $"{TestConstants.ProductInsuranceRoute}";
            var requestData = new InsuranceDto { ProductId = productId };

            // Act
            var response = await SendRequestAsync(endpoint, requestData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            AssertSuccessResponse<InsuranceDto>(response, responseContent, dto => dto.InsuranceValue == 0);
        }

        [Fact]
        public async Task CalculateInsurance_UninsurableProduct_ReturnsInsuranceValueZero()
        {
            // Arrange
            var productId = 735296;
            var endpoint = $"{TestConstants.ProductInsuranceRoute}";
            var requestData = new InsuranceDto { ProductId = productId };

            // Act
            var response = await SendRequestAsync(endpoint, requestData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            AssertSuccessResponse<InsuranceDto>(response, responseContent, dto => dto.InsuranceValue == 0);
        }

        [Fact]
        public async Task CalculateInsurance_InvalidProductId_ReturnsBadRequest()
        {
            // Arrange
            var productId = -1;
            var endpoint = $"{TestConstants.ProductInsuranceRoute}";
            var requestData = new InsuranceDto { ProductId = productId };

            // Act
            var response = await SendRequestAsync(endpoint, requestData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            AssertErrorResponse(response, HttpStatusCode.BadRequest);
        }

        #endregion

        #region Cart Insurance Calculation Tests

        [Fact]
        public async Task CalculateCartInsurance_WithDigitalCamera_ShouldApplyCameraSurcharge()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = 836194 }, // Sony CyberShot DSC-RX100 VII (Digital camera)
                new InsuranceDto { ProductId = 725435 }  // Cowon Plenue D Gold (MP3 player, non-insurable)
            };
            var endpoint = $"{TestConstants.ProductInsuranceCartRoute}";

            // Act
            var response = await SendRequestAsync(endpoint, cartItems);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Insurance calculation
            var expectedTotalInsurance = 1000 + 500; // (1000)Base for camera + (500) camera surcharge
            var totalInsuranceResponse = JsonSerializer.Deserialize<TotalInsuranceResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.NotNull(totalInsuranceResponse);
            Assert.Equal(expectedTotalInsurance, totalInsuranceResponse.TotalInsurance);
        }


        [Fact]
        public async Task CalculateCartInsurance_SingleProductLessThan500_ReturnsBadRequestWithCorrectMessage()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = 572770 }
            };
            var endpoint = $"{TestConstants.ProductInsuranceCartRoute}";

            // Act
            var response = await SendRequestAsync(endpoint, cartItems);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Verify that the response content matches the expected error message
            Assert.Contains(ApiConstants.ShoppingCartProductAreNotEligible, responseContent);
        }


        [Fact]
        public async Task CalculateCartInsurance_EmptyCart_ReturnsBadRequest()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>();
            var endpoint = $"{TestConstants.ProductInsuranceCartRoute}";

            // Act
            var response = await SendRequestAsync(endpoint, cartItems);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(ApiConstants.ShoppingCartEmpty, responseContent);
        }

        [Fact]
        public async Task CalculateCartInsurance_InvalidProductId_ReturnsBadRequest()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = -1 },
                new InsuranceDto { ProductId = 999999 } // Assuming this is an invalid ID
            };
            var endpoint = $"{TestConstants.ProductInsuranceCartRoute}";

            // Act
            var response = await SendRequestAsync(endpoint, cartItems);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(ApiConstants.ShoppingCartProductAreNotEligible, responseContent);
        }

        [Fact]
        public async Task CalculateCartInsurance_ProductsWithZeroInsurance_ReturnsTotalInsuranceValue()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = 725435 }, // Assuming this returns zero insurance
                new InsuranceDto { ProductId = 735296 }  // Assuming this returns zero insurance
            };
            var endpoint = $"{TestConstants.ProductInsuranceCartRoute}";

            // Act
            var response = await SendRequestAsync(endpoint, cartItems);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(ApiConstants.ShoppingCartProductAreNotEligible, responseContent);
        }

        #endregion

        #region Helper Methods

        private async Task<HttpResponseMessage> SendRequestAsync<T>(string endpoint, T requestData)
        {
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return response;
        }

        private void AssertErrorResponse(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        private void AssertSuccessResponse<T>(
            HttpResponseMessage response,
            string responseContent,
            Func<T, bool> validationFunc)
        {
            // Ensure the response status code is successful
            response.EnsureSuccessStatusCode();

            // Deserialize the response content to the specified type
            var responseDto = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Assert that the response DTO is not null
            Assert.NotNull(responseDto);

            // Validate the response DTO using the provided validation function
            Assert.True(validationFunc(responseDto), "Validation failed for response DTO.");
        }

        #endregion
    }
}
