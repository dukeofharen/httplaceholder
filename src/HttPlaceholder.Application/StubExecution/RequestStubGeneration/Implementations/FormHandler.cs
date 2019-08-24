using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class FormHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var pair = request.RequestParameters.Headers.FirstOrDefault(p =>
                p.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
            string contentType = pair.Value;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return Task.FromResult(false);
            }

            if (!contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            // If the body condition is already set, clear it here.
            stub.Conditions.Body = new string[0];

            var reader = new FormReader(request.RequestParameters.Body);
            var form = reader.ReadForm();
            stub.Conditions.Form = form.Select(f => new StubFormModel
            {
                Key = f.Key,
                Value = f.Value
            });

            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
