using AVC.Dtos.IssueDtos;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueTypesController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueTypesController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        // GET: api/Types
        /// <summary>
        /// Get all Type of System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<TypeReadDto>> GetTypeList()
        {
            var dto = _issueService.GetTypeList();
            return Ok(new {Result = dto });
        }

    }
}
