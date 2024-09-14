using AutoMapper;
using Insurance.Business.Domain.Constants;
using Insurance.BusinessLayer.Models;
using Insurance.BusinessLayer.Services.Contracts;
using Insurance.Data.Repositories;
using Insurance.Data.Repository.Entities;

namespace Insurance.Business.Service.Services
{
    /// <summary>
    /// Service class for managing surcharge-related operations.
    /// </summary>
    public class SurchargeService : ISurchargeService
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor to initialize the surcharge service with the repository and mapper.
        /// </summary>
        /// <param name="surchargeRepository">The surcharge repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public SurchargeService(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Uploads surcharge rates by mapping them to entities and passing them to the repository.
        /// </summary>
        /// <param name="surchargeRates">List of surcharge rate models containing ProductTypeId and Surcharge.</param>
        /// <exception cref="ArgumentException">Thrown when surchargeRates is null or empty.</exception>
        public async Task UploadSurchargeRatesAsync(List<SurchargeModel> surchargeRates)
        {
            if (surchargeRates == null || surchargeRates.Count == 0)
            {
                throw new ArgumentException(BusinessConstants.SurchargeRatesCannotBeEmptyMessage);
            }

            // Map models to entities
            var surchargeRateEntities = _mapper.Map<List<SurchargeEntity>>(surchargeRates);

            // Pass the mapped entities to the repository for handling
            await _surchargeRepository.AddSurchargeRatesAsync(surchargeRateEntities);
        }

        /// <summary>
        /// Retrieves the surcharge for a specific product type by its ID.
        /// </summary>
        /// <param name="productTypeId">The ID of the product type.</param>
        /// <returns>The surcharge for the specified product type.</returns>
        /// <exception cref="ArgumentException">Thrown when productTypeId is less than or equal to zero.</exception>
        public async Task<decimal> GetSurchargeForProductTypeAsync(int productTypeId)
        {
            if (productTypeId <= 0)
            {
                throw new ArgumentException(BusinessConstants.ProductTypeIdCannotBeEmptyMessage);
            }

            return await _surchargeRepository.GetSurchargeForProductTypeAsync(productTypeId);
        }
    }
}
