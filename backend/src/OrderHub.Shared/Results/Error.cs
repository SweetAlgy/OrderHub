using System.Net;

namespace OrderHub.Shared.Results
{
    /// <summary>
    /// Represents a structured error that accompanies a failed <see cref="Result"/>.
    /// Carries a machine-readable <see cref="Code"/>, a human-readable <see cref="Message"/>,
    /// and an HTTP <see cref="StatusCode"/> for use in API responses.
    /// </summary>
    /// <param name="Code">Machine-readable error code in the format <c>EntityName.ErrorType</c>.</param>
    /// <param name="Message">Human-readable description of the error.</param>
    /// <param name="StatusCode">HTTP status code to return to the client. Defaults to 400 Bad Request.</param>
    public record Error(string Code, string Message, int StatusCode = 400)
    {
        /// <summary>
        /// A sentinel value representing the absence of an error, used in success results.
        /// </summary>
        public static readonly Error None = new(String.Empty, String.Empty);

        /// <summary>
        /// Creates a 404 Not Found error for the given entity and identifier.
        /// </summary>
        /// <param name="entity">The entity type name (e.g. <c>"Order"</c>).</param>
        /// <param name="id">The identifier that was not found.</param>
        public static Error NotFound(string entity, object id) => new(
            $"{entity}.NotFound",
            $"{entity} with id '{id}' was not found.",
            (int)HttpStatusCode.NotFound
        );

        /// <summary>
        /// Creates a 409 Conflict error when an entity with the given field value already exists.
        /// </summary>
        /// <param name="entity">The entity type name.</param>
        /// <param name="field">The field whose value is duplicated (e.g. <c>"OrderNumber"</c>).</param>
        /// <param name="value">The duplicate value.</param>
        public static Error AlreadyExists(string entity, string field, object value) =>
            new(
                $"{entity}.AlreadyExists",
                $"{entity} with {field} '{value}' already exists.",
                (int)HttpStatusCode.Conflict
            );

        /// <summary>
        /// Creates a 400 Bad Request error indicating that the entity data is invalid.
        /// </summary>
        /// <param name="entity">The entity type name.</param>
        /// <param name="reason">A message explaining why the data is invalid.</param>
        public static Error Invalid(string entity, string reason) =>
            new($"{entity}.Invalid", reason);

        /// <summary>
        /// Creates a 500 Internal Server Error for unexpected failures.
        /// </summary>
        /// <param name="reason">A message describing the unexpected failure.</param>
        public static Error InternalServerError(string reason) =>
            new("InternalServerError", reason, (int)HttpStatusCode.InternalServerError);
    }
}