using AVC.Extensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.PagingDtos
{
    public class PagingResponseDto<TEntity> where TEntity : class
    {
        public PagingResponseDto(IEnumerable<TEntity> dto, int page, int limit)
        {
            Result = dto.AsQueryable().Paging(page, limit).AsEnumerable();
            Count = dto.Count();
            if ((double)dto.Count() / limit > page)
            {
                NextPageNumber = page + 1;
            }

            if (page > 1)
                PreviousPageNumber = page - 1;

            TotalPage = (int)Math.Ceiling((double)dto.Count() / limit);
        }

        public PagingResponseDto()
        {
        }

        public IEnumerable<TEntity> Result { get; set; }
        public int Count { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public int? NextPageNumber { get; set; }
        public int? PreviousPageNumber { get; set; }
        public int? TotalPage { get; set; }
    }
}
