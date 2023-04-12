
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole,int,IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>, IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<AppUser>()
            .HasMany(ur=>ur.UserRoles)
            .WithOne(u=>u.User)
            .HasForeignKey(ur=>ur.UserId)
            .IsRequired();


             builder.Entity<AppRole>()
            .HasMany(ur=>ur.UserRoles)
            .WithOne(u=>u.Roles)
            .HasForeignKey(ur=>ur.RoleId)
            .IsRequired();


            builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceId, k.TargetUserId });

            builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceId)
            .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);



            builder.Entity<Message>()
            .HasOne(u => u.Receiver)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);



        }

    }
}