using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace UberSy.Api.OpenApi.Transformers;

public class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"UberSy API {version}";

        return Task.CompletedTask;
    }
}
