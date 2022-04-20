using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // UserRegister -> User
            CreateMap<UserRegister, UserEntity>();

            // User -> LoginResponse
            CreateMap<UserEntity, LoginResponse>();

            // UpdateRequest -> User
            CreateMap<UpdateRequest, UserEntity>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));

            // ChangePasswordRequest -> User
            CreateMap<ChangePasswordRequest, UserEntity>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
        }
       
    }
}
