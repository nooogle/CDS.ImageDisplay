using System;

namespace CDS.ImageDisplay.WinForms.Compatibility;

/// <summary>
/// Provides a minimal <c>MathF</c>-compatible API for targets that do not expose a public implementation.
/// </summary>
public static class MathFCompat
{
    /// <summary>
    /// Represents the ratio of the circumference of a circle to its diameter.
    /// </summary>
    public const float PI = (float)Math.PI;

    /// <summary>
    /// Returns the absolute value of a specified number.
    /// </summary>
    public static float Abs(float value) => Math.Abs(value);

    /// <summary>
    /// Returns the larger of two single-precision floating-point numbers.
    /// </summary>
    public static float Max(float x, float y) => Math.Max(x, y);

    /// <summary>
    /// Returns the smaller of two single-precision floating-point numbers.
    /// </summary>
    public static float Min(float x, float y) => Math.Min(x, y);

    /// <summary>
    /// Returns the square root of a specified number.
    /// </summary>
    public static float Sqrt(float x) => (float)Math.Sqrt(x);

    /// <summary>
    /// Returns the sine of the specified angle.
    /// </summary>
    public static float Sin(float x) => (float)Math.Sin(x);

    /// <summary>
    /// Returns the cosine of the specified angle.
    /// </summary>
    public static float Cos(float x) => (float)Math.Cos(x);

    /// <summary>
    /// Returns the angle whose tangent is the quotient of two specified numbers.
    /// </summary>
    public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);

    /// <summary>
    /// Rounds a value to the nearest integer.
    /// </summary>
    public static float Round(float x) => (float)Math.Round(x);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified number.
    /// </summary>
    public static float Floor(float x) => (float)Math.Floor(x);

    /// <summary>
    /// Returns the smallest integral value greater than or equal to the specified number.
    /// </summary>
    public static float Ceiling(float x) => (float)Math.Ceiling(x);
}
