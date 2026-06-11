using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CDS.ImageDisplay.WinForms;

internal static class Guard
{
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
#if NET48
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
#else
        ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
    }
}
