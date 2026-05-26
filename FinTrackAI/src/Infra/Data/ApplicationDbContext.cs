namespace FinTrackAI;
using System.Data;
using System.Transactions;
using FinTrackAI.src.Domain.Entities;
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
            entity.HasAlternateKey(t => t.SenderAccountId);

            entity.Property(t => t.Type)
            .HasConversion<string>()
            .HasDefaultValue(OptionsTipoDaTransferencia.Deposito);

            entity.Property(t => t.Status)
            .HasConversion<string>()
            .HasDefaultValue(TransactionStatus.Active);


            entity.Property(t => t.Amount)
            .IsRequired();

            entity.HasOne(t => t.SenderAccount)
            .WithMany(a => a.TransacoesEnviadas)
             .HasForeignKey(t => t.SenderAccountId)
             .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.ReceiverAccount)
            .WithMany(a => a.TransacoesRecebidas)
             .HasForeignKey(t => t.ReceiverAccountId)
             .OnDelete(DeleteBehavior.Restrict);

        });
        modelBuilder.Entity<Accounts>(entity =>
        {
            entity.HasKey(a => a.ID);
            
            entity.HasAlternateKey(a => a.UserID);
            entity.HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Accounts>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(a => a.Saldo)
           .IsRequired();
            
            entity.Property(a => a.TipoConta)
            .IsRequired();

            entity.Property(a => a.Status)
            .HasConversion<string>()
            .HasDefaultValue(OptionsStatus.ATIVO)
            .IsRequired();
        });
    }
}
