using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Models;

namespace Placeholder.DataLogic.Implementations
{
   internal class StubContainer : IStubContainer
   {
      private readonly IServiceProvider _serviceProvider;

      public StubContainer(IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      //public string GetStubFileDirectory()
      //{
      //   string inputFileLocation = _configuration["inputFile"];
      //   string directory = _fileService.GetDirectoryPath(inputFileLocation);
      //   return directory;
      //}

      public async Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         var result = new List<StubModel>();
         var sources = ((IEnumerable<IStubSource>)_serviceProvider.GetServices(typeof(IStubSource))).ToArray();
         foreach(var source in sources)
         {
            result.AddRange(await source.GetStubsAsync());
         }

         return result;
      }
   }
}
