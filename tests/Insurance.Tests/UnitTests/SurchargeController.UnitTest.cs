using AutoMapper;
using Insurance.Api.Consts;
using Insurance.Api.Controllers;
using Insurance.Api.Dtos;
using Insurance.BusinessLayer.Models;
using Insurance.BusinessLayer.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Api.UnitTests
{
    public class SurchargeControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISurchargeService> _surchargeServiceMock;
        private readonly SurchargeController _controller;

        public SurchargeControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _surchargeServiceMock = new Mock<ISurchargeService>();
            _controller = new SurchargeController(_mapperMock.Object, _surchargeServiceMock.Object);
        }

        #region Upload Surcharge Rates Tests

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenNullSurchargeRates_ReturnsBadRequest()
        {
            // Arrange
            List<ProductSurchargeDto> nullSurchargeRates = null;

            // Act
            var result = await _controller.UploadSurchargeRatesAsync(nullSurchargeRates);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiConstants.InvalidSurchargeData, badRequestResult.Value);
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenEmptySurchargeRates_ReturnsBadRequest()
        {
            // Arrange
            var emptySurchargeRates = new List<ProductSurchargeDto>();

            // Act
            var result = await _controller.UploadSurchargeRatesAsync(emptySurchargeRates);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiConstants.InvalidSurchargeData, badRequestResult.Value);
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenValidSurchargeRates_CallsServiceAndReturnsOk()
        {
            // Arrange
            var surchargeRates = new List<ProductSurchargeDto>
            {
                new ProductSurchargeDto { ProductTypeId = 1, Surcharge = 100m },
                new ProductSurchargeDto { ProductTypeId = 2, Surcharge = 200m }
            };

            var surchargeRateModels = new List<SurchargeModel>
            {
                new SurchargeModel { ProductTypeId = 1, Surcharge = 100m },
                new SurchargeModel { ProductTypeId = 2, Surcharge = 200m }
            };

            _mapperMock.Setup(m => m.Map<List<SurchargeModel>>(surchargeRates))
                .Returns(surchargeRateModels);

            // Act
            var result = await _controller.UploadSurchargeRatesAsync(surchargeRates);

            // Assert
            _surchargeServiceMock.Verify(s => s.UploadSurchargeRatesAsync(surchargeRateModels), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ApiConstants.SurchargeSuccessfulSaved, okResult.Value);
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_CallsMapperCorrectly()
        {
            // Arrange
            var surchargeRates = new List<ProductSurchargeDto>
            {
                new ProductSurchargeDto { ProductTypeId = 3, Surcharge = 300m }
            };

            var surchargeRateModels = new List<SurchargeModel>
            {
                new SurchargeModel { ProductTypeId = 3, Surcharge = 300m }
            };

            _mapperMock.Setup(m => m.Map<List<SurchargeModel>>(surchargeRates))
                .Returns(surchargeRateModels);

            // Act
            await _controller.UploadSurchargeRatesAsync(surchargeRates);

            // Assert
            _mapperMock.Verify(m => m.Map<List<SurchargeModel>>(surchargeRates), Times.Once);
        }

        #endregion

        #region Get Surcharge Tests

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenInvalidProductTypeId_ReturnsBadRequest()
        {
            // Arrange
            int invalidProductTypeId = -1;

            // Act
            var result = await _controller.GetSurchargeForProductTypeAsync(invalidProductTypeId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiConstants.ProductIdCanNotBeEmpty, badRequestResult.Value);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenValidProductTypeId_ReturnsOkWithSurcharge()
        {
            // Arrange
            int validProductTypeId = 1;
            decimal expectedSurcharge = 150m;

            _surchargeServiceMock.Setup(s => s.GetSurchargeForProductTypeAsync(validProductTypeId))
                .ReturnsAsync(expectedSurcharge);

            // Act
            var result = await _controller.GetSurchargeForProductTypeAsync(validProductTypeId) as OkObjectResult;

            // Assert
            var dto = Assert.IsType<ProductSurchargeDto>(result?.Value);
            Assert.NotNull(dto);
            Assert.Equal(validProductTypeId, dto.ProductTypeId);
            Assert.Equal(expectedSurcharge, dto.Surcharge);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenValidProductTypeIdAndNoSurcharge_ReturnsOkWithZeroSurcharge()
        {
            // Arrange
            int validProductTypeId = 2;
            decimal expectedSurcharge = 0m;

            _surchargeServiceMock.Setup(s => s.GetSurchargeForProductTypeAsync(validProductTypeId))
                .ReturnsAsync(expectedSurcharge);

            // Act
            var result = await _controller.GetSurchargeForProductTypeAsync(validProductTypeId) as OkObjectResult;

            // Assert
            var dto = Assert.IsType<ProductSurchargeDto>(result?.Value);
            Assert.NotNull(dto);
            Assert.Equal(validProductTypeId, dto.ProductTypeId);
            Assert.Equal(expectedSurcharge, dto.Surcharge);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenProductTypeIdInService_ReturnsExpectedSurcharge()
        {
            // Arrange
            int productTypeId = 3;
            decimal expectedSurcharge = 250m;

            _surchargeServiceMock.Setup(s => s.GetSurchargeForProductTypeAsync(productTypeId))
                .ReturnsAsync(expectedSurcharge);

            // Act
            var result = await _controller.GetSurchargeForProductTypeAsync(productTypeId) as OkObjectResult;

            // Assert
            var dto = Assert.IsType<ProductSurchargeDto>(result?.Value);
            Assert.NotNull(dto);
            Assert.Equal(productTypeId, dto.ProductTypeId);
            Assert.Equal(expectedSurcharge, dto.Surcharge);
        }

        #endregion
    }
}
