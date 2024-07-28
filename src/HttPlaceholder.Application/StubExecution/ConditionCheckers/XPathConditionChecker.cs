using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the XML in the request body corresponds to a given list of XPath
///     expressions.
/// </summary>
public class XPathConditionChecker(IHttpContextService httpContextService) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 0;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Xpath?.Any() == true;

    /// <inheritdoc />
    protected override async Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var xpathConditions = stub.Conditions.Xpath.ToArray();
        var validXpaths = 0;
        var body = await httpContextService.GetBodyAsync(cancellationToken);
        var doc = XmlUtilities.LoadXmlDocument(body);
        foreach (var condition in xpathConditions)
        {
            var nsManager = GetNamespaces(condition.Namespaces, doc, body);
            var elements = doc.SelectNodes(condition.QueryString, nsManager);
            if (elements is { Count: 0 })
            {
                // No suitable XML results found.
                return await InvalidAsync(string.Format(StubResources.XPathConditionFailed, condition.QueryString));
            }

            validXpaths++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the header condition is passed and the stub ID is passed to the result.
        return validXpaths == xpathConditions.Length
            ? await ValidAsync()
            : await InvalidAsync();
    }

    private static XmlNamespaceManager GetNamespaces(
        IDictionary<string, string> namespaces,
        XmlDocument doc,
        string body)
    {
        var nsManager = new XmlNamespaceManager(doc.NameTable);
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
            nsManager.ParseBodyAndAssignNamespaces(body);
        }

        return nsManager;
    }
}
