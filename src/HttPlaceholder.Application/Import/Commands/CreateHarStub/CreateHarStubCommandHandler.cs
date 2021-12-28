﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

public class CreateHarStubCommandHandler : IRequestHandler<CreateHarStubCommand, IEnumerable<FullStubModel>>
{
    public Task<IEnumerable<FullStubModel>> Handle(CreateHarStubCommand request, CancellationToken cancellationToken) => throw new System.NotImplementedException();
}
