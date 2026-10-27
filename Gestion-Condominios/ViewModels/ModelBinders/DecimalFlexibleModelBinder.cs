using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gestion_Condominios.ViewModels.ModelBinders;

/// <summary>Interpreta decimales enviados por controles HTML number (punto) y por entrada local (coma).</summary>
public sealed class DecimalFlexibleModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (result == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);
        var value = result.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "El valor es obligatorio.");
            return Task.CompletedTask;
        }

        var culture = value.Contains(',', StringComparison.Ordinal) && !value.Contains('.', StringComparison.Ordinal)
            ? CultureInfo.CurrentCulture
            : CultureInfo.InvariantCulture;

        if (decimal.TryParse(value, NumberStyles.Number, culture, out var decimalValue))
        {
            bindingContext.Result = ModelBindingResult.Success(decimalValue);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "El valor debe ser un número decimal válido.");
        }

        return Task.CompletedTask;
    }
}
