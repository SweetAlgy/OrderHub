namespace OrderHub.Shared.Results
{
    /// <summary>
    /// Represents the outcome of an operation that can either succeed or fail.
    /// Follows the Railway-Oriented Programming pattern: a <see cref="Result"/> carries
    /// either a success state or a structured <see cref="Error"/>, but never both.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initialises a new result.
        /// </summary>
        /// <param name="isSuccess"><c>true</c> if the operation succeeded; otherwise <c>false</c>.</param>
        /// <param name="error">
        /// The error associated with a failure. Must be <see cref="Error.None"/> for a success result.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="isSuccess"/> is <c>true</c> but <paramref name="error"/> is not
        /// <see cref="Error.None"/>, or when <paramref name="isSuccess"/> is <c>false</c> but
        /// <paramref name="error"/> is <see cref="Error.None"/>.
        /// </exception>
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Success result cannot have an error.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failure result must have an error.");

            this.IsSuccess = isSuccess;
            this.Error = error;
        }

        /// <summary>Gets a value indicating whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets a value indicating whether the operation failed.</summary>
        public bool IsFailure => !this.IsSuccess;

        /// <summary>
        /// Gets the error that describes why the operation failed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessed on a success result.</exception>
        public Error Error => this.IsSuccess
            ? throw new InvalidOperationException("Success result does not have an error.")
            : field;

        /// <summary>Creates a non-generic success result.</summary>
        public static Result Success() => new(true, Error.None);

        /// <summary>Creates a non-generic failure result with the given error.</summary>
        /// <param name="error">The error describing the failure.</param>
        public static Result Failure(Error error) => new(false, error);

        /// <summary>Creates a generic success result carrying the specified value.</summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="value">The value to wrap.</param>
        public static Result<T> Success<T>(T value) => new(value, true, Error.None);

        /// <summary>Creates a generic failure result with the given error.</summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="error">The error describing the failure.</param>
        public static Result<T> Failure<T>(Error error) => new(default, false, error);
    }

    /// <summary>
    /// Extends <see cref="Result"/> with a typed <see cref="Value"/> that is only accessible
    /// when the operation succeeded.
    /// </summary>
    /// <typeparam name="T">The type of the value produced on success.</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Initialises a new typed result.
        /// </summary>
        /// <param name="value">The value, or <c>null</c> on failure.</param>
        /// <param name="isSuccess"><c>true</c> if the operation succeeded.</param>
        /// <param name="error">The error associated with a failure.</param>
        protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value produced by the successful operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessed on a failure result.</exception>
        public T Value => this.IsSuccess
            ? field!
            : throw new InvalidOperationException("Failure result has no value.");

        /// <summary>
        /// Implicitly wraps a value in a success <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public static implicit operator Result<T>(T value) => Result.Success(value);
    }
}