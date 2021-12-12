using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

public interface IStubModelValidator
{
    IEnumerable<string> ValidateStubModel(StubModel stub);
}