using KASHOP.DAL.DTO.Response;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationResponse<T>> ToPaginationAsync<T>(this IQueryable<T> query, int page, int limit)
        {
            var totalCount = await query.CountAsync();
            var data = await query.Skip((page-1)*limit).Take(limit).ToListAsync();

            return new PaginationResponse<T>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }
    }
}
