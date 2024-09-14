using Insurance.Api.Consts;
using Insurance.Api.Dtos;
using Insurance.Tests.Constants;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Api.IntegrationTests
{
    public class SurchargeControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly HttpClient _client;

        public SurchargeControllerTests(ControllerTestFixture fixture)
        {
            _client = fixture.Client;
        }

        #region Data Seeding

        private async Task SeedSurchargeRatesAsync()
        {
            var surchargeRates = new List<ProductSurchargeDto>
            {
                new ProductSurchargeDto { ProductTypeId = 1, Surcharge = 150m },
                new ProductSurchargeDto { ProductTypeId = 2, Surcharge = 200m }
            };

            var content = new StringContent(JsonSerializer.Serialize(surchargeRates), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(TestConstants.UploadSurchargeRoute, content);
            response.EnsureSuccessStatusCode(); // Ensure we get a successful response
        }

        #endregion

        #region GET Surcharge Tests

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_ValidProductTypeId_ReturnsSurcharge()
        {
            // Seed data
            //  await SeedSurchargeRatesAsync();

            // Arrange
            var productTypeId = 1;
            var expectedSurcharge = 150m;
            var endpoint = $"{TestConstants.GetSurchargeRoute}/{productTypeId}";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ProductSurchargeDto>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.NotNull(dto);
            Assert.Equal(productTypeId, dto.ProductTypeId);
            Assert.Equal(expectedSurcharge, dto.Surcharge);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_InvalidProductTypeId_ReturnsBadRequest()
        {
            // Arrange
            var productTypeId = -1; // Invalid ID
            var endpoint = $"{TestConstants.GetSurchargeRoute}/{productTypeId}";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains(ApiConstants.ProductIdCanNotBeEmpty, responseContent); // Adjust as per your error message
        }

        #endregion

        #region POST Surcharge Tests

        [Fact]
        public async Task UploadSurchargeRatesAsync_NullOrEmptySurchargeRates_ReturnsBadRequest()
        {
            // Arrange
            var endpoint = TestConstants.UploadSurchargeRoute;
            var nullContent = new StringContent("null", Encoding.UTF8, "application/json");
            var emptyContent = new StringContent("[]", Encoding.UTF8, "application/json");

            // Act
            var nullResponse = await _client.PostAsync(endpoint, nullContent);
            var emptyResponse = await _client.PostAsync(endpoint, emptyContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, nullResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, emptyResponse.StatusCode);
        }

        //[Fact]
        //public async Task UploadSurchargeRatesAsync_ValidSurchargeRates_ReturnsOk()
        //{
        //    // Arrange
        //    var surchargeRates = new List<ProductSurchargeDto>
        //    {
        //        new ProductSurchargeDto { ProductTypeId = 1, Surcharge = 150m },
        //        new ProductSurchargeDto { ProductTypeId = 2, Surcharge = 200m }
        //    };

        //    var content = new StringContent(JsonSerializer.Serialize(surchargeRates), Encoding.UTF8, "application/json");
        //    var endpoint = TestConstants.UploadSurchargeRoute;

        //    // Act
        //    var response = await _client.PostAsync(endpoint, content);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    Assert.Contains(ApiConstants.SurchargeSuccessfulSaved, responseContent); // Adjust as per your success message
        //}

        #endregion
    }
}
