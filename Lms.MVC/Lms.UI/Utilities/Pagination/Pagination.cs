using System.Collections.Generic;

namespace Lms.MVC.UI.Utilities.Pagination
{
    public abstract class PaginationResultBase
    {
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }
    }

    public class PaginationResult<T> : PaginationResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PaginationResult()
        {
            Results = new List<T>();
        }
    }
}