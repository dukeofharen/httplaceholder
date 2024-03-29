﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the XML in the request body corresponds to a given list of XPath
///     expressions.
/// </summary>
public class XPathConditionChecker : IConditionChecker, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    ///     Constructs a <see cref="BasicAuthenticationConditionChecker" /> instance.
    /// </summary>
    public XPathConditionChecker(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var xpathConditions = stub.Conditions?.Xpath?.ToArray() ?? Array.Empty<StubXpathModel>();
        if (!xpathConditions.Any())
        {
            return result;
        }

        var validXpaths = 0;
        var body = await _httpContextService.GetBodyAsync(cancellationToken);
        try
        {
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
                    nsManager.ParseBodyAndAssignNamespaces(body);
                }

                var elements = doc.SelectNodes(condition.QueryString, nsManager);
                if (elements is {Count: 0})
                {
                    // No suitable XML results found.
                    result.Log = $"No suitable XML results found with XPath query {condition.QueryString}.";
                    break;
                }

                validXpaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            result.ConditionValidation = validXpaths == xpathConditions.Length
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;
        }
        catch (XmlException ex)
        {
            result.ConditionValidation = ConditionValidationType.Invalid;
            result.Log = ex.Message;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 0;
}
