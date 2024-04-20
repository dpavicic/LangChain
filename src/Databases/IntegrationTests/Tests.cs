namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public partial class Tests
{
    [TestCase(SupportedDatabase.InMemory)]
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.SqLite)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)] // #TODO: Fix OpenSearch tests
    [TestCase(SupportedDatabase.Postgres, Explicit = true)] // #TODO: Fix Postgres tests
    public async Task SimilaritySearchByVector_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);

        await environment.VectorDatabase.AddTextsAsync(environment.EmbeddingModel, Embeddings.Keys);

        var similar = await environment.VectorDatabase.SearchAsync(Embeddings["lemon"], new VectorSearchSettings
        {
            NumberOfResults = 5,
        });
        similar.Items.Should().HaveCount(5);

        var similarTexts = similar.Items.Select(s => s.Text).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }
}