using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TekkenMinimalAPI.Entidades;

namespace TekkenMinimalAPI
{
    public class ApplicationDBContext: IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Personaje
                modelBuilder.Entity<Personaje>().Property(p => p.Nombre).HasMaxLength(30);
                modelBuilder.Entity<Personaje>().Property(p => p.NombreCompleto).HasMaxLength(150);
                modelBuilder.Entity<Personaje>().Property(p => p.TipoDeSangre).HasMaxLength(30);
                modelBuilder.Entity<Personaje>().Property(p => p.Altura).HasColumnType("decimal(18,1)");
                modelBuilder.Entity<Personaje>().Property(p => p.Peso).HasColumnType("decimal(18,2)");
                modelBuilder.Entity<Personaje>().Property(p => p.Foto).IsUnicode();
            #endregion

            #region Usuarios
                modelBuilder.Entity<IdentityUser>().ToTable("Usuarios");
                modelBuilder.Entity<IdentityRole>().ToTable("Roles");
                modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");
                modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuariosClaims");
                modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuariosLogins");
                modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuariosRoles");
                modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuariosTokens");
            #endregion
        }

        public DbSet<Personaje> Personajes { get; set; }
        public DbSet<Error> Errores { get; set; }
    }
}
