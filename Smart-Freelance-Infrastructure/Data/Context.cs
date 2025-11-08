using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Entities.Identity;

namespace Smart_Freelance_Infrastructure.Data
{
    public class Context : IdentityDbContext<ApplicationUser, Role, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        /* public Context() : base(new DbContextOptionsBuilder<Context>()
         .UseSqlServer("Data Source=DESKTOP-B4525D7;Initial Catalog=FreelanceProject;Integrated Security=True;TrustServerCertificate=True;")
         .Options)
         {
         }*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Client)
                .WithMany(u => u.ProjectsCreated)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CollaborationRoom>()
                .HasOne(cr => cr.Client)
                .WithMany(u => u.CollaborationRoomsAsClient)
                .HasForeignKey(cr => cr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CollaborationRoom>()
                .HasOne(cr => cr.AssignedFreelancer)
                .WithMany(u => u.CollaborationRoomsAsFreelancer)
                .HasForeignKey(cr => cr.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<Project> projects { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CollaborationRoom> CollaborationRooms { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProjectSubmission> ProjectSubmissions { get; set; }



    }
}
