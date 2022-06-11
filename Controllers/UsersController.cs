using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.IRepository;
using Vaan.CMS.API.Models.Google;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.Controllers
{
    // [Authorize(Policy = "AdminCheck")]

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
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetAllUsers()
        {
            var users = await _cMSDbContext.CMSUsers.ToListAsync();
            return Ok(users);
        }
        // [Authorize(Roles = Role.Admin)]        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserEntity>> GetUserById(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();
            var userToRetrieve = await _cMSDbContext.CMSUsers.FindAsync(id);
            if (userToRetrieve == null)
            {
                return NotFound("Couldn't find such a user");
            }
            return Ok(userToRetrieve);
        }


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
                return Ok(token);
            }
            return Unauthorized();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UserEntity>> UpdateUser(UserEntity user, int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            var userToDUpdate = _cMSDbContext.CMSUsers.FirstOrDefault(x => x.Id == id);
            if (userToDUpdate == null)
            {
                return NotFound();
            }
            else if (userToDUpdate.Id != id)
            {
                return BadRequest();
            }
            else if (user == null)
            {
                ModelState.AddModelError("custom", "User can not be empty");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
                await _userServices.UpdateUser(user,id);
            return Ok(user);

        }
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            //var userToDelete = _cMSDbContext.CMSUsers.FirstOrDefault(x => x.Id == id);
            //if (userToDelete == null)
            //{
            //    return NotFound();
            //}
            var userToDelete = _userServices.DeleteUser(id);
            if (userToDelete == null)
            {
                return NotFound();
            }
            return Ok();

        }

        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={0}";
        [HttpGet("userprofile")]
        public GoogleUserInfo ValidateGoogleToken(string providerToken)
        {
            HttpClient httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;

            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

            if (googleApiTokenInfo != null)
            {
                return new GoogleUserInfo
                {
                    Email = googleApiTokenInfo.email,
                    FirstName = googleApiTokenInfo.given_name,
                    LastName = googleApiTokenInfo.family_name,
                    // Locale = googleApiTokenInfo.locale,
                    //Name = googleApiTokenInfo.name,
                    //ProviderUserId = googleApiTokenInfo.sub
                };
            }
            return null;

        }
        [AllowAnonymous]
        [HttpPost("googlelogin")]
        public async Task<ActionResult<UserEntity>> GoogleUserRegister(GoogleRegister user)
        {
            var googleuserinfo = ValidateGoogleToken(user.AccessToken);
            if (googleuserinfo != null)
            {
                if (await _userServices.IsUniqueUser(user.Email))
                {
                    var userToRetrieve = await _cMSDbContext.CMSUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
                    var token = _userServices.Login(userToRetrieve);
                    return Ok(token);
                }
                else
                {
                    var usertologin = await _userServices.Register(new UserEntity { Email = user.Email, FirstName = googleuserinfo.FirstName, LastName = googleuserinfo.LastName });
                    var token = _userServices.Login(usertologin);
                    return Ok(token);
                }
            }
            return null;
            // return Unauthorized();
        }
    }
}
