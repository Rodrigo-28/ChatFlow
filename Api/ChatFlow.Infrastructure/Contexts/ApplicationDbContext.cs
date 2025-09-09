using ChatFlow.Domain.Enums;
using ChatFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatFlow.Infrastructure.Contexts
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Profile> Profile { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
             new Role { Id = (int)UserRole.Admin, Name = "admin" },
             new Role { Id = (int)UserRole.User, Name = "user" }
         );

            //    // Configurar relaciones de User a Conversation
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1) // Un User puede ser User1 en muchas Conversations
                .WithMany(u => u.Conversations)
                .HasForeignKey(c => c.User1Id);
            //.OnDelete(DeleteBehavior.Cascade); // Elimina las conversaciones si se elimina el usuario

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2) // Un User puede ser User2 en muchas Conversations
                .WithMany()
                .HasForeignKey(c => c.User2Id);
            //.OnDelete(DeleteBehavior.Cascade); // Elimina las conversaciones si se elimina el usuario


            //    // Configurar relaciones de Conversation a Message
            //    modelBuilder.Entity<Message>()
            //        .HasOne(m => m.Conversation) // Un Message pertenece a una Conversation
            //        .WithMany(c => c.Messages)
            //        .HasForeignKey(m => m.ConversationId)
            //        .OnDelete(DeleteBehavior.Cascade); // Elimina los mensajes si se elimina la conversación

            //    // Configurar relaciones de User a Message
            //    modelBuilder.Entity<Message>()
            //        .HasOne(m => m.Sender) // Un Message es enviado por un User
            //        .WithMany(u => u.Messages);
            //}
            modelBuilder.Entity<Profile>()
                .HasKey(p => p.UserId);
            modelBuilder.Entity<Profile>()
             .HasOne(p => p.User)
             .WithOne(u => u.Profile)
             .HasForeignKey<Profile>(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();
        }
    }
}
