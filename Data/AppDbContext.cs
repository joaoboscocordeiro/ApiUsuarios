using ApiUsuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UsuarioModel> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioModel>()
                .Property(usuario => usuario.Role)
                .HasMaxLength(50)
                .HasDefaultValue(UsuarioRoles.User);

            modelBuilder.Entity<UsuarioModel>()
                .Property(usuario => usuario.RefreshTokenHash)
                .HasMaxLength(200);

            modelBuilder.Entity<UsuarioModel>()
                .HasIndex(usuario => usuario.Email)
                .IsUnique();

            modelBuilder.Entity<UsuarioModel>()
                .HasIndex(usuario => usuario.Usuario)
                .IsUnique();
        }
    }
}
