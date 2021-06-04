using AutoMapper;
using AVC.Constant;
using AVC.Dtos.ProfileDtos;
using AVC.GenericRepository;
using AVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using static AVC.Extensions.Extensions.Extensions;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;

        public ProfileController(IAccountRepository repository, IMapper mapper, IConfiguration config, ILogger<ProfileController> logger, IRoleRepository roleRepository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Change password for owner account
        /// </summary>
        /// <param name="profilePasswordUpdateDto"></param>
        /// <returns></returns>
        [Authorize]
        [AuthorizeRoles(Roles.Admin, Roles.Manager)]
        [HttpPut("password")]
        public IActionResult PutAccount(ProfilePasswordUpdateDto profilePasswordUpdateDto)
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            var id = int.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


            Account accountModel = _repository.Get(x => x.Id == id && x.IsAvailable == true, x => x.Role, x => x.Gender);

            if (accountModel == null)
            {
                return NotFound();
            }

            bool isAuthorized = accountModel.Password.Equals(BCrypt.Net.BCrypt.HashPassword(profilePasswordUpdateDto.OldPassword, accountModel.Salt));

            if (!isAuthorized)
            {
                return Unauthorized("Old Password is not correct");
            }

            //Mapper to Update new password and salt
            _mapper.Map(profilePasswordUpdateDto, accountModel);

            _repository.Update(accountModel);

            _repository.SaveChanges();


            return NoContent();
        }

        /// <summary>
        /// Get personal Profile Infomation
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult<ProfileReadDto> GetProfile()
        {
            var claims = (HttpContext.User.Identity as ClaimsIdentity).Claims;

            var id = int.Parse(claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);

            Account account = _repository.Get(x => x.Id == id && x.IsAvailable == true, x => x.Role, x => x.Gender);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProfileReadDto>(account));
        }
    }
}
