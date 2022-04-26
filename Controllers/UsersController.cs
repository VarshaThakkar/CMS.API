using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.IRepository;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly CMSDbContext _cMSDbContext;
        private readonly IMapper _mapper;

        public UsersController(IUserServices userServices, CMSDbContext cMSDbContext, IMapper mapper)
        {
            _userServices = userServices;
            _cMSDbContext = cMSDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetAllUsers()
        {
            var users = await _cMSDbContext.CMSUsers.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserEntity>> GetUserById(int id)
        {
            var userToRetrieve = await _cMSDbContext.CMSUsers.FindAsync(id);
            if (userToRetrieve == null)
            {
                return NotFound("Couldn't find such a user");
            }
            return Ok(userToRetrieve);
        }

        //[AllowAnonymous]
        //[HttpGet("GetByEmail/{email}")]
        //public async Task<ActionResult<UserEntity>> GetByEmail(string email)
        //{
        //    var userToRetrieve = await _cMSDbContext.CMSUsers.FindAsync(email);
        //    if (userToRetrieve == null)
        //    {
        //        return NotFound("Couldn't find such a user");
        //    }
        //    return Ok(userToRetrieve);
        //}

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserEntity>> RegisterUser([FromBody] UserRegister user)
        {
            if (await _userServices.IsUniqueUser(user.Email))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            if (user == null)
            {
                ModelState.AddModelError("Custom", "User can not be empty");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
            {
                var userToSave = _mapper.Map<UserEntity>(user);
                if (user.Password != null)
                {
                    _userServices.CreatePasswordHahs(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    userToSave.HasswordHash = passwordHash;
                    userToSave.PasswordSalt = passwordSalt;
                }
                await _userServices.Register(userToSave);
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest user)
        {
            var userToRetrieve = await _cMSDbContext.CMSUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (userToRetrieve == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (user.Password != null)
            {
                if (!_userServices.VerifyPasswordHash(user.Password, userToRetrieve.HasswordHash, userToRetrieve.PasswordSalt))
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            if (userToRetrieve != null)
            {
                var token = _userServices.Login(userToRetrieve);
                // _userServices.Login(userToRetrieve);
                return Ok(token);
            }
            return Unauthorized();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UserEntity>> UpdateUser(UserEntity user, int id)
        {
            var userToDUpdate = _cMSDbContext.CMSUsers.FirstOrDefault(x => x.Id == id);
            if (userToDUpdate == null)
            {
                return NotFound();
            }
            else if (userToDUpdate.Id != id)
            {
                return BadRequest();
            }
            //else if (await _userServices.IsUniqueUser(user.Email))
            //{
            //    return StatusCode(StatusCodes.Status400BadRequest);
            //}
            else if (user == null)
            {
                ModelState.AddModelError("custom", "User can not be empty");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
                await _userServices.UpdateUser(user);
            return Ok(user);

        }
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var userToDelete = _cMSDbContext.CMSUsers.FirstOrDefault(x => x.Id == id);
            if (userToDelete == null)
            {
                return NotFound();
            }
            _cMSDbContext.CMSUsers.Remove(userToDelete);
            _cMSDbContext.SaveChanges();
            return NoContent();

        }

        [AllowAnonymous]
        [HttpPost("googlelogin")]
        public async Task<ActionResult<UserEntity>> GoogleUserRegister(UserRegister user)
        {
            if (await _userServices.IsUniqueUser(user.Email))
            {
                var userToRetrieve = await _cMSDbContext.CMSUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
                var token = _userServices.Login(userToRetrieve);
                return Ok(token);
            }
            else
            {
                var usertologin = await _userServices.Register(new UserEntity { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName });
                var token = _userServices.Login(usertologin);
                return Ok(token);
            }
            // return Unauthorized();
        }
    }
}
