﻿using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetByStubId
{
    public class GetByStubIdQuery : IRequest<IEnumerable<RequestResultModel>>
    {
        public GetByStubIdQuery(string stubId)
        {
            StubId = stubId;
        }

        public string StubId { get; }
    }
}
