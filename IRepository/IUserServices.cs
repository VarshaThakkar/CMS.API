using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;

namespace Vaan.CMS.API.IRepository
{
    public interface IUserServices
    {
        Task<UserEntity> Register(UserEntity user);
        string Login(UserEntity user);
        Task<bool> IsUniqueUser(string email);
        void CreatePasswordHahs(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(UserEntity user);
        Task<UserEntity> UpdateUser(UserEntity user);
        Task DeleteUser(int id);
        UserEntity GetById(int id);
    }
}
