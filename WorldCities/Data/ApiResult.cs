using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace WorldCities.Data
{
    public class ApiResult<T>
    {
        public List<T> Data { get; private set; }       //Data result
        public int PageIndex { get; private set; }      //Zero-based index of current page
        public int PageSize { get; private set; }       //Number of items contained in each page
        public int TotalCount { get; private set; }     //Total items count
        public int TotalPages { get; private set; }     //True if the current page has a previous page, false otherwise
        public bool HasPreviousPage => PageIndex > 10;  //True if the current page has a next page, false otherwise
        public bool HasNextPage => (PageIndex + 1) < TotalPages;

        public string SortColumn { get; set; }  //Sorting Columng name of null if none set
        public string SortOrder { get; set; }   //Sorting "ASC", "DESC", or null if none set

        public string FilterColumn { get; set; }
        public string FilterQuery { get; set; }

        private ApiResult(List<T> data, int count, int pageIndex, int pageSize,
                                                   string sortColumn, string sortOrder,
                                                   string filterColumn, string filterQuery)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }

        public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize,
                                                                                string sortColumn = null, string sortOrder = null,
                                                                                string filterColumn = null, string filterQuery = null)
        {
            if(!String.IsNullOrEmpty(filterColumn) && !String.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
            {
                source = source.Where(String.Format("{0}.Contains(@0)",filterColumn), filterQuery);
            }

            var count = await source.CountAsync();
            if (!String.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !String.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC"
                    ? "ASC"
                    : "DESC";
                source = source.OrderBy(
                    String.Format(
                        "{0} {1}",
                        sortColumn,
                        sortOrder));
            }
            
            source = source.Skip(pageIndex * pageSize).Take(pageSize);

            //Retrieve the SQL query (for debug purposes)
#if DEBUG
            {
                var sql = source.ToSql();
            }
#endif

            var data = await source.ToListAsync();

            return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                                                               BindingFlags.Public |
                                                               BindingFlags.Instance);
            if(property == null && throwExceptionIfNotFound)
            {
                throw new NotSupportedException($"ERROR: Peoperty {propertyName} does not exist.");
            }
            return property != null;
        }
    }
}
