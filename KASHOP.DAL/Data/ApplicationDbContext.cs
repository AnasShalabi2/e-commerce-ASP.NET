using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<CartItem> OrderItems { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option, IHttpContextAccessor httpContextAccessor) : base(option)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != null)
            {
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedById = currentUserId;
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.UpdatedById = currentUserId;
                        entry.Entity.UpdatedOn = DateTime.UtcNow;

                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.Entity<Category>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
              .HasOne(p => p.UpdatedBy)
              .WithMany()
              .HasForeignKey(p => p.UpdatedById)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
              .HasOne(p => p.CreatedBy)
              .WithMany()
              .HasForeignKey(p => p.CreatedById)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
              .HasOne(p => p.UpdatedBy)
              .WithMany()
              .HasForeignKey(p => p.UpdatedById)
              .OnDelete(DeleteBehavior.Restrict);



                
        }
    }
}

