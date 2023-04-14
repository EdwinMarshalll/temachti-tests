using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Temachti.Api.DTOs;

namespace Temachti.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController:ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<DTOAuthenticationRequest>> Register(DTOUserCredentials userCredentials)
    {
        var user = new IdentityUser
        {
            UserName = userCredentials.Email,
            Email = userCredentials.Email
        };

        var result = await userManager.CreateAsync(user, userCredentials.Password);

        if(result.Succeeded)
        {
            return CreateToken(userCredentials);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<DTOAuthenticationRequest>> Login(DTOUserCredentials userCredentials)
    {
        var result = await signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

        if(result.Succeeded)
        {
            return CreateToken(userCredentials);
        }
        else
        {
            return BadRequest("Login incorrecto");
        }
    }

    private DTOAuthenticationRequest CreateToken(DTOUserCredentials userCredentials)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", userCredentials.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddDays(6);

        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: cred);

        return new DTOAuthenticationRequest()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiration = expiration
        };
    }
}