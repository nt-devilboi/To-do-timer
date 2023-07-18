using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BBServer;
using BBServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using To_do_timer.Models;
using To_do_timer.Properties;
using Vostok.Logging.Abstractions;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace To_do_timer.Controllers;
[ApiController]
[Route("user")]
public class LoginController : Controller
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private ILog _log;

    public LoginController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILog log)
    {
        _log = log;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    [HttpGet]
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    public async Task<Result<LoginResponse>> Get()
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        
        _log.Info(userName);
        
        if (userName == null)
            return HttpContext.WithError<LoginResponse>(HttpStatusCode.Unauthorized, "Нету токена");

        return HttpContext.WithResult(HttpStatusCode.OK, new LoginResponse() { UserName = userName });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAuth user)
    {
        var appUser = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = user.Username,
            Email = user.Email
        };

        var result = await _userManager.CreateAsync(appUser, user.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        if (!await _roleManager.RoleExistsAsync(UserRole.Admin.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
        }

        if (await _roleManager.RoleExistsAsync(UserRole.Admin.ToString()))
        {
            await _userManager.AddToRoleAsync(appUser, UserRole.Admin.ToString());
        }
        

        return Ok(result);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<Result<TokenResponse>> Login([FromBody] UserLogin model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user ,model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(nameof(IdentityUser), user.SecurityStamp)
            };

            foreach (var userRole in userRoles)
            {
               userClaims.Add(new Claim(ClaimTypes.Role, userRole)) ;
            }
            var token = GetToken(userClaims);
            return HttpContext.WithResult(HttpStatusCode.OK, new TokenResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token), 
                expriration = token.ValidTo
            });
        }

        return HttpContext.WithError<TokenResponse>(HttpStatusCode.NotFound,"Такого юзера нету ");
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
        return BadRequest("Пользователь не найден");
    }

    private JwtSecurityToken GetToken(List<Claim> userClaims)
    {
        var authSingKey = AuthOptions.GetSymmetricSecurityKey();
        var token = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            expires: DateTime.Now.AddHours(3),
            claims: userClaims,
            signingCredentials: new SigningCredentials(authSingKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}