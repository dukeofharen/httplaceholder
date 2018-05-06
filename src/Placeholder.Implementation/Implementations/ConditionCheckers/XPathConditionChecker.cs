using System.Linq;
using System.Xml;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class XPathConditionChecker : IConditionChecker
   {
      private readonly ILogger<XPathConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;

      public XPathConditionChecker(
         ILogger<XPathConditionChecker> logger,
         IHttpContextService httpContextService)
      {
         _logger = logger;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var result = ConditionValidationType.NotExecuted;
         var xpathConditions = stub.Conditions?.Xpath?.ToArray();
         if (xpathConditions != null)
         {
            _logger.LogInformation($"XPath condition found for stub '{stub.Id}': '{string.Join(", ", xpathConditions.Select(x => x.QueryString))}'");
            int validXpaths = 0;
            string body = _httpContextService.GetBody();
            var doc = new XmlDocument();
            doc.LoadXml(body);
            foreach (var condition in xpathConditions)
            {
               _logger.LogInformation($"Checking posted content with XPath '{condition.QueryString}'.");
               var nsManager = new XmlNamespaceManager(doc.NameTable);
               var namespaces = condition.Namespaces;
               if (namespaces != null)
               {
                  foreach (var ns in namespaces)
                  {
                     _logger.LogInformation($"Adding namespace '{ns.Key}' with value '{ns.Value}' to the namespace manager.");
                     nsManager.AddNamespace(ns.Key, ns.Value);
                  }
               }

               var elements = doc.SelectNodes(condition.QueryString, nsManager);
               if (elements.Count == 0)
               {
                  // No suitable XML results found.
                  _logger.LogInformation("No suitable XML results found with XPath query.");
                  break;
               }

               validXpaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validXpaths == xpathConditions.Length)
            {
               _logger.LogInformation($"XPath condition check succeeded for stub '{stub.Id}'.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
