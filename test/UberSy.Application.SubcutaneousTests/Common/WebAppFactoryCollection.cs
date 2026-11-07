using Xunit;

namespace UberSy.Application.SubcutaneousTests.Common;

[CollectionDefinition(CollectionName)]
public class WebAppFactoryCollection : ICollectionFixture<WebAppFactory>
{
    public const string CollectionName = "WebAppFactoryCollection";
}
