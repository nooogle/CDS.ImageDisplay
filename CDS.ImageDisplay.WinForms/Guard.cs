using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CDS.ImageDisplay.WinForms;

internal static class Guard
{
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
    }
}
