using System.Linq;
using System.Xml;
using HttPlaceholder.Services;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
   public class XPathConditionChecker : IConditionChecker
   {
      private readonly IHttpContextService _httpContextService;

      public XPathConditionChecker(IHttpContextService httpContextService)
      {
         _httpContextService = httpContextService;
      }

      public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
      {
         var result = new ConditionCheckResultModel();
         var xpathConditions = conditions?.Xpath?.ToArray();
         if (xpathConditions != null)
         {
            int validXpaths = 0;
            string body = _httpContextService.GetBody();
            var doc = new XmlDocument();
            doc.LoadXml(body);
            foreach (var condition in xpathConditions)
            {
               var nsManager = new XmlNamespaceManager(doc.NameTable);
               var namespaces = condition.Namespaces;
               if (namespaces != null)
               {
                  foreach (var ns in namespaces)
                  {
                     nsManager.AddNamespace(ns.Key, ns.Value);
                  }
               }

               var elements = doc.SelectNodes(condition.QueryString, nsManager);
               if (elements.Count == 0)
               {
                  // No suitable XML results found.
                  result.Log = $"No suitable XML results found with XPath query {condition}.";
                  break;
               }

               validXpaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validXpaths == xpathConditions.Length)
            {
               result.ConditionValidation = ConditionValidationType.Valid;
            }
            else
            {
               result.ConditionValidation = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
