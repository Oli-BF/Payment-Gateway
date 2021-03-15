using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using PG_Core.Controllers;
using PG_Core.Models;
using PG_Core.Services.Bank;
using PG_DataAccess.Data;
using PG_Tests.Bank;
using PG_Tests.Database;
using System;
using System.Data.Common;
using Xunit;

namespace PG_Tests.Tests
{
    /// <summary>
    /// This class creates an SQL Lite data base purely in memory and it is used for all the tests
    /// also contained within this class.
    /// </summary>
    public class PaymentControllerTests : DbSetup, IDisposable
    {
        private readonly DbConnection _connection;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PaymentControllerTests()
            : base(
                new DbContextOptionsBuilder<PgDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        /// <summary>
        /// This method creates a SQLite in-memory database and opens the connection to it. The created
        /// DbConnection is extracted from the ContextOptions and saved. The connection is disposed 
        /// when the test is disposed so that resources are not leaked.
        /// </summary>
        /// <returns> An SQL Lite Connection </returns>
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();

        /// <summary>
        /// Testing the method: GetPaymentByIdAsync from the PaymentController Class.
        /// </summary>
        [Fact]
        public void GetPaymentByIdTest()
        {
            using (var context = new PgDbContext(ContextOptions))
            {
                // Arrange
                var mockBank = new MockBank();

                var mockLogger = Mock.Of<ILogger<PaymentController>>();

                var controller = new PaymentController(context, mockBank, mockLogger);

                // Payment 1
                // Act
                var payment1 = controller.GetPaymentByIdAsync(
                    Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b117"));

                // Assert
                Assert.Equal(Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b117"),
                    payment1.Result.Value.paymentId);
                Assert.Equal("GBP", payment1.Result.Value.currency);
                Assert.Equal(100M, payment1.Result.Value.amount);
                Assert.Equal("************3456", payment1.Result.Value.cardNumberMasked);
                Assert.Equal("06/21", payment1.Result.Value.expiryDate);
                Assert.Equal("MR JOHN HAMILTON-SMITH", payment1.Result.Value.cardHolder);
                Assert.True(payment1.Result.Value.paymentSuccessful);

                // Payment 2
                // Assert
                var payment2 = controller.GetPaymentByIdAsync(
                    Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b118"));

                Assert.Equal(Guid.Parse("484c2d4a-65ad-4c26-8cc5-8847c1e1b118"),
                    payment2.Result.Value.paymentId);
                Assert.Equal("USD", payment2.Result.Value.currency);
                Assert.Equal(249.99M, payment2.Result.Value.amount);
                Assert.Equal("************4321", payment2.Result.Value.cardNumberMasked);
                Assert.Equal("March/2029", payment2.Result.Value.expiryDate);
                Assert.Equal("MS JANE HAMILTON-SMITH", payment2.Result.Value.cardHolder);
                Assert.False(payment2.Result.Value.paymentSuccessful);
            }
        }

        /// <summary>
        /// Testing the method: PostPaymentRequestAsync from the PaymentController Class.
        /// </summary>
        [Fact]
        public void PostPaymentRequestTest()
        {
            using (var context = new PgDbContext(ContextOptions))
            {
                // Arrange
                var mockBankSuccess = new MockBankSuccess();
                var mockBankFailure= new MockBankFailure();

                var mockLogger = Mock.Of<ILogger<PaymentController>>();

                var controller1 = new PaymentController(context, mockBankSuccess, mockLogger);
                var controller2 = new PaymentController(context, mockBankFailure, mockLogger);

                // Payment 1 - Success
                // Act
                var paymentRequest1 = new FromMerchPayReq()
                {
                    currency = "GBP",
                    amount = 100.00M,
                    cardNumber = "1234567890123456",
                    expiryDate = "06/21",
                    cardHolder = "MR JOHN HAMILTON-SMITH"
                };

                var payment1 = controller1.PostPaymentRequestAsync(paymentRequest1);

                // Assert
                Assert.Equal(Guid.Parse("e191c52e-d815-4ad4-8982-45b64d138fd6"),
                    payment1.Result.Value.paymentId);
                Assert.True(payment1.Result.Value.paymentSuccessful);


                // Payment 2 - Failure
                // Act
                var paymentRequest2 = new FromMerchPayReq()
                {
                    currency = "USD",
                    amount = 249.99M,
                    cardNumber = "6543210987654321",
                    expiryDate = "March/2029",
                    cardHolder = "MS JANE HAMILTON-SMITH",
                };

                var payment2 = controller2.PostPaymentRequestAsync(paymentRequest2);

                // Assert
                Assert.Equal(Guid.Parse("530fa534-c2a4-41f9-9b20-f83368ad351b"),
                    payment2.Result.Value.paymentId);
                Assert.False(payment2.Result.Value.paymentSuccessful);
            }
        }
    }
}