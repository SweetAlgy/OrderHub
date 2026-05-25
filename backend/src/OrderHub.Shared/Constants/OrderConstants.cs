namespace OrderHub.Shared.Constants
{
    /// <summary>
    /// Shared validation constants for the <c>Order</c> entity.
    /// </summary>
    public static class OrderConstants
    {
        /// <summary>Maximum allowed length of an order number in characters.</summary>
        public const int OrderNumberMaxLength = 64;

        /// <summary>Maximum allowed length of an order description in characters.</summary>
        public const int DescriptionMaxLength = 1024;
    }
}