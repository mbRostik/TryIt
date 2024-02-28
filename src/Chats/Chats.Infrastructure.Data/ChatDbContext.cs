using Chats.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Infrastructure.Data
{
    public class ChatDbContext:DbContext
    {
        public ChatDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<CFile> CFiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageWithFile> MessageWithFiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet <ChatParticipant> ChatParticipants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
