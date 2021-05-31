using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.PagingDtos
{
    public class PagingResponseDto<TEntity> where TEntity : class
    {
        public PagingResponseDto(PagingDto<TEntity> dto)
        {
            Result = dto.Result;
            Count = dto.Count;
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
