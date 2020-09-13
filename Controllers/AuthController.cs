using System;
using System.Threading.Tasks;
using datingApp.api.Data;
using datingApp.api.Models;
using datingApp.api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        // [HttpGet]
        // public async Task<IActionResult> GetValues()
        // {
        //      var values = await _repo.Values.ToListAsync();

        //     return Ok(values); 
        // }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO ufrdto)
        {
            // String UserName = "sulaym123";
            // String Password = "password";
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //validate request
            ufrdto.UserName = ufrdto.UserName.ToLower();

            if (await _repo.UserExists(ufrdto.UserName))
                return BadRequest("UserName already exists");

            var userToCreate = new User
            {
                UserName = ufrdto.UserName
            };


            var createdUser = await _repo.Register(userToCreate,ufrdto.Password);
            return StatusCode(201);
        }

        //[AllowAnonymous]        
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserForLoginDTO ufldto)
        {
                // UserForLoginDTO ufldto = new UserForLoginDTO();
                // ufldto.UserName = "salman123";
                // ufldto.Password = "password";
                var userFromRepo = await _repo.login(ufldto.UserName.ToLower(),ufldto.Password);
                if(userFromRepo == null)
                return Unauthorized();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name,userFromRepo.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    // Audience = "management",
                    // Issuer = "management",
                    Subject = new ClaimsIdentity(claims),                    
                    Expires = DateTime.Now.AddDays(5),
                    SigningCredentials = creds
                };
                
                // tokenHandler.ValidateToken( token, new TokenValidationParameters
                // {
                //     ValidateIssuerSigningKey = true,
                //     IssuerSigningKey = key,
                //     ValidateIssuer = false,
                //     ValidateAudience = false,
                //     // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                //     ClockSkew = TimeSpan.Zero
                // }, out SecurityToken validatedToken);

                
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                
                return Ok(new {
                    token = tokenHandler.WriteToken(token)                    
                });
        }

        

    }
}