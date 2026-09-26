using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;


/// <summary>
/// Demonstration of <see cref="Utils.FormStatePersister"/>: the form saves its position, size and window state
/// on close and restores them the next time it is opened.
/// </summary>
internal sealed partial class FormStatePersisterDemo : Form
{
    /// <summary>
    /// Initializes a new instance of the FormStatePersisterDemo class.
    /// </summary>
    public FormStatePersisterDemo()
    {
        InitializeComponent();
    }
}
