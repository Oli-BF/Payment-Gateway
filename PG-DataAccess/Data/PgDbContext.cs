using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using PG_DataAccess.Models;
using System;

namespace PG_DataAccess.Data
{
    /// <summary>
    /// This is the DBContext class, which is an integral part of Entity Framework. An instance of 
    /// DbContext represents a session with the database and is used to query and save instances 
    /// of entities to the database.
    /// </summary>
    public class PgDbContext : DbContext
    {
        // These would be moved to Azure Blob Storage - see below commented out method.
        static private readonly byte[] key = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
                                           0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
        static private readonly byte[] iv = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                                           0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddDataProtection()
        //        .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"));
        //}

        // Encryption Key and IV
        private readonly byte[] encryptionKey = key;
        private readonly byte[] encryptionIV = iv;
        private readonly IEncryptionProvider encryptionProvider;

        public DbSet<PaymentRequest> paymentRequests { get; set; }

        /// <summary>
        /// DbContext constructor.
        /// </summary>
        /// <param name="options"></param>
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
        {
            this.encryptionProvider = new AesProvider(this.encryptionKey, this.encryptionIV);
        }

        /// <summary>
        /// This method is used to enforce encryption as well as seed the database upon first creation.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(this.encryptionProvider);

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
                currency = "USD",
                amount = 249.99M,
                cardNumberMasked = "************5678",
                expiryDate = "06/21",
                cardHolder = "MS JANE HAMILTON-SMITH",
                paymentSuccessful = false
            });
        }
    }
}