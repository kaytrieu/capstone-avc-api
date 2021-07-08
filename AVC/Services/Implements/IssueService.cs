using AutoMapper;
using AVC.Constant;
using AVC.Dtos.IssueDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Extensions;
using AVC.Extensions.Extensions;
using AVC.Models;
using AVC.Repositories.Interface;
using AVC.Service;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class IssueService : BaseService, IIssueService
    {
        public IssueService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor) : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
        }

        public IssueReadDto CreateNewIssue(IssueCreateDto dto, IFormFile image)
        {
            Issue issueModel = _mapper.Map<Issue>(dto);

            _unit.IssueRepository.Add(issueModel);

            try
            {
                _unit.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

            issueModel.Image = UploadImage(image, issueModel.Id);

            _unit.SaveChanges();

            issueModel = _unit.IssueRepository.Get(x => x.Id == issueModel.Id, x => x.Car, x=> x.Type);

            IssueReadDto issueReadDto = _mapper.Map<IssueReadDto>(issueModel);

            return issueReadDto;
        }

        public IssueReadDto GetIssueDetail(int id)
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var issueFromRepo = _unit.IssueRepository.Get(issue => issue.Id == id && (bool)issue.IsAvailable, includer: x => x.Include(issue => issue.Car).ThenInclude(car => car.AssignedCar)
                                .Include(issue => issue.Type));

            if (role.Equals(Roles.Manager))
            {
                if (issueFromRepo.Car.ManagedBy != actorId)
                {
                    throw new PermissionDeniedException("Permission denied");
                }
            }

            if (role.Equals(Roles.Staff))
            {
                var assigned = issueFromRepo.Car.AssignedCar.FirstOrDefault(x => (bool)x.IsAvailable);
                if (assigned == null || assigned.AccountId != actorId)
                {
                    throw new PermissionDeniedException("Permission denied");
                }
            }

            var issueDto = _mapper.Map<IssueReadDto>(issueFromRepo);

            return issueDto;
        }

        public PagingResponseDto<IssueReadDto> GetIssueList(IssueQueryFilter filter)
        {
            var searchValue = filter.SearchValue;
            var page = filter.Page;
            var limit = filter.Limit;

            searchValue = searchValue.IsNullOrEmpty() ? "" : searchValue.Trim();

            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;

            var role = claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

            PagingDto<Issue> dto = _unit.IssueRepository.GetAllWithOrderedDecs(page, limit, issue => (bool)issue.IsAvailable && issue.Description.Contains(searchValue), x => x.CreatedAt,
                includer: x => x.Include(issue => issue.Car).ThenInclude(car => car.AssignedCar)
                                .Include(issue => issue.Type));
            var actorId = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            if (role.Equals(Roles.Manager))
            {
                dto.Result = dto.Result.Where(x => x.Car.ManagedBy == actorId);
            }

            if (role.Equals(Roles.Staff))
            {
                dto.Result = dto.Result.Where(x => x.Car.AssignedCar.FirstOrDefault(x => (bool)x.IsAvailable).AccountId == actorId);
            }

            if (filter.TypeId != null)
            {
                dto.Result = dto.Result.Where(x => x.TypeId == filter.TypeId);
            }
            if (filter.CarId != null)
            {
                dto.Result = dto.Result.Where(x => x.CarId == filter.CarId);
            }

            var issues = _mapper.Map<IEnumerable<IssueReadDto>>(dto.Result);

            var response = new PagingResponseDto<IssueReadDto> (issues, page, limit);

            if (limit > 0)
            {
                if ((double)dto.Count / limit > page)
                {
                    response.NextPage = _urlHelper.Link(null, new { page = page + 1, limit, searchValue });
                }

                if (page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = page - 1, limit, searchValue });
            }
            return response;
        }

        public IEnumerable<TypeReadDto> GetTypeList()
        {
            var typelist = _unit.IssueTypeRepository.GetAll(x => x.IsAvailable == true);
            var response = _mapper.Map<IEnumerable<TypeReadDto>>(typelist);
            return response;
        }

        public void Patch(int id, JsonMergePatchDocument<IssueCreateDto> dto)
        {
            Issue issueFromRepo = _unit.IssueRepository.Get(x => x.Id == id);

            if (issueFromRepo == null)
            {
                throw new NotFoundException("Issue not found");
            }

            IssueCreateDto issueToPath = _mapper.Map<IssueCreateDto>(issueFromRepo);

            dto.ApplyTo(issueToPath);

            _mapper.Map(issueToPath, issueFromRepo);

            //Temp is not doing nothing
            _unit.IssueRepository.Update(issueFromRepo);

            _unit.SaveChanges();
        }

        private string UploadImage(IFormFile image, int id)
        {
             return FirebaseService.UploadFileToFirebaseStorage(image, ("Issue" + id).GetHashString(), "IssueImage", _config).Result;
        }
    }
}
