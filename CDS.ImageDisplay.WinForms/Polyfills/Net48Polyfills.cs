#if NET48
namespace System.Runtime.CompilerServices
{
    // Polyfill required for 'init' property accessors and 'record' types on .NET Framework.
    internal static class IsExternalInit { }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName) =>
            ParameterName = parameterName;

        public string ParameterName { get; }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter |
        AttributeTargets.Property | AttributeTargets.ReturnValue,
        Inherited = false)]
    internal sealed class NotNullAttribute : Attribute { }
}

namespace System
{
    // HashCode was added in .NET Core 2.1 / .NET Standard 2.1 and is not in .NET Framework 4.8.
    internal static class HashCode
    {
        public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + (v1?.GetHashCode() ?? 0);
                h = h * 31 + (v2?.GetHashCode() ?? 0);
                h = h * 31 + (v3?.GetHashCode() ?? 0);
                return h;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + (v1?.GetHashCode() ?? 0);
                h = h * 31 + (v2?.GetHashCode() ?? 0);
                h = h * 31 + (v3?.GetHashCode() ?? 0);
                h = h * 31 + (v4?.GetHashCode() ?? 0);
                h = h * 31 + (v5?.GetHashCode() ?? 0);
                return h;
            }
        }
    }

    // MathF was added in .NET Core 2.0 / .NET Standard 2.1 and is not in .NET Framework 4.8.
    // All methods delegate to System.Math with float casts.
    internal static class MathF
    {
        public const float PI = (float)Math.PI;

        public static float Abs(float x) => Math.Abs(x);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Ceiling(float x) => (float)Math.Ceiling(x);
        public static float Cos(float x) => (float)Math.Cos(x);
        public static float Floor(float x) => (float)Math.Floor(x);
        public static float Max(float x, float y) => Math.Max(x, y);
        public static float Min(float x, float y) => Math.Min(x, y);
        public static float Round(float x) => (float)Math.Round(x);
        public static float Sin(float x) => (float)Math.Sin(x);
        public static float Sqrt(float x) => (float)Math.Sqrt(x);
    }
}
#endif
