using AutoMapper;
using AVC.Dtos.IssueDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Models;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Morcatko.AspNetCore.JsonMergePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        // GET: api/Cars
        [HttpGet]
        public ActionResult<PagingResponseDto<IssueReadDto>> GetIssueList([FromQuery] IssueQueryFilter filter)
        {
            var response = _issueService.GetIssueList(filter);

            return Ok(response);
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public ActionResult<IssueReadDto> GetIssue(int id)
        {
            var respone = _issueService.GetIssueDetail(id);

            return Ok(respone);

        }

        [AllowAnonymous]
        [HttpPost()]
        public ActionResult<IssueReadDto> PostStaffAccount([FromForm] IssueCreateDto dto, IFormFile image)
        {
            IssueReadDto readDto = _issueService.CreateNewIssue(dto,image);

            //return Ok();
            return CreatedAtAction("GetIssue", new { id = readDto.Id }, readDto);
        }

        [HttpPatch("{id}")]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public IActionResult PatchAccount(int id, [FromBody] JsonMergePatchDocument<IssueCreateDto> dto)
        {
            _issueService.Patch(id, dto);

            return NoContent();
        }

    }
}
