using System;
using System.Linq;

namespace Lms.MVC.UI.Utilities.Pagination
{
    public static class LinqExtensions
    {
        public static PaginationResult<T> GetPagination<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new PaginationResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}