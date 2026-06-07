using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDS.ImageDisplay.Annotations.Internal;
using CDS.ImageDisplay.Annotations.Shapes;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Annotations;

/// <summary>
/// Component that manages interactive creation, selection, and editing of annotations
/// on a <see cref="BitmapDisplayPanel"/>.
/// </summary>
public partial class AnnotationManager : Component
{
    private const string s_categoryCDS = "CDS";

    // -----------------------------------------------------------------------
    // State
    // -----------------------------------------------------------------------

    private AnnotationInteractionState _state = AnnotationInteractionState.Idle;
    private readonly List<Annotation> _annotations = [];
    private readonly List<IAnnotationShapeDescriptor> _recognitionDescriptors = [];
    private readonly CrosshairAnnotationDescriptor _crosshairDescriptor = new();
    private readonly FreehandPathOverlay _pathOverlay = new();
    private Annotation? _selectedAnnotation;
    private AnnotationHitInfo _activeHit = AnnotationHitInfo.Miss;
    private AnnotationGeometry? _preDragSnapshot;
    private Point _lastDragDisplay;

    // -----------------------------------------------------------------------
    // Properties
    // -----------------------------------------------------------------------

    /// <summary>
    /// The panel this manager is attached to. Assign to subscribe; assign <see langword="null"/> to detach.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(null)]
    public BitmapDisplayPanel? BitmapDisplayPanel
    {
        get;
        set
        {
            if (field == value) { return; }

            if (field != null)
            {
                field.MouseDown -= BitmapDisplayPanel_MouseDown;
                field.MouseMove -= BitmapDisplayPanel_MouseMove;
                field.MouseUp -= BitmapDisplayPanel_MouseUp;
                field.PaintOver -= BitmapDisplayPanel_OnPaintOver;
                field.KeyDown -= BitmapDisplayPanel_KeyDown;
            }

            field = value;

            if (field != null)
            {
                field.MouseDown += BitmapDisplayPanel_MouseDown;
                field.MouseMove += BitmapDisplayPanel_MouseMove;
                field.MouseUp += BitmapDisplayPanel_MouseUp;
                field.PaintOver += BitmapDisplayPanel_OnPaintOver;
                field.KeyDown += BitmapDisplayPanel_KeyDown;
            }

            ResetToIdle();
        }
    }

    /// <summary>Whether new annotations can be created. Default <see langword="true"/>.</summary>
    [Category(s_categoryCDS)]
    [DefaultValue(true)]
    public bool CanCreate { get; set; } = true;

    /// <summary>Whether existing annotations can be edited or deleted. Default <see langword="true"/>.</summary>
    [Category(s_categoryCDS)]
    [DefaultValue(true)]
    public bool CanEdit { get; set; } = true;

    /// <summary>
    /// When <see langword="true"/>, each annotation's <see cref="Annotation.Label"/> (or
    /// <see cref="Annotation.Title"/> when label is empty) is drawn as a small tag near the
    /// top-left of the shape's bounding box. Default <see langword="true"/>.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(true)]
    public bool ShowLabels { get; set; } = true;

    /// <summary>Hit-test tolerance in display pixels. Default 8.</summary>
    [Category(s_categoryCDS)]
    [DefaultValue(8)]
    public int DragBorder
    {
        get;
        set => field = Math.Max(0, Math.Min(value, 100));
    } = 8;

    /// <summary>
    /// Minimum bounding-box dimension (in display pixels) for a gesture to be treated as a drag rather than a click.
    /// Default 5.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(5)]
    public int MinimumGestureSize
    {
        get;
        set => field = Math.Max(1, value);
    } = 5;

    /// <summary>The current list of annotations.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<Annotation> Annotations => _annotations.AsReadOnly();

    /// <summary>The currently selected annotation, or <see langword="null"/> if nothing is selected.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Annotation? SelectedAnnotation => _selectedAnnotation;

    // -----------------------------------------------------------------------
    // Events
    // -----------------------------------------------------------------------

    /// <summary>Fired when a new annotation is committed.</summary>
    public event EventHandler<AnnotationCreatedEventArgs>? AnnotationCreated;

    /// <summary>Fired when an annotation's geometry is modified by dragging or arrow keys.</summary>
    public event EventHandler<AnnotationModifiedEventArgs>? AnnotationModified;

    /// <summary>Fired when an annotation is removed.</summary>
    public event EventHandler<AnnotationDeletedEventArgs>? AnnotationDeleted;

    /// <summary>Fired when an annotation becomes selected.</summary>
    public event EventHandler<AnnotationSelectedEventArgs>? AnnotationSelected;

    /// <summary>Fired when the selected annotation is deselected.</summary>
    public event EventHandler? AnnotationDeselected;

    /// <summary>
    /// Fired when a drag operation is about to begin on a selected annotation.
    /// The event args carry a geometry snapshot that can be used as an undo checkpoint.
    /// </summary>
    public event EventHandler<AnnotationDragStartingEventArgs>? DragStarting;

    // -----------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------

    /// <summary>Initializes a new instance of <see cref="AnnotationManager"/>.</summary>
    public AnnotationManager()
    {
        InitializeComponent();
        RegisterBuiltInDescriptors();
    }

    /// <summary>Initializes a new instance of <see cref="AnnotationManager"/> and adds it to <paramref name="container"/>.</summary>
    public AnnotationManager(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));
        container.Add(this);
        InitializeComponent();
        RegisterBuiltInDescriptors();
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    /// <summary>Adds an annotation to the manager's list and repaints.</summary>
    public void AddAnnotation(Annotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation, nameof(annotation));
        _annotations.Add(annotation);
        BitmapDisplayPanel?.Invalidate();
    }

    /// <summary>Removes an annotation and fires <see cref="AnnotationDeleted"/>.</summary>
    public void RemoveAnnotation(Annotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation, nameof(annotation));
        if (!_annotations.Remove(annotation)) { return; }

        if (_selectedAnnotation == annotation) { ClearSelection(); }

        AnnotationDeleted?.Invoke(this, new AnnotationDeletedEventArgs(annotation));
        BitmapDisplayPanel?.Invalidate();
    }

    /// <summary>Removes all annotations and repaints without firing individual events.</summary>
    public void ClearAnnotations()
    {
        if (_annotations.Count == 0) { return; }
        if (_selectedAnnotation != null) { ClearSelection(); }
        _annotations.Clear();
        BitmapDisplayPanel?.Invalidate();
    }

    /// <summary>Adds a shape descriptor to the auto-recognition pool.</summary>
    public void RegisterShapeDescriptor(IAnnotationShapeDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor, nameof(descriptor));
        _recognitionDescriptors.Add(descriptor);
    }

    // -----------------------------------------------------------------------
    // Mouse handlers
    // -----------------------------------------------------------------------

    private void BitmapDisplayPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || IsSpacebarPressed()) { return; }

        BitmapDisplayPanel panel = BitmapDisplayPanel!;

        switch (_state)
        {
            case AnnotationInteractionState.Idle:
                HandleMouseDownIdle(e, panel);
                break;
            case AnnotationInteractionState.Selected:
                HandleMouseDownSelected(e, panel);
                break;
        }
    }

    private void HandleMouseDownIdle(MouseEventArgs e, BitmapDisplayPanel panel)
    {
        var (hitAnnotation, _) = HitTestAll(e.Location, panel);

        if (hitAnnotation != null)
        {
            SetSelected(hitAnnotation);
        }
        else if (CanCreate)
        {
            StartDrawing(e.Location, panel);
        }
    }

    private void HandleMouseDownSelected(MouseEventArgs e, BitmapDisplayPanel panel)
    {
        AnnotationHitInfo hit = _selectedAnnotation!.Geometry.HitTest(panel, e.Location, DragBorder);

        if (hit.Kind != AnnotationHitKind.None && CanEdit)
        {
            _activeHit = hit;
            _preDragSnapshot = _selectedAnnotation.Geometry.Clone();
            DragStarting?.Invoke(this, new AnnotationDragStartingEventArgs(_selectedAnnotation, _preDragSnapshot));
            _lastDragDisplay = e.Location;
            _state = AnnotationInteractionState.Dragging;
            panel.SuppressDragging();
            return;
        }

        var (otherAnnotation, _) = HitTestAll(e.Location, panel, exclude: _selectedAnnotation);

        if (otherAnnotation != null)
        {
            ClearSelection();
            SetSelected(otherAnnotation);
        }
        else
        {
            ClearSelection();
            _state = AnnotationInteractionState.Idle;
        }
    }

    private void BitmapDisplayPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        BitmapDisplayPanel panel = BitmapDisplayPanel!;

        switch (_state)
        {
            case AnnotationInteractionState.Drawing:
                _pathOverlay.AddPoint(panel.MapDisplayToImage(e.Location));
                panel.Invalidate();
                break;

            case AnnotationInteractionState.Dragging:
                HandleMouseMoveDragging(e, panel);
                break;

            case AnnotationInteractionState.Idle:
            case AnnotationInteractionState.Selected:
                UpdateCursor(e.Location, panel);
                break;
        }
    }

    private void HandleMouseMoveDragging(MouseEventArgs e, BitmapDisplayPanel panel)
    {
        var delta = new Point(e.X - _lastDragDisplay.X, e.Y - _lastDragDisplay.Y);
        if (delta.X == 0 && delta.Y == 0) { return; }

        Size imageDelta = AnnotationHandleHelper.DisplayDeltaToImageDelta(panel, delta);
        _selectedAnnotation!.Geometry.ApplyImageDelta(_activeHit, imageDelta);
        _lastDragDisplay = e.Location;
        panel.Invalidate();
    }

    private void BitmapDisplayPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) { return; }

        BitmapDisplayPanel panel = BitmapDisplayPanel!;

        switch (_state)
        {
            case AnnotationInteractionState.Drawing:
                HandleMouseUpDrawing(e, panel);
                break;

            case AnnotationInteractionState.Dragging:
                CommitDrag(panel);
                break;
        }
    }

    private void HandleMouseUpDrawing(MouseEventArgs e, BitmapDisplayPanel panel)
    {
        panel.UnsuppressDragging();
        panel.Cursor = Cursors.Default;

        FreehandPath path = _pathOverlay.ToFreehandPath();
        _pathOverlay.Clear();

        RectangleF bbox = path.BoundingBox;
        bool isClick = bbox.Width < MinimumGestureSize && bbox.Height < MinimumGestureSize;

        if (isClick)
        {
            CommitAnnotation(_crosshairDescriptor, path, panel);
            _state = AnnotationInteractionState.Idle;
            panel.Invalidate();
            return;
        }

        // Drag gesture — show the shape menu.
        _state = AnnotationInteractionState.MenuOpen;
        var ranked = AnnotationShapeRecognizer.Rank(path, _recognitionDescriptors);

        AnnotationShapeMenu.Show(panel, e.Location, ranked, _crosshairDescriptor, descriptor =>
        {
            if (descriptor != null) { CommitAnnotation(descriptor, path, panel); }
            _state = AnnotationInteractionState.Idle;
            panel.Invalidate();
        });
    }

    private void CommitDrag(BitmapDisplayPanel panel)
    {
        _preDragSnapshot = null;
        _state = AnnotationInteractionState.Selected;
        panel.UnsuppressDragging();
        AnnotationModified?.Invoke(this, new AnnotationModifiedEventArgs(_selectedAnnotation!));
        panel.Invalidate();
    }

    // -----------------------------------------------------------------------
    // Paint handler
    // -----------------------------------------------------------------------

    private void BitmapDisplayPanel_OnPaintOver(object? sender, PaintOverEventArgs e)
    {
        BitmapDisplayPanel panel = e.Sender;
        Graphics graphics = e.Graphics;
        GraphicsState graphicsState = graphics.Save();

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        try
        {
            foreach (Annotation annotation in _annotations)
            {
                bool isSelected = annotation == _selectedAnnotation
                    && _state is AnnotationInteractionState.Selected or AnnotationInteractionState.Dragging;
                annotation.Geometry.Draw(panel, graphics, isSelected);
                if (ShowLabels) { DrawAnnotationLabel(annotation, panel, graphics); }
            }

            if (_state == AnnotationInteractionState.Drawing)
            {
                _pathOverlay.Draw(graphics, panel);
            }
        }
        finally
        {
            graphics.Restore(graphicsState);
        }
    }

    private static readonly FontSpec s_labelFontSpec = new() { FontName = "Segoe UI", FontSize = 8 };
    private static readonly BrushSpec s_labelBackingSpec = new() { Color = Color.FromArgb(160, Color.Black) };
    private static readonly BrushSpec s_labelTextSpec = new() { Color = Color.White };

    private static void DrawAnnotationLabel(Annotation annotation, BitmapDisplayPanel panel, Graphics graphics)
    {
        string text = string.IsNullOrEmpty(annotation.Label) ? annotation.Title : annotation.Label;
        if (string.IsNullOrEmpty(text)) { return; }

        RectangleF bbox = annotation.Geometry.GetBoundingBox();
        if (bbox.IsEmpty) { return; }

        PointF displayTL = panel.MapImageToDisplay(new PointF(bbox.Left, bbox.Top), DisplayPixelAlign.TopLeft);

        Font font = DrawingToolsPool.GetFont(s_labelFontSpec);
        SizeF textSize = graphics.MeasureString(text, font);

        var backing = new RectangleF(displayTL.X, displayTL.Y - textSize.Height - 2f, textSize.Width + 4f, textSize.Height + 2f);

        Brush backBrush = DrawingToolsPool.GetBrush(s_labelBackingSpec);
        Brush textBrush = DrawingToolsPool.GetBrush(s_labelTextSpec);

        graphics.FillRectangle(backBrush, backing);
        graphics.DrawString(text, font, textBrush, backing.Left + 2f, backing.Top + 1f);
    }

    // -----------------------------------------------------------------------
    // Keyboard handler
    // -----------------------------------------------------------------------

    private void BitmapDisplayPanel_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (_state)
        {
            case AnnotationInteractionState.Drawing when e.KeyCode == Keys.Escape:
                CancelDrawing();
                break;

            case AnnotationInteractionState.Selected:
                HandleKeyDownSelected(e);
                break;

            case AnnotationInteractionState.Dragging when e.KeyCode == Keys.Escape:
                RevertDrag();
                break;
        }
    }

    private void HandleKeyDownSelected(KeyEventArgs e)
    {
        if (_selectedAnnotation == null) { return; }

        if (e.KeyCode is Keys.Delete or Keys.Back)
        {
            DeleteSelectedAnnotation();
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            ClearSelection();
            _state = AnnotationInteractionState.Idle;
            return;
        }

        int step = e.Control ? 10 : 1;
        Size imageDelta = e.KeyCode switch
        {
            Keys.Left => new Size(-step, 0),
            Keys.Right => new Size(step, 0),
            Keys.Up => new Size(0, -step),
            Keys.Down => new Size(0, step),
            _ => Size.Empty,
        };

        if (imageDelta == Size.Empty) { return; }

        _selectedAnnotation.Geometry.ApplyImageDelta(AnnotationHitInfo.Move, imageDelta);
        AnnotationModified?.Invoke(this, new AnnotationModifiedEventArgs(_selectedAnnotation));
        BitmapDisplayPanel?.Invalidate();
        e.Handled = true;
    }

    // -----------------------------------------------------------------------
    // State helpers
    // -----------------------------------------------------------------------

    private void StartDrawing(Point mouseDisplay, BitmapDisplayPanel panel)
    {
        _pathOverlay.Clear();
        _pathOverlay.AddPoint(panel.MapDisplayToImage(mouseDisplay));
        _state = AnnotationInteractionState.Drawing;
        panel.SuppressDragging();
        panel.Cursor = Cursors.Cross;
        panel.Invalidate();
    }

    private void CancelDrawing()
    {
        _pathOverlay.Clear();
        _state = AnnotationInteractionState.Idle;
        BitmapDisplayPanel?.UnsuppressDragging();
        BitmapDisplayPanel!.Cursor = Cursors.Default;
        BitmapDisplayPanel.Invalidate();
    }

    private void SetSelected(Annotation annotation)
    {
        _selectedAnnotation = annotation;
        _state = AnnotationInteractionState.Selected;
        AnnotationSelected?.Invoke(this, new AnnotationSelectedEventArgs(annotation));
        BitmapDisplayPanel?.Invalidate();
    }

    private void ClearSelection()
    {
        if (_selectedAnnotation == null) { return; }
        _selectedAnnotation = null;
        _state = AnnotationInteractionState.Idle;
        AnnotationDeselected?.Invoke(this, EventArgs.Empty);
        BitmapDisplayPanel?.Invalidate();
    }

    private void RevertDrag()
    {
        if (_preDragSnapshot != null && _selectedAnnotation != null)
        {
            _selectedAnnotation.Geometry = _preDragSnapshot;
        }

        _preDragSnapshot = null;
        BitmapDisplayPanel?.UnsuppressDragging();
        _state = AnnotationInteractionState.Selected;
        BitmapDisplayPanel?.Invalidate();
    }

    private void DeleteSelectedAnnotation()
    {
        if (_selectedAnnotation == null) { return; }

        Annotation annotation = _selectedAnnotation;
        ClearSelection();
        _annotations.Remove(annotation);
        _state = AnnotationInteractionState.Idle;
        AnnotationDeleted?.Invoke(this, new AnnotationDeletedEventArgs(annotation));
        BitmapDisplayPanel?.Invalidate();
    }

    private void CommitAnnotation(IAnnotationShapeDescriptor descriptor, FreehandPath path, BitmapDisplayPanel panel)
    {
        var annotation = new Annotation(descriptor.CreateGeometry(path));
        _annotations.Add(annotation);
        AnnotationCreated?.Invoke(this, new AnnotationCreatedEventArgs(annotation));
        panel.Invalidate();
    }

    private void ResetToIdle()
    {
        _pathOverlay.Clear();
        _preDragSnapshot = null;
        _selectedAnnotation = null;
        _activeHit = AnnotationHitInfo.Miss;
        _state = AnnotationInteractionState.Idle;
    }

    // -----------------------------------------------------------------------
    // Hit testing
    // -----------------------------------------------------------------------

    private (Annotation? Annotation, AnnotationHitInfo Hit) HitTestAll(
        Point displayPoint, BitmapDisplayPanel panel, Annotation? exclude = null)
    {
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            Annotation annotation = _annotations[i];
            if (annotation == exclude) { continue; }

            AnnotationHitInfo hit = annotation.Geometry.HitTest(panel, displayPoint, DragBorder);
            if (hit.Kind != AnnotationHitKind.None) { return (annotation, hit); }
        }

        return (null, AnnotationHitInfo.Miss);
    }

    // -----------------------------------------------------------------------
    // Cursor
    // -----------------------------------------------------------------------

    private void UpdateCursor(Point displayPoint, BitmapDisplayPanel panel)
    {
        if (_selectedAnnotation != null)
        {
            AnnotationHitInfo hit = _selectedAnnotation.Geometry.HitTest(panel, displayPoint, DragBorder);
            if (hit.Kind != AnnotationHitKind.None)
            {
                panel.Cursor = hit.Kind == AnnotationHitKind.MoveBody ? Cursors.SizeAll : Cursors.Cross;
                return;
            }
        }

        var (annotation, _) = HitTestAll(displayPoint, panel);
        panel.Cursor = annotation != null ? Cursors.SizeAll : Cursors.Default;
    }

    // -----------------------------------------------------------------------
    // Utilities
    // -----------------------------------------------------------------------

    private void RegisterBuiltInDescriptors()
    {
        _recognitionDescriptors.Add(new RectAnnotationDescriptor());
        _recognitionDescriptors.Add(new RotatedRectAnnotationDescriptor());
        _recognitionDescriptors.Add(new CircleAnnotationDescriptor());
        _recognitionDescriptors.Add(new EllipseAnnotationDescriptor());
        _recognitionDescriptors.Add(new LineAnnotationDescriptor());
        _recognitionDescriptors.Add(new PolygonAnnotationDescriptor());
    }

    private static bool IsSpacebarPressed() =>
        (Win32Imports.GetKeyState(Win32Imports.VK_SPACE) & 0x8000) != 0;
}
