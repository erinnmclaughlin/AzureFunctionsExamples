using Microsoft.Azure.Functions.Worker.Converters;

namespace FunctionsApp.Bindings;

internal sealed class BindQueryConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        var conversionFeature = context.FunctionContext.Features.Get<IBindQueryConversionFeature>()
            ?? new DefaultBindQueryConversionFeature();

        var result = conversionFeature.ConvertAsync(context.FunctionContext, context.TargetType);

        if (result.IsCompletedSuccessfully)
        {
            return new ValueTask<ConversionResult>(ConversionResult.Success(result.Result));
        }

        return HandleResultAsync(result);
    }

    private async ValueTask<ConversionResult> HandleResultAsync(ValueTask<object?> result)
    {
        object? bodyResult = await result;

        return ConversionResult.Success(bodyResult);
    }
}
