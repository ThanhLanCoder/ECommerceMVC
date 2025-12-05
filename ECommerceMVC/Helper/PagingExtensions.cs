using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerceMVC.Models.Common;

namespace ECommerceMVC.Helper
{
    public static class PagingExtensions
    {
        public static async Task<PagedResult<T>> ToPagedListAsync<T>(
           this IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) page = 1;

            var result = new PagedResult<T>
            {
                Page = page,
                PageSize = pageSize
            };

            // Tổng số bản ghi
            result.TotalItems = query.Count();
            result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

            // Lấy dữ liệu trang
            result.Items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return await Task.FromResult(result);
        }
    }
}
