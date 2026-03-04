namespace webapi;

using webapi.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DBSets das Models
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    /// <summary>
    /// Configura as relações entre os modelos
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
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

    /// <summary>
    /// Função chamada quando é salvo uma alteração no BD, ela é sobrescrita para preencher created_at e updated_at.
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Adiciona os valores para created_at e updated_at nos registros alterados/adicionados
    /// </summary>
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
