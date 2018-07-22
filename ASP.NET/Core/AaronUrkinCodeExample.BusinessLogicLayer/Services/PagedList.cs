using System;
using System.Collections.Generic;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services
{
    /// <summary>
    /// Represents a page with items of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type of each item</typeparam>
    public class PagedList<T> : List<T>
    {
        public int Page { get; private set; }

        public int TotalPages { get; private set; }

        public int TotalCount { get; private set; }

        public int PageEntries { get; private set; }

        public PagedList(IEnumerable<T> items, int totalCount, int page, int entries)
        {
            this.Page = page;
            this.TotalCount = totalCount;
            this.PageEntries = entries;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)entries);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (this.Page > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (this.Page < this.TotalPages);
            }
        }
    }
}
