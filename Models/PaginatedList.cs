
using Microsoft.EntityFrameworkCore;

namespace Prog7311_Part2.Models
{
    // Repository Pattern Implementation:
    // This PaginatedList class is part of the Repository pattern implementation
    // by providing data filtering, counting, and pagination functionality.
    // It works with the AppDbContext to handle data access for collections
    // in a structured way, supporting the report's mention of the Repository pattern.
    
    /// <summary>
    /// Generic pagination class for creating paginated lists from IQueryable sources
    
    /// Adapted from: Microsoft (2024) 'Sorting, filtering, and paging with EF Core', Microsoft Docs.4 October 2024.[online]
    /// Available at: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page
    /// (Accessed: 14 May 2025).
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;

            this.AddRange(items);
        }

        // Navigation helper properties for UI pagination controls
        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
} 