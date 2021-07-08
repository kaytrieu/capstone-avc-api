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
        }

        public PagingResponseDto()
        {
        }

        public IEnumerable<TEntity> Result { get; set; }
        public int Count { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}
