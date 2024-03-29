﻿using Microsoft.EntityFrameworkCore;
using SecuredChatApp.Core.Entities;

namespace SecuredChatApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<FriendEntity> Friends { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MessageBoxEntity> MessageBoxes { get; set; }
    }
}
