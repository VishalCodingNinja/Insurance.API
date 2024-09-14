using AutoMapper;
using Insurance.Business.Service.Services;
using Insurance.BusinessLayer.Models;
using Insurance.Data.Repositories;
using Insurance.Data.Repository.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Business.Service.UnitTests
{
    public class SurchargeServiceTests
    {
        private readonly SurchargeService _service;
        private readonly Mock<ISurchargeRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;

        public SurchargeServiceTests()
        {
            _mockRepository = new Mock<ISurchargeRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new SurchargeService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenNullSurchargeRates_ThrowsArgumentException()
        {
            // Arrange
            List<SurchargeModel> nullSurchargeRates = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadSurchargeRatesAsync(nullSurchargeRates));
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenEmptySurchargeRates_ThrowsArgumentException()
        {
            // Arrange
            List<SurchargeModel> emptySurchargeRates = new List<SurchargeModel>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadSurchargeRatesAsync(emptySurchargeRates));
        }

        [Fact]
        public async Task UploadSurchargeRatesAsync_GivenValidSurchargeRates_CallsRepositoryAddSurchargeRatesAsync()
        {
            // Arrange
            var surchargeRates = new List<SurchargeModel>
            {
                new SurchargeModel { ProductTypeId = 1, Surcharge = 100m },
                new SurchargeModel { ProductTypeId = 2, Surcharge = 200m }
            };

            _mockMapper.Setup(mapper => mapper.Map<List<SurchargeEntity>>(It.IsAny<List<SurchargeModel>>()))
                .Returns(new List<SurchargeEntity>
                {
                    new SurchargeEntity { ProductTypeId = 1, Surcharge = 100m },
                    new SurchargeEntity { ProductTypeId = 2, Surcharge = 200m }
                });

            // Act
            await _service.UploadSurchargeRatesAsync(surchargeRates);

            // Assert
            _mockRepository.Verify(repo => repo.AddSurchargeRatesAsync(It.IsAny<List<SurchargeEntity>>()), Times.Once);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenValidProductTypeId_ReturnsCorrectSurcharge()
        {
            // Arrange
            int validProductTypeId = 1;
            decimal expectedSurcharge = 150m;
            _mockRepository.Setup(repo => repo.GetSurchargeForProductTypeAsync(validProductTypeId))
                .ReturnsAsync(expectedSurcharge);

            // Act
            var actualSurcharge = await _service.GetSurchargeForProductTypeAsync(validProductTypeId);

            // Assert
            Assert.Equal(expectedSurcharge, actualSurcharge);
        }

        [Fact]
        public async Task GetSurchargeForProductTypeAsync_GivenProductTypeIdNotInDictionary_ReturnsZero()
        {
            // Arrange
            int nonExistingProductTypeId = 999;
            decimal expectedSurcharge = 0m;
            _mockRepository.Setup(repo => repo.GetSurchargeForProductTypeAsync(nonExistingProductTypeId))
                .ReturnsAsync(expectedSurcharge);

            // Act
            var actualSurcharge = await _service.GetSurchargeForProductTypeAsync(nonExistingProductTypeId);

            // Assert
            Assert.Equal(expectedSurcharge, actualSurcharge);
        }
    }
}
