using ASP_DemoWebAPIJWTAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_DemoWebAPIJWTAuth.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Dummy validation
            if(model.UserName == "admin" && model.Password == "P@ssw0rd")
            {
                return Ok(GenerateToken(model.UserName, "Admin"));
            }
            else if (model.UserName == "user" && model.Password == "P@ssw0rd")
            {
                return Ok(GenerateToken(model.UserName, "User"));
            }

            return Unauthorized();

        }

        //private object GenerateToken(string userName, string role)
        //{
        //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, userName),
        //        new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
        //        new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
        //        new Claim(ClaimTypes.Role, role) // Role claim is added
        //    };

        //    var token = new JwtSecurityToken(claims: claims,
        //        expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:Expiry"])),
        //        signingCredentials: creds);
        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        //    return Ok(new { token = tokenString });
        //}

        private object GenerateToken(string userName, string role)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                new Claim(ClaimTypes.Role, role) // Role claim is added
            };

            // Add fine-grained permission
            if(role == "Admin")
            {
                claims.Add(new Claim("permission", "read"));
                claims.Add(new Claim("permission", "write"));
                claims.Add(new Claim("permission", "delete"));
            }
            else if(role == "User")
            {
                claims.Add(new Claim("permission", "read"));
            }

            var token = new JwtSecurityToken(claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:Expiry"])),
                signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = tokenString, role });
        }
    }
}
