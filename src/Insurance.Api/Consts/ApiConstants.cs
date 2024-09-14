namespace Insurance.Api.Consts
{
    public class ApiConstants
    {
        public const string ProductNotFound = "Requested product {0} can not be found in system";
        public const string InvalidProductData = "Invalid product insurance id {0} .";
        public const string ShoppingCartEmpty = "Shopping cart is empty or invalid.";
        public const string ShoppingCartInsuranceEmpty = "Shopping cart values has empty insurance value is empty or invalid.";
        public const string ShoppingCartProductAreNotEligible = "Shopping cart values are not eligible for insurance.";
        public const string InvalidSurchargeData = "Invalid surcharge rates data.";
        public const string SurchargeSuccessfulSaved = "Surcharge rates uploaded successfully.";
        public const string ErrorUploadingSurcharge = "Error uploading surcharge rates: {0}";
        public const string ProductIdCanNotBeEmpty = "Product id cannot be empty.";
        public const string ErrorRetriveingSurcharge = "Error retrieving surcharge rate: {0}";
        public const string ProductApiClient = "ProductApi";
        public const string SurchargeApiClient = "SurchargeApi";
    }
}
