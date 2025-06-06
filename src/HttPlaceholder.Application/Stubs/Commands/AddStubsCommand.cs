﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands;

/// <summary>
///     A command for adding multiple stubs.
/// </summary>
public class AddStubsCommand(IEnumerable<StubModel> stubs) : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Gets the stubs to add.
    /// </summary>
    public IEnumerable<StubModel> Stubs { get; } = stubs;
}

/// <summary>
///     A command handler for adding multiple stubs.
/// </summary>
public class AddStubsCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
    : IRequestHandler<AddStubsCommand, IEnumerable<FullStubModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(
        AddStubsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Stubs == null || !request.Stubs.Any())
        {
            throw new ArgumentException(StubResources.NoStubsPosted);
        }

        // Validate posted stubs.
        var stubsToAdd = request.Stubs.ToArray();
        var validationResults = stubsToAdd
            .SelectMany(Validate)
            .ToArray();
        if (validationResults.Length != 0)
        {
            throw new ValidationException(validationResults);
        }

        // Validate that none of the posted stubs have the same ID.
        var duplicateIds = stubsToAdd
            .Select(s => s.Id)
            .GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();
        if (duplicateIds.Length != 0)
        {
            throw new ArgumentException(string.Format(StubResources.DuplicatePostedStubIds,
                string.Join(", ", duplicateIds)));
        }

        // Validated that no stubs with the same ID exist in readonly stub sources.
        var stubsFromReadonlySource = await stubContext.GetStubsFromReadOnlySourcesAsync(cancellationToken);
        var duplicateStubs = stubsFromReadonlySource.Where(r =>
            stubsToAdd.Any(s => string.Equals(s.Id, r.Stub.Id, StringComparison.OrdinalIgnoreCase))).ToArray();
        if (duplicateStubs.Length != 0)
        {
            throw new ValidationException(duplicateStubs.Select(s =>
                string.Format(StubResources.StubWithIdExists, s.Stub.Id)));
        }

        // Add the stubs.
        var result = new List<FullStubModel>();
        foreach (var stub in stubsToAdd)
        {
            // First, delete existing stub with same ID.
            await stubContext.DeleteStubAsync(stub.Id, cancellationToken);
            result.Add(await stubContext.AddStubAsync(stub, cancellationToken));
        }

        return result;
    }

    private IEnumerable<string> Validate(StubModel s)
    {
        var validation = stubModelValidator.ValidateStubModel(s);
        return !string.IsNullOrWhiteSpace(s.Id)
            ? validation.Select(v => $"{s.Id}: {v}")
            : validation;
    }
}
