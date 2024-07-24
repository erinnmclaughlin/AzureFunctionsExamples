using Microsoft.Azure.Functions.Worker.Converters;

namespace FunctionsApp.Bindings;

public class BindQueryAttribute : InputConverterAttribute
{
    public BindQueryAttribute() : base(typeof(BindQueryConverter))
    {
        
    }
}
