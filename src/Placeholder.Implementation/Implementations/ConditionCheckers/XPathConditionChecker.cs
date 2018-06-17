using System.Linq;
using System.Xml;
using Placeholder.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class XPathConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public XPathConditionChecker(
         IRequestLoggerFactory requestLoggerFactory,
         IHttpContextService httpContextService)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(string stubId, StubConditionsModel conditions)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         var xpathConditions = conditions?.Xpath?.ToArray();
         if (xpathConditions != null)
         {
            requestLogger.Log($"XPath condition found for stub '{stubId}': '{string.Join(", ", xpathConditions.Select(x => x.QueryString))}'");
            int validXpaths = 0;
            string body = _httpContextService.GetBody();
            var doc = new XmlDocument();
            doc.LoadXml(body);
            foreach (var condition in xpathConditions)
            {
               requestLogger.Log($"Checking posted content with XPath '{condition.QueryString}'.");
               var nsManager = new XmlNamespaceManager(doc.NameTable);
               var namespaces = condition.Namespaces;
               if (namespaces != null)
               {
                  foreach (var ns in namespaces)
                  {
                     requestLogger.Log($"Adding namespace '{ns.Key}' with value '{ns.Value}' to the namespace manager.");
                     nsManager.AddNamespace(ns.Key, ns.Value);
                  }
               }

               var elements = doc.SelectNodes(condition.QueryString, nsManager);
               if (elements.Count == 0)
               {
                  // No suitable XML results found.
                  requestLogger.Log("No suitable XML results found with XPath query.");
                  break;
               }

               validXpaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validXpaths == xpathConditions.Length)
            {
               requestLogger.Log($"XPath condition check succeeded for stub '{stubId}'.");
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
