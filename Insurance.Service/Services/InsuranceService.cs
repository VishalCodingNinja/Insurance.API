using Insurance.Business.Domain.Constants;
using Insurance.Business.Domain.Services.Shared;
using Insurance.BusinessLayer.Models;
using Insurance.BusinessLayer.Services.Contracts;

namespace Insurance.Business.Services
{
    public class InsuranceService : BaseApiService, IInsuranceService
    {
        public InsuranceService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        #region Insurance Calculation Methods

        public async Task<InsuranceModel?> CalculateInsuranceAsync(InsuranceModel insuranceRequest, bool forCart = false)
        {
            if (insuranceRequest == null)
            {
                throw new ArgumentNullException(nameof(insuranceRequest), BusinessConstants.InsuranceRequestCanNotBeNull);
            }

            try
            {
                var product = await GetProductByIdAsync(insuranceRequest.ProductId);
                if (product == null) return null;

                insuranceRequest.ProductModel = product;

                var productType = await GetProductTypeByIdAsync(product.ProductTypeId);
                if (productType == null) return null;

                insuranceRequest.ProductModel.ProductType = productType;

                insuranceRequest.InsuranceValue = await CalculateInsuranceValue(insuranceRequest, forCart);

                return insuranceRequest;
            }
            catch (Exception ex)
            {
                LogError("Error occurred while calculating insurance", ex);
                throw;
            }
        }

        public async Task<decimal> CalculateTotalInsuranceForCartAsync(List<InsuranceModel> insuranceRequests)
        {
            if (insuranceRequests == null || !insuranceRequests.Any())
            {
                throw new ArgumentException("Insurance requests cannot be null or empty", nameof(insuranceRequests));
            }

            try
            {
                var insuranceTasks = insuranceRequests.Select(request => CalculateInsuranceAsync(request, true));
                var insuranceResults = await Task.WhenAll(insuranceTasks);

                var totalInsuranceValue = insuranceResults.Where(result => result != null)
                                                         .Sum(result => result.InsuranceValue);

                return ApplyCameraSurcharge(insuranceRequests, totalInsuranceValue);
            }
            catch (Exception ex)
            {
                LogError("Error occurred while calculating total insurance for cart", ex);
                throw;
            }
        }

        #endregion

        #region Insurance Value and Surcharge Calculation

        public async Task<decimal> CalculateInsuranceValue(InsuranceModel insuranceModel, bool forCart)
        {
            if (insuranceModel.ProductModel?.ProductType == null)
            {
                throw new ArgumentNullException(nameof(insuranceModel.ProductModel.ProductType), "Product type cannot be null");
            }

            if (!insuranceModel.ProductModel.ProductType.CanBeInsured) return 0;

            bool isLaptop = IsLaptop(insuranceModel.ProductModel.ProductType.Name);

            if (insuranceModel.ProductModel.SalesPrice < 500 && !isLaptop)
            {
                return 0m;
            }

            decimal baseInsuranceValue = DetermineBaseInsuranceValue(insuranceModel);
            decimal additionalInsuranceCost = AdditionalInsuranceCost(insuranceModel.ProductModel.ProductType.Name, forCart);
            decimal surcharge = (await GetSurchargeByProductTypeIdAsync(insuranceModel.ProductModel.ProductType.Id))?.Surcharge ?? 0.0m;

            return baseInsuranceValue + surcharge + additionalInsuranceCost;
        }

        public decimal DetermineBaseInsuranceValue(InsuranceModel insuranceModel)
        {
            return insuranceModel.ProductModel.SalesPrice switch
            {
                < BusinessConstants.MinimumInsuredValueThreshold when IsLaptopOrSmartPhone(insuranceModel.ProductModel.ProductType.Name) => BusinessConstants.BaseInsuranceValueForLowRange,
                < BusinessConstants.HighValueInsuredThreshold => BusinessConstants.BaseInsuranceValueForMidRange,
                _ => BusinessConstants.BaseInsuranceValueForHighRange
            };
        }

        public int AdditionalInsuranceCost(string productTypeName, bool forCart)
        {
            return productTypeName switch
            {
                var name when IsLaptopOrSmartPhone(name) => BusinessConstants.AdditionalInsuranceForElectronicsCategory,
                var name when IsDigitalCamera(name) && !forCart => BusinessConstants.AdditionalInsuranceForDigitalCameras,
                _ => 0
            };
        }

        public async Task<SurchargeModel?> GetSurchargeByProductTypeIdAsync(int productTypeId)
        {
            return await FetchFromApiAsync<SurchargeModel>($"{BusinessConstants.SurchargeRoute}/{productTypeId}", BusinessConstants.SurchargeApiClientName);
        }

        #endregion

        #region Camera Surcharge

        public decimal ApplyCameraSurcharge(List<InsuranceModel> insuranceRequests, decimal currentTotalInsuranceValue)
        {
            try
            {
                if (insuranceRequests == null || !insuranceRequests.Any())
                {
                    throw new ArgumentException("Insurance requests cannot be null or empty", nameof(insuranceRequests));
                }

                bool hasCamera = insuranceRequests.Any(request => IsDigitalCamera(request.ProductModel.ProductType.Name));
                return hasCamera ? currentTotalInsuranceValue + BusinessConstants.AdditionalInsuranceForDigitalCameras : currentTotalInsuranceValue;
            }
            catch (Exception ex)
            {
                LogError("Error occurred while applying camera surcharge", ex);
                return currentTotalInsuranceValue;
            }
        }

        #endregion

        #region Helper Methods

        private bool IsLaptopOrSmartPhone(string productTypeName)
        {
            return IsProductType(productTypeName, BusinessConstants.LaptopProductType, BusinessConstants.SmartphoneProductType);
        }

        private bool IsLaptop(string productTypeName)
        {
            return IsProductType(productTypeName, BusinessConstants.LaptopProductType);
        }

        private bool IsDigitalCamera(string productTypeName)
        {
            return IsProductType(productTypeName, BusinessConstants.DigitalCameraProductType);
        }

        private bool IsProductType(string productTypeName, params string[] validProductTypes)
        {
            if (string.IsNullOrEmpty(productTypeName))
                return false;

            productTypeName = productTypeName.Trim().Replace(" ", "").ToLower();

            return validProductTypes.Any(validType =>
                string.Equals(productTypeName, validType, StringComparison.OrdinalIgnoreCase));
        }

        private void LogError(string message, Exception ex)
        {
            //later we can put loging here
            Console.WriteLine($"{message}: {ex.Message}");
        }

        #endregion

        #region API Calls

        public async Task<ProductModel?> GetProductByIdAsync(int productId)
        {
            return await FetchFromApiAsync<ProductModel>($"{BusinessConstants.ProductsRoute}/{productId}", BusinessConstants.ProductApiClientName);
        }

        public async Task<ProductTypeModel?> GetProductTypeByIdAsync(int productTypeId)
        {
            return await FetchFromApiAsync<ProductTypeModel>($"{BusinessConstants.ProductTypesRoute}/{productTypeId}", BusinessConstants.ProductApiClientName);
        }

        #endregion
    }
}
