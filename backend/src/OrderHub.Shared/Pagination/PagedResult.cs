namespace OrderHub.Shared.Pagination
{
    /// <summary>
    /// Wraps a single page of query results together with pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of the items on the current page.</typeparam>
    public record PagedResult<T>
    {
        /// <summary>Gets the items on the current page.</summary>
        public IEnumerable<T> Items { get; init; } = [];

        /// <summary>Gets the current 1-based page number.</summary>
        public int Page { get; init; }

        /// <summary>Gets the maximum number of items per page.</summary>
        public int PageSize { get; init; }

        /// <summary>Gets the total number of items across all pages.</summary>
        public int TotalCount { get; init; }

        /// <summary>Gets the total number of pages, derived from <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
        public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.PageSize);

        /// <summary>Gets a value indicating whether a next page exists.</summary>
        public bool HasNextPage => this.Page < this.TotalPages;

        /// <summary>Gets a value indicating whether a previous page exists.</summary>
        public bool HasPreviousPage => this.Page > 1;

        /// <summary>
        /// Projects the current page into a new <see cref="PagedResult{TResult}"/> by replacing
        /// the item collection while preserving all pagination metadata.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected items.</typeparam>
        /// <param name="items">The projected items to place on the new page.</param>
        /// <returns>A new <see cref="PagedResult{TResult}"/> with the same pagination metadata.</returns>
        public PagedResult<TResult> To<TResult>(IEnumerable<TResult> items)
            => new()
            {
                Items = items,
                Page = this.Page,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount
            };
    }
}