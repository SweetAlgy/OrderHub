namespace OrderHub.Shared.Entities
{
    /// <summary>
    /// Extends <see cref="IEntity{T}"/> with audit timestamps that record when the entity was
    /// created and last modified.
    /// </summary>
    /// <typeparam name="T">The type of the entity's primary key.</typeparam>
    public interface ITrackableEntity<out T> : IEntity<T>
    {
        /// <summary>Gets the UTC date and time when the entity was created.</summary>
        public DateTime CreatedAt { get; }

        /// <summary>Gets the UTC date and time when the entity was last updated.</summary>
        public DateTime UpdatedAt { get; }
    }
}