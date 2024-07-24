using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace FunctionsApp.Middleware;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var functionName = context.FunctionDefinition.Name;
        logger.LogInformation("Start invocation: {FunctionName}", functionName);

        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while attempting to invoke {FunctionName}.", functionName);
            await CreateErrorResponse(context);
        }
        finally
        {
            logger.LogInformation("Finish invocation: {FunctionName}", functionName);
        }
    }
    
    private static async Task CreateErrorResponse(FunctionContext context)
    {
        var requestData = await context.GetHttpRequestDataAsync();

        if (requestData is not null)
        {
            var response = requestData.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("Whoops! Something went wrong. Please try again later.");

            var httpOutput = context
                .GetOutputBindings<HttpResponseData>()
                .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");
        
            if (httpOutput is not null)
            {
                httpOutput.Value = response;
            }
            else
            {
                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = response;
            }
        }
    }
}