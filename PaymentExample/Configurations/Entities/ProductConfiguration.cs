using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentExample.Data;

namespace PaymentExample.Configurations.Entities
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Starter plan",
                    StripeId = "prod_LxxwZgU3EpFOgn"
                },
                new Product
                {
                    Id = 2,
                    Name = "Premium plan",
                    StripeId = "prod_LxxsTRwCIErUG8"
                },
                new Product
                {
                    Id = 3,
                    Name = "Test product",
                    StripeId = "prod_Lxw2vUnp50qn0Y"
                }
             );
        }
    }
}
