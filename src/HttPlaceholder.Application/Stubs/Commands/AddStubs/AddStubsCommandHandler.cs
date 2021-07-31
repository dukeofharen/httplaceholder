using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStubs
{
    public class AddStubsCommandHandler : IRequestHandler<AddStubsCommand, IEnumerable<FullStubModel>>
    {
        private readonly IStubContext _stubContext;
        private readonly IStubModelValidator _stubModelValidator;

        public AddStubsCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
        {
            _stubContext = stubContext;
            _stubModelValidator = stubModelValidator;
        }

        public async Task<IEnumerable<FullStubModel>> Handle(
            AddStubsCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Stubs == null || !request.Stubs.Any())
            {
                throw new ArgumentException("No stubs posted.");
            }

            // Validate posted stubs.
            var stubsToAdd = request.Stubs.ToArray();
            var validationResults = stubsToAdd
                .SelectMany(s =>
                {
                    var validation = _stubModelValidator.ValidateStubModel(s);
                    return !string.IsNullOrWhiteSpace(s.Id)
                        ? validation.Select(v => $"{s.Id}: {v}")
                        : validation;
                })
                .ToArray();
            if (validationResults.Any())
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
            if (duplicateIds.Any())
            {
                throw new ArgumentException(
                    $"The following stub IDs are posted more than once: {string.Join(", ", duplicateIds)}");
            }

            // Validated that no stubs with the same ID exist in readonly stub sources.
            var stubsFromReadonlySource = await _stubContext.GetStubsFromReadOnlySourcesAsync();
            var duplicateStubs = stubsFromReadonlySource.Where(r =>
                stubsToAdd.Any(s => string.Equals(s.Id, r.Stub.Id, StringComparison.OrdinalIgnoreCase))).ToArray();
            if (duplicateStubs.Any())
            {
                throw new ValidationException(duplicateStubs.Select(s => $"Stub with ID already exists: {s.Stub.Id}"));
            }

            // Add the stubs.
            var result = new List<FullStubModel>();
            foreach (var stub in stubsToAdd)
            {
                result.Add(await _stubContext.AddStubAsync(stub));
            }

            return result;
        }
    }
}
