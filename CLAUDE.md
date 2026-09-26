# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

CDS.ImageDisplay is a Windows Forms library (targeting `net10.0-windows`) for displaying, zooming, panning, and annotating images. It is published to NuGet as "CDS Imaging" and versioned via MinVer with a `V` tag prefix (e.g. `V2.2.0`).

## Build & test commands

```
dotnet build                          # build all projects
dotnet test                           # run unit tests
dotnet test --filter "FullyQualifiedName~VirtualDisplay"  # run a single test class
dotnet run --project BenchmarkTests   # run benchmarks (Release config recommended)
dotnet pack CDS.ImageDisplay.WinForms      # produce NuGet package
```

**Solution build quirks**: `BenchmarkTests` is excluded from Debug builds; `UnitTests` is excluded from Release builds. This is intentional in the `.slnx`.

## Project layout

| Project | Role |
|---|---|
| `CDS.ImageDisplay.WinForms` | Core library — the NuGet deliverable |
| `UnitTests` | MSTest + AwesomeAssertions unit tests |
| `BenchmarkTests` | BenchmarkDotNet performance tests for resource pooling |
| `CDS.ImageDisplay.Demo` | WinForms demo app; uses OpenCvSharp4 — not part of the library |

## Architecture

### BitmapDisplay namespace

The central control is `BitmapDisplayPanel` (a `UserControl`). It composes:

- **`VirtualDisplay`** — pure coordinate math: maps image pixels to screen pixels given zoom, pan offset, and display mode. No GDI+ dependency. This is the right place for zoom/pan logic.
- **`ZoomManager`** — zoom step control; snaps to exactly 1.0× within a tolerance.
- **`DragManager`** — translates mouse deltas into pan offsets.
- **`ImageWrapper`** — owns the displayed `Bitmap`; reuses the existing bitmap when the new spec matches, avoiding unnecessary allocations.
- **`IImageSource`** — abstraction callers provide to update the display (exposes Width, Height, Stride, PixelFormat, Scan0). `BitmapImageSource` is the standard implementation.

Display modes (`BitmapDisplayMode`): `FitToWindowCentred`, `ActualSizeCentred`, `Free`.

`BitmapDisplayPanel` exposes paint-hook events (`OnPaintOver`, `OnPaintUnder`) and a `PaintRectChanged` event so callers can synchronise overlays without subclassing.

### Overlays namespace

Shapes are added to a `BitmapDisplayPanel` and drawn via its paint cycle. Each shape has a `DrawingSpec` that carries:

- `PenSpec` / `BrushSpec` / `FontSpec` — style configuration; each has a `Create()` method that produces the corresponding GDI+ object.
- `MappingMode` — `ImageToDisplay` (scales and pans with the image) or `DirectToDisplay` (fixed screen coordinates).
- Visibility flag.

`DrawingToolsPool` caches `Pen`, `Brush`, and `Font` objects keyed on spec equality — avoids GDI+ churn in paint loops. Shape classes (`RectangleShape`, `CircleShape`, `LineShape`, `TextShape`, `CrosshairShape`, etc.) call into the pool.

`ColorJsonConverter` handles `System.Drawing.Color` ↔ JSON because the built-in serializer doesn't support it.

### RegionOfInterest namespace

- `SingleROIManager` — owns one interactive ROI on a `BitmapDisplayPanel`. Handles hit-testing against eight resize grapples plus a drag-to-move centre. Exposes `OnCommittedROIChanged` (mouse-up) and `OnDraggingROIChanged` (live).
- `MultipleROIManager` — delegates to a list of `ISingleROIDescriptor` objects; tracks the active one during interaction.
- `ROIWithGrapplesShape` — implements `ISingleROIDescriptor` and `ISingleROI`; renders the rectangle and its grapple handles. Min/max size constraints live here.

Implement `ISingleROIDescriptor` to add custom ROI types to `MultipleROIManager`.

### Utils namespace

- `DrawingToolsPool` — shared GDI+ resource cache (also used from Overlays).
- `UIDispatcher` — captures the UI thread and its `SynchronizationContext`; use `IsOnUIThread`/`TryPost` instead of `InvokeRequired`/`BeginInvoke` (which misbehave when a control has no handle). `BitmapDisplayPanel.SetImage` uses it for cross-thread updates.
- `FormStatePersister` — saves/restores a form's bounds and maximised state. Restores in `ISupportInitialize.EndInit()` (designer-emitted, before the handle exists), never from `Load`: by Load the form is `Visible`, so setting `WindowState` calls `ShowWindow` and shows it mid-Load. `EndInit` runs before the designer's `ResumeLayout`, so the size is pre-divided by the pending auto-scale factor. Designer files without `EndInit` fall back to `HandleCreated` (bounds) and `Shown` (maximise, since `Form.CreateHandle` resets a state set earlier).
- `SystemInfo` / `SystemInfoPanel` — GPU/CPU/memory introspection for diagnostics.
- `Win32` — P/Invoke declarations.
- `SerializableExpandableObjectConverter` — designer TypeConverter base for specs.

## Testing conventions

Tests live in `UnitTests/`. Key classes:

- `VirtualDisplayTests` — coordinate mapping; most zoom/pan edge-case logic is here.
- `SpecTests` — equality and `Create()` correctness for `PenSpec`, `BrushSpec`, `FontSpec`.
- `ColorJsonConverterTests` — round-trip serialization.
- `PointFConverterTests`, `RectangleFConverterTests` — TypeConverter correctness.
- `BitmapDisplayPanelThreadingTests` — cross-thread `SetImage`, driven through `UIThreadHarness`.
- `BitmapDisplayPanelConcurrencyTests` — the same code under real lock contention.
- `FormStatePersisterTests` — restore timing; builds forms the way `InitializeComponent` does and records any show during `OnLoad`.

Test method naming: `MethodName_Scenario_ExpectedResult`. Use `[TestCategory]` for grouping where helpful. Use AwesomeAssertions (not FluentAssertions) — the package is `AwesomeAssertions`.

Tests run sequentially — `[assembly: DoNotParallelize]` in each test project's
`MSTestSettings.cs`. This is a UI suite: tests construct WinForms controls, run STA threads
and share process-wide WinForms and GDI+ state. Keep it that way, and still don't let a test
mutate process-wide state — `WindowsFormsSynchronizationContext.AutoInstall` in particular.

To test anything that marshals to the UI thread, use `UIThreadHarness` with a
`QueuedSynchronizationContext`: it runs the body on an STA thread and lets the test choose when
posted work runs, instead of pumping with `Application.DoEvents` and sleeping. WinForms only
auto-installs its own context over one that is null or exactly `SynchronizationContext`, so a
derived context survives control construction untouched. Avoid `Application.DoEvents` in tests:
it processes messages until the queue is empty, which a thread posting flat out can prevent
indefinitely.

`UITests/` is a FlaUI rig (harness launcher, sample-image factory) driving the
`CDS.ImageDisplay.WinForms.TestHarness` app out of process. It currently holds no tests, so it
passes `--ignore-exit-code 8`; CI runs `UnitTests` only.

## Coordinate system note

`VirtualDisplay` operates in three coordinate spaces: *image* (pixels in the source bitmap), *display* (pixels in the control client area), and *paint-rect* (the sub-rectangle of the control where the image is rendered). Always be explicit about which space a `PointF`/`RectangleF` lives in when working on zoom or ROI math.
