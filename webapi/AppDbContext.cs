namespace webapi;

using webapi.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Transacao

        #endregion
        
        #region Pessoa
        
        modelBuilder.Entity<Pessoa>()
            .HasMany(p => p.Transacoes)
            .WithOne(t => t.Pessoa)
            .HasForeignKey("PessoaId");
            

        #endregion

        #region Categoria

        modelBuilder.Entity<Categoria>()
            .HasMany(c => c.Transacoes)
            .WithOne(t => t.Categoria)
            .HasForeignKey("CategoriaId");

        #endregion

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }


    private void AddTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseModel)entry.Entity;
            var now = DateTime.UtcNow;
            entity.UpdatedAt = now;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
            }
        }
    }
}
