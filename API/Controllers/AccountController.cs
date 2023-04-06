
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpPost("register")] // Post : api/Account/register


        public async Task<ActionResult<UserDtos>> Register(RegisterDtos registerDtos)
        {
            if (await UserExist(registerDtos.Username)) return BadRequest("username is taken");

            using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDtos);



            user.UserName = registerDtos.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDtos.Password));
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new UserDtos
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender =  user.Gender
            };

        }


        [HttpPost("login")]

        public async Task<ActionResult<UserDtos>> Login(LoginDtos loginDtos)
        {

            var user = await context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDtos.Username);

            if (user == null) return Unauthorized("Invalid Username");



            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.Password));

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(ComputeHash, user.PasswordHash))
            {
                return Unauthorized("invalid Password");
            }

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
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender= user.Gender
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}