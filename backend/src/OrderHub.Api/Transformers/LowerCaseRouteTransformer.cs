using OrderHub.Shared.Extensions;

namespace OrderHub.Api.Transformers
{
    /// <summary>
    /// An <see cref="IOutboundParameterTransformer"/> that converts route tokens from
    /// PascalCase to snake_case, ensuring that generated URLs follow a consistent lowercase
    /// naming convention (e.g. <c>OrdersController</c> → <c>orders</c>).
    /// </summary>
    public sealed class LowerCaseRouteTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// Transforms an outbound route parameter value to its snake_case representation.
        /// </summary>
        /// <param name="value">The route parameter value to transform.</param>
        /// <returns>The snake_case string, or <c>null</c> if <paramref name="value"/> is <c>null</c>.</returns>
        public string? TransformOutbound(object? value) => value?.ToString()?.ToSnakeCase();
    }
}