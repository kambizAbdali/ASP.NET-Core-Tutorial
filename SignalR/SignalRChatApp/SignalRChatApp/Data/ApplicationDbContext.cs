using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SignalRChatApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<ChatRoom>()
                .HasMany(cr => cr.ChatMessages)
                .WithOne(cm => cm.ChatRoom)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}