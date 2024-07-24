using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FunctionsApp.Bindings;

public class DefaultBindQueryConversionFeature : IBindQueryConversionFeature
{
    public ValueTask<object?> ConvertAsync(FunctionContext context, Type targetType)
    {
        var requestData = context.GetHttpRequestDataAsync();
        if (requestData.IsCompletedSuccessfully)
        {
            return ConvertRequestAsync(requestData.Result, context, targetType);
        }

        return ConvertAsync(requestData, context, targetType);
    }

    private async ValueTask<object?> ConvertAsync(ValueTask<HttpRequestData?> requestDataResult, FunctionContext context, Type targetType)
    {
        var requestData = await requestDataResult;
        return await ConvertRequestAsync(requestData, context, targetType);
    }

    private ValueTask<object?> ConvertRequestAsync(HttpRequestData? requestData, FunctionContext context, Type targetType)
    {
        if (requestData is null)
        {
            throw new InvalidOperationException($"The '{nameof(DefaultBindQueryConversionFeature)} expects an '{nameof(HttpRequestData)}' instance in the current context.");
        }

        return ConvertQueryAsync(requestData, context, targetType);
    }

    private ValueTask<object?> ConvertQueryAsync(HttpRequestData requestData, FunctionContext context, Type targetType)
    {
        var query = requestData.Url.Query;
        var queryDictionary = QueryHelpers.ParseQuery(query);
        var queryObject = Activator.CreateInstance(targetType);
        var properties = targetType.GetProperties();
        
        foreach (var property in properties)
        {
            if (queryDictionary.TryGetValue(property.Name, out var values))
            {
                var value = values.FirstOrDefault();
                if (value is not null)
                {
                    property.SetValue(queryObject, Convert.ChangeType(value, property.PropertyType));
                }
            }
        }

        return new ValueTask<object?>(queryObject);
    }
}