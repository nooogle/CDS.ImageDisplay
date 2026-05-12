# CDS.Imaging — Code Review Todos

---

## Design Issues


- [x] **i17** Review: `GetROIDescriptors` callback pattern in `MultipleROIManager`  
  Replaced `Func<IEnumerable<ISingleROIDescriptor>>` with `IReadOnlyList<ISingleROIDescriptor> ROIDescriptors`. Covariance lets callers assign `BindingList<T>` or `List<T>` directly; the live reference means the manager always sees the current contents without needing a callback.

- [x] **i18** `RectangleShape`: `DirectToDisplay` mode uses `Truncate`, `ImageToDisplay` mode uses `Round`  
  The same `Rect` value can render at different pixel positions depending on mapping mode, which is visually inconsistent. Use `Rectangle.Round` in both branches, or keep everything as `RectangleF` (GDI+ `DrawRectangle`/`FillRectangle` both accept `RectangleF`).

- [x] **i19** Review: pending image drop behaviour in `SetImageIndirectlyFromNonUIThread`  
  When a new frame arrives on a non-UI thread while the previous pending frame hasn't been applied yet, the new frame is silently dropped (backpressure). Verify this is the correct policy. An alternative is to replace the pending frame (last-writer-wins), so the displayed image is always the most recent one rather than the one that arrived first.

- [x] **i20** Auto-deselect timer in `MultipleROIManager` — replaced with two nullable-TimeSpan properties  
  `ClickDeselectDelay` (null = never) for click-only selections; `MoveDeselectDelay` (2 s default) for after a commit. Both control independent `Timer` instances. Dragging stops both; `DeselectActiveROI` stops both regardless of trigger.

---

## Nits & Minor Improvements

- [x] **i21** Delete `MouseMode.cs` — entire file is commented-out dead code

- [x] **i22** `BitmapDisplayMetrics.SetImage` is declared but never measured — removed.

- [x] **i23** `FontSpec.GetHashCode` — replaced XOR with `HashCode.Combine(FontSize, FontName, FontStyle)`.

- [x] **i24** `FontSpec.Equals` — aligned with `PenSpec` pattern (`obj is not FontSpec other`).

- [x] **i25** `FontSpec` — added `FontStyle` property; included in `Equals`/`GetHashCode`/`Create()`/`ToString()`.

- [x] **i26** `PolygonShape.Points` — removed unreachable null guard; default changed from `new PointF[0]` to `[]`.

- [x] **i27** `DrawingSpec.ToString()` returns `$"Visible={Visible}, Mode={MappingMode}"`. `ROIWithGrapplesShape.ToString()` returns `$"{Name}: {ROI}"`.

- [x] **i28** `ISingleROIDescriptor.Locked` XML doc fixed ("should NOT be editable").

- [x] **i29** Removed empty `BitmapDisplayPanel_KeyPress` handler and its event subscriptions from `SingleROIManager`.

- [x] **i30** Removed unused imports from `ImageWrapper.cs` and `BitmapImageSource.cs`.

- [x] **i31** `ZoomManager` — removed `Math.Abs`; `changeFactor` is now computed inside each branch using the known sign of `change`.

- [x] **i32** `OnPaintBackground` — comparison now uses `Rectangle.Truncate(clippedDrawingRect)` so the optimisation fires correctly when the paint rect has fractional coordinates.

- [x] **i33** `Win32.cs` — migrated to `[LibraryImport]` with `partial static` class and method.
