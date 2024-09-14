namespace Insurance.Business.Domain.Constants
{
    public static class BusinessConstants
    {
        public const string ProductApiClientName = "ProductApi";
        public const string SurchargeApiClientName = "SurchargeApi";
        public const string ProductsRoute = "products";
        public const string ProductTypesRoute = "product_types";
        public const string SurchargeRoute = "api/surcharge";

        public const string LaptopProductType = "laptops";
        public const string SmartphoneProductType = "Smartphones";
        public const string DigitalCameraProductType = "digitalcameras";

        public const int MinimumInsuredValueThreshold = 500;
        public const int HighValueInsuredThreshold = 2000;

        public const int BaseInsuranceValueForLowRange = 500;
        public const int BaseInsuranceValueForMidRange = 1000;
        public const int BaseInsuranceValueForHighRange = 2000;

        public const int AdditionalInsuranceForElectronicsCategory = 500;
        public const int AdditionalInsuranceForDigitalCameras = 500;

        public const string ShoppingCartEmpty = "The shopping cart is empty.";
        public const string ShoppingCartInsuranceEmpty = "The total insurance value is empty.";
        public const string InvalidProductData = "Invalid product data for product ID {0}.";
        public const string ProductNotFound = "Product with ID {0} not found.";
        public const string SurchargeRatesCannotBeEmptyMessage = "Surcharge rate cannot be empty.";
        public const string ProductTypeIdCannotBeEmptyMessage = "Product Type Id cannot be empty.";
        public const string InsuranceRequestCanNotBeNull = "Insurance request cannot be null";
        public const string ErrorOccuredWhileInsurace = "Error Occured while insurance calculation : {0}";
        public const string ErrorOccuredWhileTotCartInsurace = "Error Occured while insurance calculation for cart: {0}";

    }
}
