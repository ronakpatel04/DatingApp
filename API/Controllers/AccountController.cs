
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;

        }

        [HttpPost("register")] // Post : api/Account/register


        public async Task<ActionResult<UserDtos>> Register(RegisterDtos registerDtos)
        {
            if (await UserExist(registerDtos.Username)) return BadRequest("username is taken");

            using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDtos);



            user.UserName = registerDtos.Username.ToLower();

            var result = await userManager.CreateAsync(user, registerDtos.Password);


            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var rolesResult = await userManager.AddToRoleAsync(user, "Member");

            if (!rolesResult.Succeeded) return BadRequest(result.Errors);

            return new UserDtos
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }


        [HttpPost("login")]

        public async Task<ActionResult<UserDtos>> Login(LoginDtos loginDtos)
        {

            var user = await userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDtos.Username);

            if (user == null) return Unauthorized("Invalid Username");

            var result = await userManager.CheckPasswordAsync(user, loginDtos.Password);


            if (!result) return Unauthorized("Invalid Password");

            // for (int i = 0; i < ComputeHash.Length; i++)
            // {
            //     if (ComputeHash[i] != user.PasswordHash[i])
            //     {
            //         return Unauthorized("Invalid Password");
            //     }
            // }
            return new UserDtos
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await userManager.Users.AnyAsync(x => x.UserName == username);
        }
    }
}