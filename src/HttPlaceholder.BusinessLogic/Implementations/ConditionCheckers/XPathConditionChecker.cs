using System.Linq;
using System.Xml;
using HttPlaceholder.Services;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using System.Text.RegularExpressions;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
   public class XPathConditionChecker : IConditionChecker
   {
      private const string NamespacesRegexPattern = "xmlns:(.*?)=\"(.*?)\"";
      private readonly IHttpContextService _httpContextService;
      private readonly Regex _namespaceRegex;

      public XPathConditionChecker(IHttpContextService httpContextService)
      {
         _httpContextService = httpContextService;
         _namespaceRegex = new Regex(NamespacesRegexPattern);
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
               else
               {
                  // If no namespaces are defined, check the XML namespaces with a regex.
                  var matches = _namespaceRegex.Matches(body);
                  for (int i = 0; i < matches.Count; i++)
                  {
                     var match = matches[i];

                     // If there is a match, the regex should contain three groups: the full match, the prefix and the namespace
                     if (match.Groups.Count == 3)
                     {
                        var prefix = match.Groups[1].Value;
                        var uri = match.Groups[2].Value;
                        nsManager.AddNamespace(prefix, uri);
                     }
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
