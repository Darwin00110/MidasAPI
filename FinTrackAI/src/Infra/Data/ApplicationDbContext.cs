namespace FinTrackAI;
using System.Data;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.ID);

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasDefaultValue(OptionsRole.USER);
            
            entity.Property(u => u.StatusUsuario)
                .HasConversion<string>()
                .HasDefaultValue(OptionsStatusUser.ATIVO);

            entity.Property(u => u.Nome)
                .IsRequired();

            entity.Property(u => u.Email)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.CPF)
                .IsUnique();
        });
    }
}
