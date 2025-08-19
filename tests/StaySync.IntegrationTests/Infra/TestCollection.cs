using Xunit;

namespace StaySync.IntegrationTests.Infra;

[CollectionDefinition(nameof(IntegrationCollection))]
public sealed class IntegrationCollection : ICollectionFixture<SqlServerContainerFixture> { }
