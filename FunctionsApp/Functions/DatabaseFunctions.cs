using System.Net;
using FunctionsApp.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace FunctionsApp.Functions;

public sealed record CreateMessageRequest(string Message);

public sealed class DatabaseFunctions(MyDbContext dbContext)
{
    [Function(nameof(CreateMessage))]
    [OpenApiOperation(nameof(CreateMessage))]
    [OpenApiRequestBody("application/json", typeof(CreateMessageRequest))]
    [OpenApiResponseWithoutBody(HttpStatusCode.Created)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
    public async Task<IActionResult> CreateMessage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequest req,
        [FromBody] CreateMessageRequest requestBody,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(requestBody.Message))
        {
            return new BadRequestObjectResult("Message contained no content.");
        }
        
        dbContext.Messages.Add(new Message { Content = requestBody.Message });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedResult();
    }

    [Function(nameof(GetMessages))]
    [OpenApiOperation(nameof(GetMessages))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<string>))]
    public async Task<IActionResult> GetMessages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET")]
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        var messages = await dbContext.Messages.Select(x => x.Content).ToListAsync(cancellationToken);
        return new OkObjectResult(messages);
    }
}
