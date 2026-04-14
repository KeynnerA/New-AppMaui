using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiApp1.Converters;

public class ContenidoNotaAFormattedStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var contenido = value as string ?? string.Empty;
        var resultado = new FormattedString();
        var lineas = contenido.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

        for (var i = 0; i < lineas.Length; i++)
        {
            var linea = lineas[i];
            var recortada = linea.TrimStart();
            var esMarcada = recortada.StartsWith("☑ ");

            if (recortada.StartsWith("☑ ") || recortada.StartsWith("☐ "))
            {
                resultado.Spans.Add(new Span { Text = recortada[..2] });
                resultado.Spans.Add(new Span
                {
                    Text = recortada[2..],
                    TextDecorations = esMarcada ? TextDecorations.Strikethrough : TextDecorations.None
                });
            }
            else
            {
                resultado.Spans.Add(new Span { Text = linea });
            }

            if (i < lineas.Length - 1)
            {
                resultado.Spans.Add(new Span { Text = Environment.NewLine });
            }
        }

        return resultado;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
