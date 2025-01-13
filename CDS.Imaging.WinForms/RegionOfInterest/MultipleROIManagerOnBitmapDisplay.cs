using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    public partial class MultipleROIManagerOnBitmapDisplay : Component
    {
        public class ROIDescriptor : IDisposable
        {
            public override string ToString() => Name;

            public string Name { get; set; } = "";

            public Rectangle ROI { get; set; }

            public Size MinimumSize { get; set; } = new Size(1, 1);

            public Size MaximumSize { get; set; } = new Size(1000000, 1000000);

            public RectangleRenderer Renderer { get; } = new RectangleRenderer()
            {
                GrapplesMode = RectangleRenderer.GrapplesRenderingMode.Hide,
            };

            public void Dispose()
            {
                Renderer.Dispose();
            }
        }


        private const string categoryCDS = "CDS";

        private Size? imageSize;
        private bool visible = true;
        private BitmapDisplay.BitmapDisplayPanel? bitmapDisplayPanel;
        private ROIDescriptor? activeROIDescriptor;


        /// <summary>
        /// Callback to get the ROIs.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IEnumerable<ROIDescriptor>>? GetROIDescriptors { get; set; } = () => [];



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
                    }

                    bitmapDisplayPanel = value;

                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.Click += BitmapDisplayPanel_Click;
                        bitmapDisplayPanel.OnPaintOver += BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                    }

                    roiSelectionOnBitmapDisplay.BitmapDisplayPanel = bitmapDisplayPanel;
                }
            }
        }





        /// <summary>
        /// Constructor
        /// </summary>
        public MultipleROIManagerOnBitmapDisplay()
        {
            InitializeComponent();
            CommonInitialise();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MultipleROIManagerOnBitmapDisplay(IContainer container)
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
                var displayRect = bitmapDisplayPanel!.MapImageRectangleToDisplayRectangle(roiDescriptor.ROI);
                roiDescriptor.Renderer.Draw(graphics, displayRect);
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
            if (!DoesHaveImageToWorkWith) { return; }

            var roiDescriptors = GetROIDescriptors!();
            var mouseLocationOnThisControl = bitmapDisplayPanel!.PointToClient(Cursor.Position);
            var mouseLocationOnImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnThisControl);

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

        private void HandleROIClicked(ROIDescriptor roiDescriptor)
        {
            if(roiDescriptor == activeROIDescriptor) { return; }
            if(roiSelectionOnBitmapDisplay.IsDragging) { return; }

            DeselectActiveROI();

            activeROIDescriptor = roiDescriptor;
            activeROIDescriptor.Renderer.Visible = false;

            roiSelectionOnBitmapDisplay.CommittedROI = activeROIDescriptor.ROI;
            roiSelectionOnBitmapDisplay.Visible = true;
            roiSelectionOnBitmapDisplay.CanEditCommitted = true;

            bitmapDisplayPanel!.Invalidate();
        }


        private void DeselectActiveROI()
        {
            if (activeROIDescriptor == null) { return; }

            activeROIDescriptor.Renderer.Visible = true;
            activeROIDescriptor = null;
            roiSelectionOnBitmapDisplay.Visible = false;
            roiSelectionOnBitmapDisplay.CanEditCommitted = false;

            bitmapDisplayPanel!.Invalidate();
        }


        private void roiSelectionOnBitmapDisplay_OnCommittedROIChanged(ROISelectionOnBitmapDisplay sender, Rectangle roi)
        {
            var newROI = new Rectangle(
                x: roi.Location.X,
                y: roi.Location.Y,
                width: Math.Min(Math.Max(roi.Width, activeROIDescriptor!.MinimumSize.Width), activeROIDescriptor.MaximumSize.Width),
                height: Math.Min(Math.Max(roi.Height, activeROIDescriptor.MinimumSize.Height), activeROIDescriptor.MaximumSize.Height));

            if(activeROIDescriptor.ROI == roi) { return; }

            roiSelectionOnBitmapDisplay.CommittedROI = newROI;
            activeROIDescriptor.ROI = newROI;
        }
    }
}
