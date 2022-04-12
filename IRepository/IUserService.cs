using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.IRepository
{
  public  interface IUserService
    {
        void Register(UserRegister user);
        LoginResponse Authenticate(LoginRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Update(int id,UpdateRequest model);
        void ChangePassword(int id,ChangePasswordRequest model);
        void Delete(int id);
    }
}
