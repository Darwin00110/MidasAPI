namespace MidasAPI;

using MidasAPI.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transacao> Transacao { get; set; }
    public DbSet<Accounts> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.ID);
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasDefaultValue(OptionsRole.USER);
            entity.Property(u => u.Status)
                .HasConversion<string>()
                .HasDefaultValue(OptionsStatus.ATIVO);
            entity.Property(u => u.Nome)
                .IsRequired();
            entity.Property(u => u.Email)
                .IsRequired();
            entity.HasIndex(u => u.Email)
                .IsUnique();
            entity.HasIndex(u => u.CPF)
                .IsUnique();
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.ID);


            entity.Property(t => t.Tipo)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasDefaultValue(OptionsStatusDaTransferencia.PENDENTE);
            entity.Property(t => t.Valor)
                .IsRequired();
            entity.Property(t => t.ValorLiquido)
                .IsRequired();
            entity.Property(t => t.Protocolo)
                .IsRequired();

            entity.HasOne(t => t.ContaOrigem)
                .WithMany(a => a.TransacoesEnviadas)
                .HasForeignKey(t => t.ContaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.ContaDestino)
                .WithMany(a => a.TransacoesRecebidas)
                .HasForeignKey(t => t.ContaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Accounts>(entity =>
        {
            entity.HasKey(a => a.ID);

            entity.HasIndex(a => a.UserID)
                .IsUnique();

            entity.HasOne(a => a.User)
                .WithOne(u => u.Accounts)
                .HasForeignKey<Accounts>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(a => a.Saldo)
                .IsRequired();
            entity.Property(a => a.TipoConta)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(a => a.Status)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(a => a.NumeroConta)
                .IsRequired();
            entity.Property(a => a.NumeroAgencia)
                .IsRequired();

            entity.Property(a => a.ChavePix)
                .IsRequired();
        });
    }
}