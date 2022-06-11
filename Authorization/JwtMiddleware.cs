
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaan.CMS.API.IRepository;


namespace Vaan.CMS.API.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        public IConfiguration _configuration;
        private readonly IUserServices _userService;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IUserServices userService)
        {
            _next = next;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachAccountToContext(context, token);

            await _next(context);

            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //var userId = jwtUtils.ValidateToken(token);
            //if (userId != null)
            //{
            //    // attach user to context on successful jwt validation
            //    context.Items["User"] = userServices.GetById(userId.Value);
            //}

            //await _next(context);
        }
        private void attachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // attach account to context on successful jwt validation
                context.Items["UserEntity"] = _userService.GetById(Convert.ToInt32(userId));
            }
            catch
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }

    }
}
