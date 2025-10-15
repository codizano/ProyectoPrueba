using Microsoft.EntityFrameworkCore;
using ProyectoPrueba.Models;

namespace ProyectoPrueba
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Sheet> Sheets { get; set; } = null!;
        public DbSet<SheetObservation> SheetObservations { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasKey(s => s._id);

            modelBuilder.Entity<Sheet>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId);

            modelBuilder.Entity<SheetObservation>()
                .HasOne(so => so.Sheet)
                .WithMany()
                .HasForeignKey(so => so.SheetId);
        }
    }
}