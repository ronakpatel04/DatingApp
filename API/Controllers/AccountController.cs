
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;

        public AccountController(DataContext context)
        {
            this.context = context;
        }

        [HttpPost("register")] // Post : api/Account/register


        public async Task<ActionResult<AppUser>> Register(RegisterDtos registerDtos)
        {
            if (await UserExist(registerDtos.Username)) return BadRequest("username is taken");

            using var hmac = new HMACSHA512();


            var user = new AppUser
            {

                UserName = registerDtos.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDtos.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;

        }


        [HttpPost("login")]

        public async Task<ActionResult<AppUser>> Login(LoginDTOs loginDtos)
        {

            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDtos.Username);

            if (user == null) return Unauthorized("Invalid Username");



            using var hmac = new HMACSHA512(user.PasswordSalt);
           
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.Password));
          
            if(!StructuralComparisons.StructuralEqualityComparer.Equals(ComputeHash, user.PasswordHash)){
                return Unauthorized("invalid PassComputeHashword");
            }

            // for (int i = 0; i < ComputeHash.Length; i++)
            // {
            //     if (ComputeHash[i] != user.PasswordHash[i])
            //     {
            //         return Unauthorized("Invalid Password");
            //     }
            // }
            return user;
        }

        private async Task<bool> UserExist(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}