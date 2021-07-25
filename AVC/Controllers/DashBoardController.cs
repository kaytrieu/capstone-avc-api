using AutoMapper;
using AVC.Dtos.DashBoardDtos;
using AVC.Dtos.RoleDtos;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;


        public DashBoardController(IDashBoardService DashBoardervice)
        {
            _dashBoardService = DashBoardervice;
        }

        [HttpGet]
        public ActionResult<DashBoardDto> GetRole()
        {
            var dto = _dashBoardService.GetDashBoard();
            return Ok(dto);
        }

    }
}
