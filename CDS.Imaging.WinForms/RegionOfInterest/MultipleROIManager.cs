using CDS.Imaging.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.RegionOfInterest
{
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
        private bool visible = true;
        private BitmapDisplay.BitmapDisplayPanel? bitmapDisplayPanel;
        private ISingleROIDescriptor? activeROIDescriptor;
        private bool refreshSelectionSentry = false;


        /// <summary>
        /// Fired when the committed ROI changes.
        /// </summary>
        public event EventHandler<CommittedROIDescriptorChangedEventArgs>? OnCommittedROIDescriptorChanged;



        /// <summary>
        /// Callback to get the ROIs.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IEnumerable<ISingleROIDescriptor>>? GetROIDescriptors { get; set; } = () => [];



        /// <summary>
        /// Controls whether the ROI is visible.
        /// </summary>
        public bool Visible
        {
            get => visible;

            set
            {
                if (visible != value)
                {
                    visible = value;
                    bitmapDisplayPanel?.Invalidate();
                }
            }
        }


        /// <summary>
        /// The bitmap display panel that the ROI is drawn on.
        /// </summary>
        [Category(categoryCDS)]
        public BitmapDisplay.BitmapDisplayPanel? BitmapDisplayPanel
        {
            get => bitmapDisplayPanel;

            set
            {
                if (bitmapDisplayPanel != value)
                {
                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.Click -= BitmapDisplayPanel_Click;
                        bitmapDisplayPanel.OnPaintOver -= BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged -= BitmapDisplayPanel_OnImageSizeChanged;
                        bitmapDisplayPanel.KeyPress -= BitmapDisplayPanel_KeyPress;
                    }

                    bitmapDisplayPanel = value;

                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.Click += BitmapDisplayPanel_Click;
                        bitmapDisplayPanel.OnPaintOver += BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                        bitmapDisplayPanel.KeyPress += BitmapDisplayPanel_KeyPress;
                    }

                    roiSelectionOnBitmapDisplay.BitmapDisplayPanel = bitmapDisplayPanel;

                }
            }
        }

        private void BitmapDisplayPanel_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (activeROIDescriptor == null) { return; }

            const int escapeKeyCode = 27;

            if ((e.KeyChar == '\r') || (e.KeyChar == escapeKeyCode))
            {
                DeselectActiveROI();
            }
        }



        /// <summary>
        /// The renderer for the dragging ROI.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(categoryCDS)]
        [DisplayName("Dragging ROI renderer")]
        public ROIWithGrapplesShape DraggingROIRenderer
        {
            get => roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer;
        }


        /// <summary>
        /// The renderer for the committed ROI.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(categoryCDS)]
        [DisplayName("Committed ROI renderer")]
        public ROIWithGrapplesShape CommittedROIRenderer
        {
            get => roiSelectionOnBitmapDisplay.CommittedROIRenderer;
        }




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
        }



        /// <summary>
        /// True if there's an image that the ROI can be applied to.
        /// </summary>
        private bool DoesHaveImageToWorkWith => (bitmapDisplayPanel != null) && imageSize.HasValue && (GetROIDescriptors != null);


        /// <summary>
        /// Draws the current ROI.
        /// </summary>
        private void BitmapDisplayPanel_OnPaintOver(BitmapDisplay.BitmapDisplayPanel sender, Graphics graphics)
        {
            if (!DoesHaveImageToWorkWith) { return; }
            if (!visible) { return; }

            foreach (var roiDescriptor in GetROIDescriptors!())
            {
                roiDescriptor.Draw(bitmapDisplayPanel!, graphics);
            }
        }


        /// <summary>
        /// Handle a change to the image size
        /// </summary>
        private void BitmapDisplayPanel_OnImageSizeChanged(BitmapDisplay.BitmapDisplayPanel sender, Size oldSize, Size newSize)
        {
            imageSize = newSize;
        }


        private void BitmapDisplayPanel_Click(object? sender, EventArgs e)
        {
            HandleMouseLButtonDown();
        }


        private void HandleMouseLButtonDown()
        {
            if (!DoesHaveImageToWorkWith) { return; }
            if (IsSpacebarPressed()) { return; }

            var roiDescriptors = GetROIDescriptors!();
            var mouseLocationOnThisControl = bitmapDisplayPanel!.PointToClient(Cursor.Position);
            var mouseLocationOnImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnThisControl));

            bool didHandleClick = false;
            foreach (var roiDescriptor in roiDescriptors)
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
            if (roiDescriptor == activeROIDescriptor) { return; }
            if (roiSelectionOnBitmapDisplay.IsDragging) { return; }
            if (roiDescriptor.Locked) { return; }

            DeselectActiveROI();

            roiDescriptor.Visible = false;
            activeROIDescriptor = roiDescriptor;

            roiSelectionOnBitmapDisplay.CommittedROI = activeROIDescriptor.ROI;
            roiSelectionOnBitmapDisplay.Visible = true;
            roiSelectionOnBitmapDisplay.CanEditCommitted = true;

            bitmapDisplayPanel!.Invalidate();
        }


        private void DeselectActiveROI()
        {
            if (activeROIDescriptor == null) { return; }

            activeROIDescriptor.Visible = true;
            activeROIDescriptor = null;

            roiSelectionOnBitmapDisplay.Visible = false;
            roiSelectionOnBitmapDisplay.CanEditCommitted = false;

            bitmapDisplayPanel!.Invalidate();
        }


        private void roiSelectionOnBitmapDisplay_OnCommittedROIChanged(SingleROIManager sender, Rectangle roi)
        {
            var newROI = new Rectangle(
                x: roi.Location.X,
                y: roi.Location.Y,
                width: Math.Min(Math.Max(roi.Width, activeROIDescriptor!.MinimumSize.Width), activeROIDescriptor.MaximumSize.Width),
                height: Math.Min(Math.Max(roi.Height, activeROIDescriptor.MinimumSize.Height), activeROIDescriptor.MaximumSize.Height));

            if (activeROIDescriptor.ROI == roi) { return; }

            roiSelectionOnBitmapDisplay.CommittedROI = newROI;
            activeROIDescriptor.ROI = newROI;

            OnCommittedROIDescriptorChanged?.Invoke(this, new CommittedROIDescriptorChangedEventArgs(activeROIDescriptor));

            timerAutoDeselectActiveROI.Stop();
            timerAutoDeselectActiveROI.Start();

            System.Diagnostics.Debug.WriteLine("Started");
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
            if (refreshSelectionSentry) { return; }
            refreshSelectionSentry = true;

            if ((activeROIDescriptor != null) && (!activeROIDescriptor.Visible || activeROIDescriptor.Locked))
            {
                DeselectActiveROI();
            }

            bitmapDisplayPanel?.Invalidate();

            refreshSelectionSentry = false;
        }


        /// <summary>
        /// True if the spacebar is pressed.
        /// </summary>
        private bool IsSpacebarPressed()
        {
            return (Win32.GetKeyState(Win32.VK_SPACE) & 0x8000) != 0;
        }


        /// <summary>
        /// Automatically deselects the active ROI after a period of inactivity.
        /// </summary>
        private void timerAutoDeselectActiveROI_Tick(object sender, EventArgs e)
        {
            timerAutoDeselectActiveROI.Stop();

            if (activeROIDescriptor == null) { return; }

            DeselectActiveROI();
        }


        /// <summary>
        /// The ROI selection is being dragged.
        /// </summary>
        private void roiSelectionOnBitmapDisplay_OnDraggingROIChanged(SingleROIManager sender, Rectangle roi)
        {
            if(roi.IsEmpty) { return; }

            timerAutoDeselectActiveROI.Stop();

            System.Diagnostics.Debug.WriteLine("Stopping from drag");
        }
    }
}
