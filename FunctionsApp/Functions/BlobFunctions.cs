using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionsApp.Functions;

public sealed class BlobFunctions(ILogger<BlobFunctions> logger)
{
    [Function(nameof(ProcessFile))]
    public void ProcessFile(
        [BlobTrigger("examples/{fileName}.{fileExtension}", Connection = "Azure:StorageAccount")]
        string fileContent, 
        string fileName,
        string fileExtension)
    {
        logger.LogInformation("Processing file {FileName}.{FileExtension}", fileName, fileExtension);
        logger.LogInformation(fileContent);
    }
}