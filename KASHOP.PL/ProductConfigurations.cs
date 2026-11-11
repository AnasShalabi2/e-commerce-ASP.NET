using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KASHOP.PL
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        { 
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price)
                .HasPrecision(6, 2);
        }
    }
}
