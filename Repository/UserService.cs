using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.IRepository;
using Vaan.CMS.API.Models.Users;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.Helpers;
using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;
using Vaan.CMS.API.Authorization;


namespace Vaan.CMS.API.Repository
{
    public class UserService : IUserService
    {
        private CMSDbContext _context;
        private readonly IMapper _mapper;
        private IJwtUtils _jwtUtils;
        public UserService(CMSDbContext context, IMapper mapper, IJwtUtils jwtUtils)
        {
            _context = context;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
        }
        public void Register(UserRegister model)
        {
            if (_context.Users.Any(x => x.Email == model.Email))
            {  
                throw new AppException("Email " + model.Email + " is already taken");
            }

            // map model to user object
            var user = _mapper.Map<User>(model);

            //hash password
            user.Password = BCryptNet.HashPassword(model.Password);

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public LoginResponse Authenticate(LoginRequest model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == model.Email);

            if (user == null || !BCryptNet.Verify(model.Password,user.Password))
            {
                throw new AppException("Email or password is incorrect");

            }
            else
            {
                if (user.Status == false)
                {
                    throw new AppException("User status is deactivated");
                }
                var response = _mapper.Map<LoginResponse>(user);
                response.Token = _jwtUtils.GenrateToken(user);
                return response;
            }
        }
        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }
        public User GetById(int id)
        {
            return getUser(id);
        }

        // helper methods
        private User getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
        public void Update(int id, UpdateRequest model)
        {
            var user = getUser(id);
            if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            {
                throw new AppException("Email '" + model.Email + "' already taken");
            }
            if (!string.IsNullOrEmpty(model.Password))
            {   
                _mapper.Map(model, user);

                //hash password
                user.Password = BCryptNet.HashPassword(model.Password);

                _context.Users.Update(user);
                _context.SaveChanges();

            }
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public void ChangePassword(int id, ChangePasswordRequest model)
        {
            var user = getUser(id);
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = BCryptNet.HashPassword(model.Password);
                _mapper.Map(model, user);
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }
    }
}
