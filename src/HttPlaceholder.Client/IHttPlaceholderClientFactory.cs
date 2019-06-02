using System;
using System.Collections.Generic;
using System.Text;

namespace HttPlaceholder.Client
{
    public interface IHttPlaceholderClientFactory
    {
        IMetadataClient MetadataClient { get; }

        IRequestClient RequestClient { get; }

        IStubClient StubClient { get; }

        ITenantClient TenantClient { get; }

        IUserClient UserClient { get; }
    }
}
