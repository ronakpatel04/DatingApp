using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository

    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await  context.Users.Where(x=>x.UserName == username).ProjectTo<MemberDto>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
           
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await context.Users.Take(4).Skip(4).ProjectTo<MemberDto>(mapper.ConfigurationProvider).ToListAsync();
 
         }

        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
           return await context.Users.Include(p=>p.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
             return await context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(x=>x.UserName == username);
        }

        

        public  async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }

        public void Update(AppUser user)
        {
           context.Entry(user).State = EntityState.Modified;
        }

        Task<PagedList<MemberDto>> IUserRepository.GetMembersAsync()
        {
            throw new NotImplementedException();
        }
    }
}