using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NugetMoodReboot.Models;
using APIMoodReboot.Helpers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepositoryUsers repositoryUsers;
        private readonly HelperOAuthToken authToken;

        public AuthController(IRepositoryUsers repository, HelperOAuthToken authToken)
        {
            this.repositoryUsers = repository;
            this.authToken = authToken;
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            AppUser? user = await this.repositoryUsers.LoginUserAsync(loginModel.Username, loginModel.Password);

            if (user == null)
            {
                return Unauthorized("Usuario o password incorrectos");
            }

            if (user.Approved == false)
            {
                return Unauthorized("Usuario no está activado, por favor, revisa tu correo electrónico.");
            }

            SigningCredentials credentials = new(this.authToken.GetKeyToken(), SecurityAlgorithms.HmacSha512);

            string jsonAppUser = JsonConvert.SerializeObject(user);
            Claim[] informacion = new[]
            {
                new Claim("UserData", jsonAppUser)
            };

            JwtSecurityToken token = new(
                            claims: informacion,
                            issuer: this.authToken.Issuer,
                            audience: this.authToken.Audience,
                            signingCredentials: credentials,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            notBefore: DateTime.UtcNow);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost]
        public async Task<IActionResult> SignUp
            (SignUpModel signUpModel)
        {
            int userId = await this.repositoryUsers.RegisterUserAsync(signUpModel.Username, signUpModel.FirstName, signUpModel.LastName, signUpModel.Email, signUpModel.Password, signUpModel.Path);

            // Confirmation token
            string token = await this.repositoryUsers.CreateUserActionAsync(userId);

            return Ok(token);
        }
    }
}
