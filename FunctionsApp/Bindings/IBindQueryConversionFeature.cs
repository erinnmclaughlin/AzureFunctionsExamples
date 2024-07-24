using Microsoft.Azure.Functions.Worker;

namespace FunctionsApp.Bindings;

public interface IBindQueryConversionFeature
{
    ValueTask<object?> ConvertAsync(FunctionContext context, Type targetType);
}
