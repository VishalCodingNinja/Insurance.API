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
    public class InsuranceControllerTests
    {
        private readonly InsuranceController _controller;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IInsuranceService> _mockInsuranceService;

        public InsuranceControllerTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockInsuranceService = new Mock<IInsuranceService>();
            _controller = new InsuranceController(_mockMapper.Object, _mockInsuranceService.Object);
        }

        [Fact]
        public async Task CalculateInsuranceAsync_LaptopUnder500_EurosInsuranceSetTo500()
        {
            // Arrange
            var insuranceRequest = new InsuranceDto { ProductId = 572770 };
            var expectedInsurance = 500m;

            var productModel = new ProductModel
            {
                Id = 572770,
                SalesPrice = 475m,
                ProductType = new ProductTypeModel { Name = "Laptop", CanBeInsured = true }
            };

            var insuranceModel = new InsuranceModel
            {
                ProductId = 572770,
                ProductModel = productModel
            };

            _mockMapper.Setup(mapper => mapper.Map<InsuranceModel>(insuranceRequest))
                .Returns(insuranceModel);

            _mockInsuranceService.Setup(service => service.CalculateInsuranceAsync(It.IsAny<InsuranceModel>(), It.IsAny<bool>()))
                .ReturnsAsync(new InsuranceModel { InsuranceValue = expectedInsurance });

            _mockMapper.Setup(mapper => mapper.Map<InsuranceDto>(It.IsAny<InsuranceModel>()))
                .Returns(new InsuranceDto { InsuranceValue = expectedInsurance });

            // Act
            var result = await _controller.CalculateInsuranceAsync(insuranceRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var insuranceResponse = result.Value as InsuranceDto;
            Assert.Equal(expectedInsurance, insuranceResponse.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsuranceAsync_SmartphoneWithHighPrice_EurosInsuranceSetAccordingToPrice()
        {
            // Arrange
            var insuranceRequest = new InsuranceDto { ProductId = 827074 };
            var expectedInsurance = 500m;

            var productModel = new ProductModel
            {
                Id = 827074,
                SalesPrice = 699m,
                ProductType = new ProductTypeModel { Name = "Smartphone", CanBeInsured = true }
            };

            var insuranceModel = new InsuranceModel
            {
                ProductId = 827074,
                ProductModel = productModel
            };

            _mockMapper.Setup(mapper => mapper.Map<InsuranceModel>(insuranceRequest))
                .Returns(insuranceModel);

            _mockInsuranceService.Setup(service => service.CalculateInsuranceAsync(It.IsAny<InsuranceModel>(), It.IsAny<bool>()))
                .ReturnsAsync(new InsuranceModel { InsuranceValue = expectedInsurance });

            _mockMapper.Setup(mapper => mapper.Map<InsuranceDto>(It.IsAny<InsuranceModel>()))
                .Returns(new InsuranceDto { InsuranceValue = expectedInsurance });

            // Act
            var result = await _controller.CalculateInsuranceAsync(insuranceRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var insuranceResponse = result.Value as InsuranceDto;
            Assert.Equal(expectedInsurance, insuranceResponse.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsuranceAsync_InvalidProductId_ReturnsBadRequest()
        {
            // Arrange
            var insuranceRequest = new InsuranceDto { ProductId = -1 };

            // Act
            var result = await _controller.CalculateInsuranceAsync(insuranceRequest) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(string.Format($"{ApiConstants.InvalidProductData}", insuranceRequest.ProductId), result.Value);
        }

        [Fact]
        public async Task CalculateInsuranceAsync_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var insuranceRequest = new InsuranceDto { ProductId = 999999 };

            _mockMapper.Setup(mapper => mapper.Map<InsuranceModel>(insuranceRequest))
                .Returns(new InsuranceModel { ProductId = insuranceRequest.ProductId });

            _mockInsuranceService.Setup(service => service.CalculateInsuranceAsync(It.IsAny<InsuranceModel>(), It.IsAny<bool>()))
                .ReturnsAsync((InsuranceModel)null);

            // Act
            var result = await _controller.CalculateInsuranceAsync(insuranceRequest) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(string.Format($"{ApiConstants.ProductNotFound}", insuranceRequest.ProductId), result.Value);
        }

        [Fact]
        public async Task CalculateCartInsuranceAsync_EmptyCart_ReturnsBadRequest()
        {
            // Arrange
            var emptyCart = new List<InsuranceDto>();

            // Act
            var result = await _controller.CalculateCartInsuranceAsync(emptyCart) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(ApiConstants.ShoppingCartEmpty, result.Value);
        }

        [Fact]
        public async Task CalculateCartInsuranceAsync_CartWithMultipleItems_ReturnsTotalInsuranceValue()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = 572770 },
                new InsuranceDto { ProductId = 827074 }
            };
            var expectedTotalInsurance = 800m;

            var insuranceModels = new List<InsuranceModel>
            {
                new InsuranceModel
                {
                    ProductId = 572770,
                    ProductModel = new ProductModel
                    {
                        Id = 572770,
                        SalesPrice = 475m,
                        ProductType = new ProductTypeModel { Name = "Laptop", CanBeInsured = true }
                    }
                },
                new InsuranceModel
                {
                    ProductId = 827074,
                    ProductModel = new ProductModel
                    {
                        Id = 827074,
                        SalesPrice = 699m,
                        ProductType = new ProductTypeModel { Name = "Smartphone", CanBeInsured = true }
                    }
                }
            };

            _mockMapper.Setup(mapper => mapper.Map<List<InsuranceModel>>(cartItems))
                .Returns(insuranceModels);

            _mockInsuranceService.Setup(service => service.CalculateTotalInsuranceForCartAsync(It.IsAny<List<InsuranceModel>>()))
                .ReturnsAsync(expectedTotalInsurance);

            // Act
            var result = await _controller.CalculateCartInsuranceAsync(cartItems) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as TotalInsuranceResponse;
            Assert.Equal(expectedTotalInsurance, response.TotalInsurance);
        }

        [Fact]
        public async Task CalculateCartInsuranceAsync_CartContainsDigitalCamera_AddsExtra500Insurance()
        {
            // Arrange
            var cartItems = new List<InsuranceDto>
            {
                new InsuranceDto { ProductId = 715990 }
            };
            var expectedTotalInsurance = 695m;

            var insuranceModels = new List<InsuranceModel>
            {
                new InsuranceModel
                {
                    ProductId = 715990,
                    ProductModel = new ProductModel
                    {
                        Id = 715990,
                        SalesPrice = 195m,
                        ProductType = new ProductTypeModel { Name = "Digital Camera", CanBeInsured = true }
                    }
                }
            };

            _mockMapper.Setup(mapper => mapper.Map<List<InsuranceModel>>(cartItems))
                .Returns(insuranceModels);

            _mockInsuranceService.Setup(service => service.CalculateTotalInsuranceForCartAsync(It.IsAny<List<InsuranceModel>>()))
                .ReturnsAsync(expectedTotalInsurance);

            // Act
            var result = await _controller.CalculateCartInsuranceAsync(cartItems) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as TotalInsuranceResponse;
            Assert.Equal(expectedTotalInsurance, response.TotalInsurance);
        }
    }
}
