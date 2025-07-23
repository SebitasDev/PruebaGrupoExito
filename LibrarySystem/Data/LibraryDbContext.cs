using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<BibliographicMaterial> BibliographicMaterials { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de MaterialBibliografico con Table Per Hierarchy (TPH)
        modelBuilder.Entity<BibliographicMaterial>(entity =>
        {
            entity.ToTable("bibliographic_materials");
            entity.HasKey(e => e.ISBN);
            entity.Property(e => e.ISBN).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            
            entity.Property(e => e.Type)
                .HasConversion<int>()
                .IsRequired();
            
            // Configurar discriminador usando el enum
            entity.HasDiscriminator(e => e.Type)
                .HasValue<Book>(MaterialTypes.Book)
                .HasValue<Journal>(MaterialTypes.Journal);
            
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.IsAvailable);
        });

        // Configuración de Prestamo
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("loans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ISBN).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            
            entity.HasOne(l => l.Material)
                .WithMany(m => m.Loans)
                .HasForeignKey(l => l.ISBN)
                .HasPrincipalKey(m => m.ISBN)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.ISBN);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RequestDate);
        });

        base.OnModelCreating(modelBuilder);
    }
}