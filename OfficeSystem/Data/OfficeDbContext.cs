using Microsoft.EntityFrameworkCore;
using OfficeSystem.Models.Approval;
using OfficeSystem.Models.Users;

namespace OfficeSystem.Data
{
    public class OfficeDbContext : DbContext
    {
        public OfficeDbContext(DbContextOptions<OfficeDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.LoginId).IsUnique();
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "일반직원" },
                new Role { Id = 2, Name = "관리자" }
            );

            base.OnModelCreating(modelBuilder);
            // (기존 User/Role 설정이 있으면 그대로 두고, 아래를 추가)

            // enum → string 저장
            modelBuilder.Entity<Document>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ApprovalLine>()
                .Property(a => a.Status)
                .HasConversion<string>();

            // Document → Drafter (User) 관계
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Drafter)
                .WithMany()
                .HasForeignKey(d => d.DrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApprovalLine → Approver (User) 관계
            modelBuilder.Entity<ApprovalLine>()
                .HasOne(a => a.Approver)
                .WithMany()
                .HasForeignKey(a => a.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ApprovalLine> ApprovalLines { get; set; }
    }
}