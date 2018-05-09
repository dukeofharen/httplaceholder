using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations
{
   [TestClass]
   public class StubResponseGeneratorFacts
   {
      private StubResponseGenerator _generator;

      [TestInitialize]
      public void Initialize()
      {
         _generator = new StubResponseGenerator(TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               Text = "321"
            }
         };

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual("321", Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow_Base64()
      {
         // arrange
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               Base64 = "VGhpcyBpcyB0aGUgY29udGVudCE="
            }
         };

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual("This is the content!", Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }
   }
}
