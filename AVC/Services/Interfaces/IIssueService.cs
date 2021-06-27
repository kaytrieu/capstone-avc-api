using AVC.Dtos.IssueDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Models;
using Microsoft.AspNetCore.Http;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public  interface IIssueService
    {
        PagingResponseDto<IssueReadDto> GetIssueList(IssueQueryFilter filter);
        IssueReadDto GetIssueDetail(int id);
        IssueReadDto CreateNewIssue(IssueCreateDto dto, IFormFile image);
        void Patch(int id, JsonMergePatchDocument<IssueCreateDto> dto);
        IEnumerable<TypeReadDto> GetTypeList();
    }
}
