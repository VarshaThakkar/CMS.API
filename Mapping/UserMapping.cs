using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;
using Vaan.CMS.API.Models.Users;

namespace Vaan.CMS.API.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<UserEntity, UserRegister>().ReverseMap();
        }
    }
}
