using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.IRepository
{
    public interface IUserServices
    {
        Task<UserEntity> Register(UserEntity user);
        LoginResponse Login(UserEntity user);
        Task<bool> IsUniqueUser(string email);
        void CreatePasswordHahs(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
       // string CreateToken(UserEntity user);
        Task<UserEntity> UpdateUser(UserEntity user,int id);
        Task DeleteUser(int id);
        UserEntity GetById(int id);
       
    }
}
