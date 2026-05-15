using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.RegionOfInterest;

/// <summary>
/// Manages multiple ROIs on a bitmap display.
/// </summary>
/// <remarks>
/// Each region of interest (ROI) is represented by an <see cref="ISingleROIDescriptor"/> object.
/// A default class, <see cref="ROIWithGrapplesShape"/>, is provided that implements this interface.
/// Otherwise, you can create your own class that implements the interface, and optionally delegate
/// most of the properties and methods to an instance of <see cref="ROIWithGrapplesShape"/> (using
/// the composition pattern).
/// </remarks>
public partial class MultipleROIManager : Component
{
    private const string s_categoryCDS = "CDS";

    private ISingleROIDescriptor? _activeROIDescriptor;
    private Size? _imageSize;
    private bool _refreshSelectionSentry;
    private Timer _timerDeselectAfterClick = null!;
    private Timer _timerDeselectAfterMove = null!;


    /// <summary>
    /// Fired when the committed ROI changes.
    /// </summary>
    public event EventHandler<CommittedROIDescriptorChangedEventArgs>? OnCommittedROIDescriptorChanged;



    /// <summary>
    /// The ROI descriptors to display and interact with.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<ISingleROIDescriptor> ROIDescriptors { get; set; } = [];


    /// <summary>
    /// How long after a ROI is clicked (without being moved) before it is automatically
    /// deselected. <see langword="null"/> disables auto-deselection for click-only selections.
    /// </summary>
    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan? ClickDeselectDelay { get; set; } = null;


    /// <summary>
    /// How long after a ROI is committed (moved or resized) before it is automatically
    /// deselected. <see langword="null"/> disables auto-deselection after a move.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan? MoveDeselectDelay { get; set; } = TimeSpan.FromSeconds(2);



    /// <summary>
    /// Controls whether the ROI is visible.
    /// </summary>
    [DefaultValue(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool Visible
    {
        get;

        set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;


    /// <summary>
    /// The bitmap display panel that the ROI is drawn on.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public BitmapDisplay.BitmapDisplayPanel? BitmapDisplayPanel
    {
        get;

        set
        {
            if (field != value)
            {
                if (field != null)
                {
                    field.MouseClick -= BitmapDisplayPanel_MouseClick;
                    field.OnPaintOver -= BitmapDisplayPanel_OnPaintOver;
                    field.OnImageSizeChanged -= BitmapDisplayPanel_OnImageSizeChanged;
                    field.KeyPress -= BitmapDisplayPanel_KeyPress;
                }

                field = value;

                if (field != null)
                {
                    field.MouseClick += BitmapDisplayPanel_MouseClick;
                    field.OnPaintOver += BitmapDisplayPanel_OnPaintOver;
                    field.OnImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                    field.KeyPress += BitmapDisplayPanel_KeyPress;

                    // Seed imageSize from the panel's current image so that ROIs can
                    // be used immediately, without waiting for the next OnImageSizeChanged event.
                    Size existingSize = field.ImageSize;
                    _imageSize = existingSize == Size.Empty ? null : existingSize;
                }
                else
                {
                    _imageSize = null;
                }

                roiSelectionOnBitmapDisplay.BitmapDisplayPanel = field;

            }
        }
    }

    private void BitmapDisplayPanel_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (_activeROIDescriptor == null)
        { return; }

        const int escapeKeyCode = 27;

        if (e.KeyChar is '\r' or (char)escapeKeyCode)
        {
            DeselectActiveROI();
        }
    }



    /// <summary>
    /// The shape for the dragging ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category(s_categoryCDS)]
    [DisplayName("Dragging ROI shape")]
    public ROIWithGrapplesShape DraggingROIShape => roiSelectionOnBitmapDisplay.LiveDraggingROIShape;


    /// <summary>
    /// The shape for the committed ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category(s_categoryCDS)]
    [DisplayName("Committed ROI shape")]
    public ROIWithGrapplesShape CommittedROIShape => roiSelectionOnBitmapDisplay.CommittedROIShape;




    /// <summary>
    /// Constructor
    /// </summary>
    public MultipleROIManager()
    {
        InitializeComponent();
        CommonInitialise();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public MultipleROIManager(IContainer container)
    {
        container.Add(this);
        InitializeComponent();
        CommonInitialise();
    }


    /// <summary>
    /// Common initialisation
    /// </summary>
    private void CommonInitialise()
    {
        _timerDeselectAfterClick = new Timer(components);
        _timerDeselectAfterClick.Tick += (_, _) => { _timerDeselectAfterClick.Stop(); DeselectActiveROI(); };

        _timerDeselectAfterMove = new Timer(components);
        _timerDeselectAfterMove.Tick += (_, _) => { _timerDeselectAfterMove.Stop(); DeselectActiveROI(); };
    }


    /// <summary>
    /// Starts <paramref name="timer"/> after configuring its interval from <paramref name="delay"/>.
    /// Does nothing if <paramref name="delay"/> is <see langword="null"/>.
    /// </summary>
    private static void StartTimer(Timer timer, TimeSpan? delay)
    {
        timer.Stop();
        if (delay is null)
        { return; }
        timer.Interval = Math.Max(1, (int)delay.Value.TotalMilliseconds);
        timer.Start();
    }



    /// <summary>
    /// True if there's an image that the ROI can be applied to.
    /// </summary>
    private bool DoesHaveImageToWorkWith => (BitmapDisplayPanel != null) && _imageSize.HasValue;


    /// <summary>
    /// Draws the current ROI.
    /// </summary>
    private void BitmapDisplayPanel_OnPaintOver(object? sender, CDS.ImageDisplay.BitmapDisplay.PaintOverEventArgs e)
    {
        if (!DoesHaveImageToWorkWith)
        { return; }
        if (!Visible)
        { return; }

        foreach (ISingleROIDescriptor roiDescriptor in ROIDescriptors)
        {
            roiDescriptor.Draw(BitmapDisplayPanel!, e.Graphics);
        }
    }


    /// <summary>
    /// Handle a change to the image size
    /// </summary>
    private void BitmapDisplayPanel_OnImageSizeChanged(object? sender, BitmapDisplay.ImageSizeChangedEventArgs e)
    {
        _imageSize = e.NewSize;
    }

    private void BitmapDisplayPanel_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
        { return; }
        HandleMouseLButtonDown(e.Location);
    }


    private void HandleMouseLButtonDown(Point mouseLocationOnThisControl)
    {
        if (!DoesHaveImageToWorkWith)
        { return; }
        if (IsSpacebarPressed())
        { return; }

        var mouseLocationOnImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnThisControl));

        bool didHandleClick = false;
        foreach (ISingleROIDescriptor roiDescriptor in ROIDescriptors)
        {
            if (roiDescriptor.ROI.Contains(mouseLocationOnImage))
            {
                HandleROIClicked(roiDescriptor);
                didHandleClick = true;
                break;
            }
        }

        if (!didHandleClick && !roiSelectionOnBitmapDisplay.IsDragging)
        {
            DeselectActiveROI();
        }
    }


    private void HandleROIClicked(ISingleROIDescriptor roiDescriptor)
    {
        if (roiDescriptor == _activeROIDescriptor)
        { return; }
        if (roiSelectionOnBitmapDisplay.IsDragging)
        { return; }
        if (roiDescriptor.Locked)
        { return; }

        DeselectActiveROI();

        roiDescriptor.Visible = false;
        _activeROIDescriptor = roiDescriptor;

        roiSelectionOnBitmapDisplay.CommittedROI = _activeROIDescriptor.ROI;
        roiSelectionOnBitmapDisplay.Visible = true;
        roiSelectionOnBitmapDisplay.CanEditCommitted = true;

        BitmapDisplayPanel!.Invalidate();

        _timerDeselectAfterMove.Stop();
        StartTimer(_timerDeselectAfterClick, ClickDeselectDelay);
    }


    private void DeselectActiveROI()
    {
        if (_activeROIDescriptor == null)
        { return; }

        _timerDeselectAfterClick.Stop();
        _timerDeselectAfterMove.Stop();

        _activeROIDescriptor.Visible = true;
        _activeROIDescriptor = null;

        roiSelectionOnBitmapDisplay.Visible = false;
        roiSelectionOnBitmapDisplay.CanEditCommitted = false;

        BitmapDisplayPanel!.Invalidate();
    }


    private void roiSelectionOnBitmapDisplay_OnCommittedROIChanged(object? sender, CommittedROIChangedEventArgs e)
    {
        var roi = e.ROI;

        var newROI = new Rectangle(
            x: roi.Location.X,
            y: roi.Location.Y,
            width: Math.Min(Math.Max(roi.Width, _activeROIDescriptor!.MinimumSize.Width), _activeROIDescriptor.MaximumSize.Width),
            height: Math.Min(Math.Max(roi.Height, _activeROIDescriptor.MinimumSize.Height), _activeROIDescriptor.MaximumSize.Height));

        if (_activeROIDescriptor.ROI == roi)
        { return; }

        roiSelectionOnBitmapDisplay.CommittedROI = newROI;
        _activeROIDescriptor.ROI = newROI;

        OnCommittedROIDescriptorChanged?.Invoke(this, new CommittedROIDescriptorChangedEventArgs(_activeROIDescriptor));

        _timerDeselectAfterClick.Stop();
        StartTimer(_timerDeselectAfterMove, MoveDeselectDelay);
    }


    /// <summary>
    /// Refreshes the selected region of interest.
    /// </summary>
    /// <remarks>
    /// This is useful if a ROI descriptor property of the currently selected ROI has been changed 
    /// which results in the ROI no longer being editable or visible.
    /// </remarks>
    public void RefreshSelection()
    {
        if (_refreshSelectionSentry)
        { return; }
        _refreshSelectionSentry = true;

        if ((_activeROIDescriptor != null) && (!_activeROIDescriptor.Visible || _activeROIDescriptor.Locked))
        {
            DeselectActiveROI();
        }

        BitmapDisplayPanel?.Invalidate();

        _refreshSelectionSentry = false;
    }


    /// <summary>
    /// True if the spacebar is pressed.
    /// </summary>
    private static bool IsSpacebarPressed() => (Win32.GetKeyState(Win32.VK_SPACE) & 0x8000) != 0;


    /// <summary>
    /// The ROI selection is being dragged.
    /// </summary>
    private void roiSelectionOnBitmapDisplay_OnDraggingROIChanged(object? sender, DraggingROIChangedEventArgs e)
    {
        var roi = e.ROI;
        if (roi.IsEmpty)
        {
            return;
        }

        _timerDeselectAfterClick.Stop();
        _timerDeselectAfterMove.Stop();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timerDeselectAfterClick?.Dispose();
            _timerDeselectAfterMove?.Dispose();

            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
