using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentExample.Data;

namespace PaymentExample.Configurations.Entities
{
    public class PriceConfiguration : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.HasData(
                new Price
                {
                    Id = 1,
                    Nickname = "20 USD/per month",
                    StripeId = "price_1LG20OJnaOlhKwhIPtNbGuPc",
                    Currency = "usd",
                    UnitAmount = 20,
                    ProductId = 1,
                    LookupKey = "look_up_starter"
                },
                new Price
                {
                    Id = 2,
                    Nickname = "100 USD/per month",
                    StripeId = "price_1LG1wsJnaOlhKwhIeFUE0g0u",
                    Currency = "usd",
                    UnitAmount = 100,
                    ProductId = 2,                  
                    LookupKey = "look_up_plan"
                }
             );
        }
    }
}
