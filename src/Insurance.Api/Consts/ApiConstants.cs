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

        //Exceptions
        public static class ExceptionMessages
        {
            public const string DefaultErrorMessage = "An unexpected error occurred. Please try again later.";
            public const string InvalidOperation = "Invalid operation.";
            public const string NullParameter = "A required parameter was null.";
            public const string AccessDenied = "Access denied.";
            public const string NotSupported = "The requested operation is not supported.";
            public const string ResourceNotFound = "The specified resource was not found.";
            public const string Timeout = "The request timed out. Please try again.";
            public const string Conflict = "A conflict occurred with the current state of the resource.";
            public const string UnauthorizedAction = "You are not authorized to perform this action.";
            public const string BadRequest = "The request is invalid. Please check your inputs.";
            public const string NotImplemented = "This functionality is not yet implemented.";
            public const string DatabaseError = "A database error occurred.";
        }
    }
}
