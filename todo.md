# CDS.Imaging — Code Review Todos

---

## Design Issues


- [ ] **i17** Review: `GetROIDescriptors` callback pattern in `MultipleROIManager`  
  Currently a `Func<IEnumerable<ISingleROIDescriptor>>` property. A collection property (`IReadOnlyList<ISingleROIDescriptor>`) would be more natural, simpler to use from a designer, and easier to observe for membership changes. Review and decide on preferred API shape.

- [ ] **i18** `RectangleShape`: `DirectToDisplay` mode uses `Truncate`, `ImageToDisplay` mode uses `Round`  
  The same `Rect` value can render at different pixel positions depending on mapping mode, which is visually inconsistent. Use `Rectangle.Round` in both branches, or keep everything as `RectangleF` (GDI+ `DrawRectangle`/`FillRectangle` both accept `RectangleF`).

- [ ] **i19** Review: pending image drop behaviour in `SetImageIndirectlyFromNonUIThread`  
  When a new frame arrives on a non-UI thread while the previous pending frame hasn't been applied yet, the new frame is silently dropped (backpressure). Verify this is the correct policy. An alternative is to replace the pending frame (last-writer-wins), so the displayed image is always the most recent one rather than the one that arrived first.

- [ ] **i20** Auto-deselect timer in `MultipleROIManager` — intent unclear, leave for now  
  After committing a ROI change, a 1-second timer fires and auto-deselects the active ROI. Origin of this behaviour is uncertain. Leave as-is until the intended UX is confirmed.

---

## Nits & Minor Improvements

- [ ] **i21** Delete `MouseMode.cs` — entire file is commented-out dead code

- [ ] **i22** `BitmapDisplayMetrics.SetImage` is declared but never measured  
  The stopwatch in `BitmapDisplayPanel` times background and foreground paint only. Either instrument `SetImageDirectlyFromUIThread` or remove the property.

- [ ] **i23** `FontSpec.GetHashCode` uses XOR — use `HashCode.Combine`  
  `FontSize.GetHashCode() ^ FontName.GetHashCode()` has weaker distribution. Replace with `HashCode.Combine(FontSize, FontName)`.

- [ ] **i24** `FontSpec.Equals` uses old null+type-check pattern — align with `PenSpec`  
  Replace with `if (obj is not FontSpec other) return false;` for consistency.

- [ ] **i25** `FontSpec` has no `FontStyle` — text overlays cannot be bold or italic  
  Add `public FontStyle FontStyle { get; init; } = FontStyle.Regular;` and include in `Equals`/`GetHashCode`/`Create()`.

- [ ] **i26** `PolygonShape.Points` null check is unreachable with NRT enabled  
  `Points` is typed `PointF[]` (not nullable). Remove the null guard; also replace `new PointF[0]` default with `Array.Empty<PointF>()` or `[]`.

- [ ] **i27** `DrawingSpec.ToString()` and `ROIWithGrapplesShape.ToString()` both return `""`  
  Not useful for debugging or property-grid display. Return something meaningful, e.g. `$"Visible={Visible}, Mode={MappingMode}"`.

- [ ] **i28** `ISingleROIDescriptor.Locked` XML doc says "should be editable" — should be "should NOT be editable"

- [ ] **i29** Empty `BitmapDisplayPanel_KeyPress` handler in `SingleROIManager`  
  The method body is empty. Remove the event subscription and the method.

- [ ] **i30** Unused imports in `ImageWrapper.cs` and `BitmapImageSource.cs`  
  `System.Collections.Generic`, `System.Linq`, `System.Media`, `System.Threading.Tasks`.

- [ ] **i31** `ZoomManager`: `Math.Abs(change)` is redundant  
  The sign of `change` is already resolved by the surrounding `if/else if` branches. Remove `Math.Abs`.

- [ ] **i32** `OnPaintBackground` optimisation doesn't fire for sub-pixel-positioned images  
  The `e.ClipRectangle != clippedDrawingRect` comparison converts the integer clip rect to `RectangleF`. When the paint rect has fractional coordinates (free-mode pan), the comparison is always `true` and the optimisation is dead. Use `Rectangle.Truncate(virtualDisplay.PaintRect)` for the comparison.

- [ ] **i33** Update `Win32.cs` to use `[LibraryImport]` source generation  
  `[DllImport("user32.dll")]` is the legacy approach. `[LibraryImport("user32.dll")]` with `partial static` generates more efficient P/Invoke code on .NET 7+.
