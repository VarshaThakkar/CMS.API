using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Vaan.CMS.API.Authorization;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.IRepository;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.Repository
{
    public class UserServices : IUserServices
    {
        private readonly CMSDbContext _cMSDbContext;
        private readonly IConfiguration _configuration;
        private readonly IJwtUtils _jwtUtils;

        public UserServices(IConfiguration configuration, CMSDbContext cMSDbContext, IJwtUtils jwtUtils)
        {
            _configuration = configuration;
            _cMSDbContext = cMSDbContext;
            _jwtUtils = jwtUtils;
        }

        public void CreatePasswordHahs(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> IsUniqueUser(string email)
        {
            return await _cMSDbContext.CMSUsers.AnyAsync(u => u.Email == email);
        }
        public LoginResponse Login(UserEntity model)
        {
            var user = _cMSDbContext.CMSUsers.SingleOrDefault(x => x.Email == model.Email);
            string token = _jwtUtils.GenrateToken(user);
            return new LoginResponse(user, token);
        }
        public async Task<UserEntity> Register(UserEntity user)
        {
            await _cMSDbContext.CMSUsers.AddAsync(user);
            await _cMSDbContext.SaveChangesAsync();
            return user;
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<UserEntity> UpdateUser(UserEntity user, int id)
        {
            _cMSDbContext.CMSUsers.Update(user);
            await _cMSDbContext.SaveChangesAsync();
            return user;
        }
        public UserEntity GetById(int id)
        {
            return getUser(id);
        }
        public async Task DeleteUser(int id)
        {
            var userToDelete = getUser(id);
            if (userToDelete != null)
            {
                _cMSDbContext.CMSUsers.Remove(userToDelete);
                await _cMSDbContext.SaveChangesAsync();
            }
        }
        //helper method
        private UserEntity getUser(int id)
        {
            var user = _cMSDbContext.CMSUsers.Find(id);

            if (user == null) return null;
            return user;
        }
    }
}
