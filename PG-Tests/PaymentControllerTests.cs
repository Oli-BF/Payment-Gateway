using PG_DataAccess.Models;
using System;
using System.Linq;
using Xunit;

namespace PG_Tests
{
    public class PaymentControllerTests
    {
        [Fact]
        public void Task_Add_Time_Test()
        {
            //Arrange    
            var factory = new DbSetup();

            //Get the instance of BlogDBContext  
            var context = factory.CreateContextForSQLite();

            //Act   
            for (int i = 1; i <= 5; i++)
            {
                var payment = new PaymentRequest() 
                {
                    paymentId = 1,
                    currency = "GBP",
                    amount = 100.00M,
                    cardNumberMasked = "************1234",
                    expiryDate = "06/21",
                    cardHolder = "MR JOHN HAMILTON-SMITH",
                    paymentSuccessful = true
                };

                var payment2 = new PaymentRequest()
                {
                    paymentId = 2,
                    currency = "USD",
                    amount = 250.00M,
                    cardNumberMasked = "************5678",
                    expiryDate = "06/21",
                    cardHolder = "MS JANE HAMILTON-SMITH",
                    paymentSuccessful = false
                };

                context.paymentRequests.Add(payment);
                context.paymentRequests.Add(payment2);
            }

            context.SaveChanges();


            //Assert    
            //Get the post count  
            var postCount = context.paymentRequests.Count();
            if (postCount != 0)
            {
                Assert.Equal(1000, postCount);
            }

            //Get single post detail  
            //var singlePost = context.paymentRequests.Where(x => x.PostId == 1).FirstOrDefault();
            //if (singlePost != null)
            //{
            //    Assert.Equal("Test Title 1", singlePost.Title);
            //}
        }
    }
}
