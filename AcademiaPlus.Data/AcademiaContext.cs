using AcademiaPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaPlus.Data
{
    public class AcademiaContext : DbContext
    {
        public AcademiaContext(DbContextOptions<AcademiaContext> options) : base(options) { }
        public DbSet<Student> Student { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Registration> Registration { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Registration>()
                .HasKey(i => new { i.StudentId, i.SubjectId });

            modelBuilder.Entity<Registration>()
                .HasOne(i => i.Student)
                .WithMany(e => e.Registrations)
                .HasForeignKey(i => i.StudentId);

            modelBuilder.Entity<Registration>()
                .HasOne(i => i.Subject)
                .WithMany(m => m.Registrations)
                .HasForeignKey(i => i.SubjectId);
        }
    }
}
