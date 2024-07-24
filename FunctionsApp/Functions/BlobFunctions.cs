using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionsApp.Functions;

public sealed class BlobFunctions(ILogger<BlobFunctions> logger)
{
    [Function(nameof(ProcessFile))]
    public Task ProcessFile(
        [BlobTrigger("examples/{fileName}.{fileExtension}", Connection = "Azure:StorageAccount")]
        string fileContent, 
        string fileName,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing file {fileName}", fileName);
        logger.LogInformation(fileContent);
        return Task.CompletedTask;
    }
}