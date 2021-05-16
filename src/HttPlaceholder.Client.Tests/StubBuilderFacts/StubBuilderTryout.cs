using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.Tests.StubBuilderFacts
{
    [TestClass]
    public class StubBuilderTryout
    {
        [TestMethod]
        public void Bla123()
        {
            var stub = StubBuilder.Begin()
                .WithId("stub123")
                .WithJsonObject(new
                {
                    duco = "winterwerp"
                })
                .Build();
            var json = JsonConvert.SerializeObject(stub);
        }
    }
}
