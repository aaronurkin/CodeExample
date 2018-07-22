using AaronUrkinCodeExample.BusinessLogicLayer.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedList<TResult>> ToPagedListAsync<T, TResult>(this IQueryable<T> source, Func<T, TResult> selector, int page, int entries)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (entries < 1)
            {
                entries = 5;
            }

            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * entries).Take(entries).ToArrayAsync();

            return new PagedList<TResult>(items.Select(selector), count, page, entries);
        }
    }
}
