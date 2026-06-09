using System;
using System.Collections.Generic;
using System.Text.Json;
using CDS.ImageDisplay.Annotations.Json;

namespace CDS.ImageDisplay.Annotations;

/// <summary>
/// Provides JSON serialization and deserialization for <see cref="Annotation"/> objects.
/// Uses polymorphic type handling so every built-in geometry subtype round-trips correctly.
/// Register additional geometry subtypes with <c>[JsonDerivedType]</c> on a derived class if needed.
/// </summary>
public static class AnnotationSerializer
{
    /// <summary>
    /// Creates a <see cref="JsonSerializerOptions"/> instance pre-configured with all converters
    /// required for annotation serialization.
    /// </summary>
    /// <returns>A new options instance; callers may add further converters without affecting others.</returns>
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new Json.PointConverter());
        options.Converters.Add(new Json.PointFConverter());
        options.Converters.Add(new Json.RectangleConverter());
        options.Converters.Add(new Json.RectangleFConverter());
        return options;
    }

    /// <summary>
    /// Serializes a single <see cref="Annotation"/> to a JSON string.
    /// </summary>
    /// <param name="annotation">The annotation to serialize.</param>
    public static string Serialize(Annotation annotation)
    {
        Guard.ThrowIfNull(annotation, nameof(annotation));
        return JsonSerializer.Serialize(annotation, CreateOptions());
    }

    /// <summary>
    /// Serializes a collection of <see cref="Annotation"/> objects to a JSON string array.
    /// </summary>
    /// <param name="annotations">The annotations to serialize.</param>
    public static string Serialize(IEnumerable<Annotation> annotations)
    {
        Guard.ThrowIfNull(annotations, nameof(annotations));
        return JsonSerializer.Serialize(annotations, CreateOptions());
    }

    /// <summary>
    /// Deserializes a single <see cref="Annotation"/> from a JSON string.
    /// Returns <see langword="null"/> if the JSON represents a null value.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    public static Annotation? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Annotation>(json, CreateOptions());
    }

    /// <summary>
    /// Deserializes a single annotation as the given type <typeparamref name="T"/>.
    /// Returns <see langword="null"/> if the JSON represents a null value.
    /// </summary>
    /// <typeparam name="T">A concrete <see cref="Annotation"/> subtype.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    public static T? Deserialize<T>(string json) where T : Annotation
    {
        return JsonSerializer.Deserialize<T>(json, CreateOptions());
    }

    /// <summary>
    /// Deserializes a JSON array to a list of <see cref="Annotation"/> objects.
    /// Returns an empty list if the JSON represents null or an empty array.
    /// </summary>
    /// <param name="json">The JSON string containing a JSON array of annotations.</param>
    public static IReadOnlyList<Annotation> DeserializeList(string json)
    {
        return JsonSerializer.Deserialize<List<Annotation>>(json, CreateOptions()) ?? [];
    }
}
