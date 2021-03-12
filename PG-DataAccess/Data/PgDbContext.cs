using Microsoft.EntityFrameworkCore;
using PG_DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PG_DataAccess.Data
{
    public class PgDbContext : DbContext
    {
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
        {
        }

        public DbSet<PaymentRequest> paymentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Payment Requests Table
            modelBuilder.Entity<PaymentRequest>().HasData(new PaymentRequest
            {
                paymentId = Guid.NewGuid(),
                currency = "GBP",
                amount = 100.00M,
                cardNumberMasked = "************1234",
                expiryDate = "06/21",
                cardHolder = "MR JOHN HAMILTON-SMITH",
                paymentSuccessful = true
            });

            modelBuilder.Entity<PaymentRequest>().HasData(new PaymentRequest
            {
                paymentId = Guid.NewGuid(),
                currency = "GBP",
                amount = 250.00M,
                cardNumberMasked = "************5678",
                expiryDate = "06/21",
                cardHolder = "MS JANE HAMILTON-SMITH",
                paymentSuccessful = true
            });
        }
    }
}
