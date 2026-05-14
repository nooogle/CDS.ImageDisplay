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
    private const string categoryCDS = "CDS";

    private Size? imageSize;
    private ISingleROIDescriptor? activeROIDescriptor;
    private bool refreshSelectionSentry = false;
    private Timer timerDeselectAfterClick = null!;
    private Timer timerDeselectAfterMove = null!;


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
    [Category(categoryCDS)]
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
                    imageSize = existingSize == Size.Empty ? null : existingSize;
                }
                else
                {
                    imageSize = null;
                }

                roiSelectionOnBitmapDisplay.BitmapDisplayPanel = field;

            }
        }
    }

    private void BitmapDisplayPanel_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (activeROIDescriptor == null)
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
    [Category(categoryCDS)]
    [DisplayName("Dragging ROI shape")]
    public ROIWithGrapplesShape DraggingROIShape => roiSelectionOnBitmapDisplay.LiveDraggingROIShape;


    /// <summary>
    /// The shape for the committed ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category(categoryCDS)]
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
        timerDeselectAfterClick = new Timer(components);
        timerDeselectAfterClick.Tick += (_, _) => { timerDeselectAfterClick.Stop(); DeselectActiveROI(); };

        timerDeselectAfterMove = new Timer(components);
        timerDeselectAfterMove.Tick += (_, _) => { timerDeselectAfterMove.Stop(); DeselectActiveROI(); };
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
    private bool DoesHaveImageToWorkWith => (BitmapDisplayPanel != null) && imageSize.HasValue;


    /// <summary>
    /// Draws the current ROI.
    /// </summary>
    private void BitmapDisplayPanel_OnPaintOver(BitmapDisplay.BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!DoesHaveImageToWorkWith)
        { return; }
        if (!Visible)
        { return; }

        foreach (ISingleROIDescriptor roiDescriptor in ROIDescriptors)
        {
            roiDescriptor.Draw(BitmapDisplayPanel!, graphics);
        }
    }


    /// <summary>
    /// Handle a change to the image size
    /// </summary>
    private void BitmapDisplayPanel_OnImageSizeChanged(BitmapDisplay.BitmapDisplayPanel sender, Size oldSize, Size newSize) => imageSize = newSize;


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
        if (roiDescriptor == activeROIDescriptor)
        { return; }
        if (roiSelectionOnBitmapDisplay.IsDragging)
        { return; }
        if (roiDescriptor.Locked)
        { return; }

        DeselectActiveROI();

        roiDescriptor.Visible = false;
        activeROIDescriptor = roiDescriptor;

        roiSelectionOnBitmapDisplay.CommittedROI = activeROIDescriptor.ROI;
        roiSelectionOnBitmapDisplay.Visible = true;
        roiSelectionOnBitmapDisplay.CanEditCommitted = true;

        BitmapDisplayPanel!.Invalidate();

        timerDeselectAfterMove.Stop();
        StartTimer(timerDeselectAfterClick, ClickDeselectDelay);
    }


    private void DeselectActiveROI()
    {
        if (activeROIDescriptor == null)
        { return; }

        timerDeselectAfterClick.Stop();
        timerDeselectAfterMove.Stop();

        activeROIDescriptor.Visible = true;
        activeROIDescriptor = null;

        roiSelectionOnBitmapDisplay.Visible = false;
        roiSelectionOnBitmapDisplay.CanEditCommitted = false;

        BitmapDisplayPanel!.Invalidate();
    }


    private void roiSelectionOnBitmapDisplay_OnCommittedROIChanged(SingleROIManager sender, Rectangle roi)
    {
        var newROI = new Rectangle(
            x: roi.Location.X,
            y: roi.Location.Y,
            width: Math.Min(Math.Max(roi.Width, activeROIDescriptor!.MinimumSize.Width), activeROIDescriptor.MaximumSize.Width),
            height: Math.Min(Math.Max(roi.Height, activeROIDescriptor.MinimumSize.Height), activeROIDescriptor.MaximumSize.Height));

        if (activeROIDescriptor.ROI == roi)
        { return; }

        roiSelectionOnBitmapDisplay.CommittedROI = newROI;
        activeROIDescriptor.ROI = newROI;

        OnCommittedROIDescriptorChanged?.Invoke(this, new CommittedROIDescriptorChangedEventArgs(activeROIDescriptor));

        timerDeselectAfterClick.Stop();
        StartTimer(timerDeselectAfterMove, MoveDeselectDelay);
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
        if (refreshSelectionSentry)
        { return; }
        refreshSelectionSentry = true;

        if ((activeROIDescriptor != null) && (!activeROIDescriptor.Visible || activeROIDescriptor.Locked))
        {
            DeselectActiveROI();
        }

        BitmapDisplayPanel?.Invalidate();

        refreshSelectionSentry = false;
    }


    /// <summary>
    /// True if the spacebar is pressed.
    /// </summary>
    private bool IsSpacebarPressed() => (Win32.GetKeyState(Win32.VK_SPACE) & 0x8000) != 0;


    /// <summary>
    /// The ROI selection is being dragged.
    /// </summary>
    private void roiSelectionOnBitmapDisplay_OnDraggingROIChanged(SingleROIManager sender, Rectangle roi)
    {
        if (roi.IsEmpty)
        { return; }

        timerDeselectAfterClick.Stop();
        timerDeselectAfterMove.Stop();
    }
}
