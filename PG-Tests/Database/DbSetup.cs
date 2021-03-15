using Microsoft.EntityFrameworkCore;
using PG_DataAccess.Data;
using PG_DataAccess.Models;
using System;

namespace PG_Tests.Database
{
    /// <summary>
    /// When each test is run DbContextOptions are configured and passed to the base class constructor.
    /// These options are stored in a property and used throughout the tests for creating DbContext 
    /// instances. A Seed method is called to create and seed the database and ensures the database 
    /// is clean by deleting it and then re-creating it.
    /// </summary>
    public abstract class DbSetup 
    {
        protected DbSetup(DbContextOptions<PgDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<PgDbContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new PgDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var payment1 = new PaymentRequest()
                {
                    paymentId = Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b117"),
                    currency = "GBP",
                    amount = 100.00M,
                    cardNumberMasked = "1234567890123456",
                    expiryDate = "06/21",
                    cardHolder = "MR JOHN HAMILTON-SMITH",
                    paymentSuccessful = true
                };

                var payment2 = new PaymentRequest()
                {
                    paymentId = Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b118"),
                    currency = "USD",
                    amount = 249.99M,
                    cardNumberMasked = "6543210987654321",
                    expiryDate = "March/2029",
                    cardHolder = "MS JANE HAMILTON-SMITH",
                    paymentSuccessful = false
                };

                context.AddRange(payment1, payment2);

                context.SaveChanges();
            }
        }
    }
}