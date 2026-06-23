# CDS.ImageDisplay

A Windows Forms library for displaying, zooming, panning, and annotating bitmaps. Designed for imaging and machine-vision applications where you need a responsive, thread-safe image viewer with overlay graphics and interactive region-of-interest selection.

## Requirements

- .NET 10, Windows
- Windows Forms

## Installation

```
dotnet add package CDS.ImageDisplay.WinForms
```

## Quick start

```csharp
// Drop BitmapDisplayPanel onto a form (designer or code), then:
bitmapDisplayPanel.DisplayMode = BitmapDisplayMode.FitToWindowCentred;

using var bitmap = new Bitmap("photo.jpg");
bitmapDisplayPanel.SetImage(bitmap);   // safe to call from any thread
```

## Features

| Area | Summary |
|---|---|
| [Display](docs/display.md) | Zoom/pan, display modes, thread-safe updates, coordinate mapping, overlay scale factor, greyscale palette modes, custom image sources, paint hooks |
| [Overlays](docs/overlays.md) | Static shapes (`RectangleShape`, `CircleShape`, `LineShape`, …), `DrawingSpec`, `TextPanelStd` |
| [Annotations](docs/annotations.md) | Interactive freehand annotation, shape recognition, serialization, custom shapes |
| [Regions of interest](docs/regions-of-interest.md) | `SingleROIManager`, `MultipleROIManager`, line selection |
| [Image browsing](docs/image-browsing.md) | `ImageListPanel` — virtualised thumbnail list, LRU cache, debounced loading, per-item status indicators |

## Solution layout

| Project | Role |
|---|---|
| `CDS.ImageDisplay.WinForms` | Library — the NuGet deliverable |
| `UnitTests` | MSTest + AwesomeAssertions unit tests |
| `BenchmarkTests` | BenchmarkDotNet benchmarks (Release config) |
| `CDS.ImageDisplay.Demo` | WinForms demo app |

## Building

```
dotnet build
dotnet test
dotnet pack CDS.ImageDisplay.WinForms
```

Versioning is handled by [MinVer](https://github.com/adamralph/minver) using a `V` tag prefix (e.g. `V2.2.0`).

## License

[MIT](LICENSE)
