using System.Net;
using FunctionsApp.Bindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace FunctionsApp.Functions;

public class GetRandomNumberQuery
{
    public int? Min { get; set; }
    public int? Max { get; set; }
}

// also see DatabaseFunctions, which include other HTTP endpoints
public sealed class HttpFunctions
{
    [Function(nameof(GetRandomNumber))]
    [OpenApiOperation(nameof(GetRandomNumber))]
    [OpenApiParameter("min", In = ParameterLocation.Query, Type = typeof(int?), Required = false, Description = "The minimum value to generate.")]
    [OpenApiParameter("max", In = ParameterLocation.Query, Type = typeof(int?), Required = false, Description = "The maximum value to generate.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(int), Description = "Returns a random number.")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Description = "Returns an error message.")]
    public IActionResult GetRandomNumber(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET")] 
        HttpRequest request,
        [BindQuery]
        GetRandomNumberQuery queryParams)
    {
        if (queryParams.Min > queryParams.Max)
            return new BadRequestObjectResult("Min value must be less than or equal to max value.");
        
        var number = Random.Shared.Next(queryParams.Min ?? int.MinValue, (queryParams.Max + 1) ?? int.MaxValue);
        return new OkObjectResult(number);
    }
    
    [Function(nameof(ThrowException))]
    [OpenApiOperation(nameof(ThrowException))]
    public void ThrowException([HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequest req)
    {
        throw new Exception("This is an exception!!!");
    }
}