using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikeRepository
    {


        public LikesRepository(DataContext context)
        {
            Context = context;
        }

        public DataContext Context { get; }

        public async Task<UserLike> GetUserLike(int sourceId, int targetUserId)
        {

            return await Context.Likes.FindAsync(sourceId, targetUserId);

        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {

            var users = Context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = Context.Likes.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceId == likeParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }
             if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers =  users.Select(user=>new LikeDto{
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain).Url,
                City =user.City,
                Id = user.Id
            });
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber,likeParams.PageSize);
        }

        public  async Task<AppUser> GetUserWithLikes(int userId)
        {
           return await Context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=>x.Id == userId);
        }
    }
}