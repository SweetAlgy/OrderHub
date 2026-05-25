namespace OrderHub.Shared.Entities
{
    /// <summary>
    /// Base contract for all domain entities that carry a primary key.
    /// </summary>
    /// <typeparam name="T">The type of the entity's primary key.</typeparam>
    public interface IEntity<out T>
    {
        /// <summary>Gets the unique identifier of the entity.</summary>
        public T Id { get; }
    }
}