using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
