using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.IRepository;

namespace Vaan.CMS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [RequiredScope(scopeRequiredByAPI)]

    public class ProtectedController : ControllerBase
    {
        const string scopeRequiredByAPI = "read-write";
        private readonly ILogger<ProtectedController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly CMSDbContext _cMSDbContext;
        private readonly IUserServices _userServices;
        public ProtectedController(ILogger<ProtectedController> logger, IHttpContextAccessor contextAccessor, CMSDbContext cMSDbContext, IUserServices userServices)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _cMSDbContext = cMSDbContext;
            _userServices = userServices;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetAllUsers()
        {
            var name = User.Identity.Name;
            var users = await _cMSDbContext.CMSUsers.ToListAsync();
            return Ok(users);
        }
        // GET: api/values
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<UserEntity>> Get(int id)
        {
            var userToRetrieve = await _cMSDbContext.CMSUsers.FindAsync(id);
            if (userToRetrieve == null)
            {
                return NotFound("Couldn't find such a user");
            }
            return Ok(userToRetrieve);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<UserEntity>> Patch(int id, [FromBody] UserEntity user)
        {
            var userToDUpdate = _cMSDbContext.CMSUsers.AsNoTracking().Where(e => e.Id == id);
            if (userToDUpdate == null)
            {
                return NotFound();
            }
            else if (user == null)
            {
                ModelState.AddModelError("custom", "User can not be empty");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
                await _userServices.UpdateUser(user, id);
            return Ok(user);
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var userToDelete = _userServices.DeleteUser(id);
            if (userToDelete == null)
            {
                return NotFound();
            }
            return Ok();

        }
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Customer customer)
        {
            var customerinfo = _cMSDbContext.Customers.FirstOrDefault(x => x.ObjectId == customer.ObjectId);
            if (customerinfo == null)
            {
                var customertoadd = _cMSDbContext.Customers.AddAsync(customer);
                _cMSDbContext.SaveChanges();
                return Ok(customertoadd);
            }
            return Ok(customer);
        }
    }
}
