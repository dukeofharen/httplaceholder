﻿using System.IO;
using Placeholder.Services;

namespace Placeholder.DataLogic.Implementations
{
   internal class StubRootPathResolver : IStubRootPathResolver
   {
      private readonly IAssemblyService _assemblyService;
      private readonly IConfigurationService _configurationService;
      private readonly IFileService _fileService;

      public StubRootPathResolver(
         IAssemblyService assemblyService,
         IConfigurationService configurationService,
         IFileService fileService)
      {
         _assemblyService = assemblyService;
         _configurationService = configurationService;
         _fileService = fileService;
      }

      public string GetStubRootPath()
      {
         // First, check the "inputFile" configuration property and extract the directory of this folder.
         var config = _configurationService.GetConfiguration();
         if (config.TryGetValue("inputFile", out string inputFile))
         {
            return 
               _fileService.IsDirectory(inputFile) ? 
               inputFile :
               Path.GetDirectoryName(inputFile);
         }
         else
         {
            // If no input file was provided, return the assembly path instead.
            return _assemblyService.GetAssemblyRootPath();
         }
      }
   }
}
