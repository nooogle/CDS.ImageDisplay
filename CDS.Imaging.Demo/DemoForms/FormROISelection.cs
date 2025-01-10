using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms;

public partial class FormROISelection : Form
{
    public FormROISelection()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.CDSSetImage(Properties.Resources.Thailand);
        UpdateROILabels();


        propertyGrid1.SelectedObject = bitmapDisplayPanel;
    }

    private void UpdateROILabels()
    {
        labelCommittedROI.Text = $"Committed ROI: {bitmapDisplayPanel.CommittedROI}";
        labelDraggingROI.Text = $"Dragging ROI: {bitmapDisplayPanel.DraggingROI}";
    }

    private void FormROISelection_Load(object sender, EventArgs e)
    {

    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.CDSFitToWindowCentred();
    }

    private void btnSetROI_Click(object sender, EventArgs e)
    {
        bitmapDisplayPanel.CommittedROI = new Rectangle(10, 20, 100, 200);
    }

    private void btnClearROI_Click(object sender, EventArgs e)
    {
        bitmapDisplayPanel.CommittedROI = Rectangle.Empty;
    }

    private void bitmapDisplayPanel_CDSOnCommittedROIChanged(object sender, EventArgs e)
    {
        UpdateROILabels();
    }

    private void bitmapDisplayPanel_CDSOnDraggingROIChanged(object sender, EventArgs e)
    {
        UpdateROILabels();
    }
}
