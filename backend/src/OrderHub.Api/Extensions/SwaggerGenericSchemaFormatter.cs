namespace OrderHub.Api.Extensions
{
    /// <summary>
    /// Provides a helper for formatting .NET generic type names into a human-readable
    /// <c>TypeName&lt;Arg1, Arg2&gt;</c> form suitable for use as Swagger schema IDs.
    /// </summary>
    public static class SwaggerGenericSchemaFormatter
    {
        /// <summary>
        /// Recursively formats a <see cref="Type"/> into a human-readable generic type name string.
        /// Non-generic types are returned as their simple <see cref="Type.Name"/>.
        /// For generic types the arity suffix (e.g. <c>`1</c>) is stripped and the type arguments
        /// are formatted recursively.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <returns>
        /// A string such as <c>"PagedResult&lt;OrderDto&gt;"</c> for generic types,
        /// or simply <c>"OrderDto"</c> for non-generic types.
        /// </returns>
        public static string Format(Type type)
        {
            if (!type.IsGenericType) return type.Name;

            var genericTypeName = type.GetGenericTypeDefinition().Name;
            var genericTypeNameWithoutArity = genericTypeName.Contains('`')
                ? genericTypeName[..genericTypeName.IndexOf('`')]
                : genericTypeName;

            var genericTypeArguments = String.Join(
                ", ",
                type.GetGenericArguments().Select(SwaggerGenericSchemaFormatter.Format)
            );

            return $"{genericTypeNameWithoutArity}<{genericTypeArguments}>";
        }
    }
}