using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.Json;

namespace CDS.ImageDisplay.WinForms.Utils;

/// <summary>
/// A non-visual component that saves and restores a <see cref="System.Windows.Forms.Form"/>'s
/// size, position, and maximised state to/from a JSON file under
/// <see cref="Environment.SpecialFolder.LocalApplicationData"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Designer use:</b> Drop this component onto a form in the Visual Studio designer.
/// The <see cref="Form"/> property is set automatically; set <see cref="ContextId"/> in the
/// property grid if the same form class is used in more than one context.
/// </para>
/// <para>
/// <b>Code-behind use:</b> Construct with <see cref="FormStatePersister(Form, string?)"/>
/// and hold the instance for the form's lifetime; event wiring is handled automatically.
/// </para>
/// <para>
/// On restore the persister checks that at least 100 × 100 pixels of the restored window would
/// be visible on one of the available screens. If the saved position is off-screen (e.g. a
/// monitor has been disconnected) it keeps the saved size but centres the form on the primary
/// screen instead.
/// </para>
/// </remarks>
[DesignerCategory("Component")]
[ToolboxItem(true)]
public sealed class FormStatePersister : Component
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private const int MinVisiblePixels = 100;

    private Form? _form;
    private string? _contextId;

    // -------------------------------------------------------------------------
    // Properties
    // -------------------------------------------------------------------------

    /// <summary>
    /// Gets the path of the JSON file used to persist the form's state, or
    /// <see langword="null"/> when <see cref="Form"/> has not been assigned.
    /// </summary>
    [Browsable(false)]
    public string? FilePath => _form is null ? null : ResolvePath(_form.GetType().Name, _contextId);

    /// <summary>
    /// Gets or sets an optional identifier appended to the state file name, allowing
    /// the same form class to have separate persisted states for different usage contexts
    /// (e.g. <c>"main"</c>, <c>"details"</c>).
    /// </summary>
    [Category("Form State")]
    [DefaultValue(null)]
    [Description("Optional suffix appended to the state file name. Use when the same form class is used in more than one context.")]
    public string? ContextId
    {
        get => _contextId;
        set => _contextId = string.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>
    /// Gets or sets the form whose window state is persisted.
    /// Set automatically when the component is dropped onto a form in the Visual Studio designer.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Form? Form
    {
        get => _form;
        set
        {
            if (_form == value) { return; }

            if (_form != null)
            {
                _form.Load -= OnFormLoad;
                _form.FormClosing -= OnFormClosing;
            }

            _form = value;

            if (_form != null && !DesignMode)
            {
                _form.Load += OnFormLoad;
                _form.FormClosing += OnFormClosing;
            }
        }
    }

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    /// <summary>Initialises a new instance with no associated form.</summary>
    public FormStatePersister() { }

    /// <summary>
    /// Initialises a new instance and registers it with the given container.
    /// This is the constructor the Visual Studio designer uses; it generates
    /// <c>new FormStatePersister(this.components)</c> followed by
    /// <c>this.formStatePersister1.Form = this</c>.
    /// </summary>
    /// <param name="container">The designer container to add this component to.</param>
    public FormStatePersister(IContainer container) : this()
    {
        Guard.ThrowIfNull(container);
        container.Add(this);
    }

    /// <summary>
    /// Initialises a new instance and immediately associates it with the given form.
    /// Convenient for code-behind use when the Visual Studio designer is not involved.
    /// </summary>
    /// <param name="form">The form whose state will be saved and restored.</param>
    /// <param name="contextId">
    /// Optional identifier appended to the state file name so that the same form class can
    /// maintain separate states for different usage contexts.
    /// </param>
    public FormStatePersister(Form form, string? contextId = null) : this()
    {
        Guard.ThrowIfNull(form);
        _contextId = string.IsNullOrEmpty(contextId) ? null : contextId;
        Form = form;
    }

    // -------------------------------------------------------------------------
    // Designer integration
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Overridden to detect the host form via <see cref="IDesignerHost"/> at design time,
    /// so that the <see cref="Form"/> property is populated automatically when the component
    /// is first dropped onto a form in the Visual Studio designer.
    /// </remarks>
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            base.Site = value;

            if (value?.GetService(typeof(IDesignerHost)) is IDesignerHost host &&
                host.RootComponent is Form rootForm)
            {
                Form = rootForm;
            }
        }
    }

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing) { Form = null; }
        base.Dispose(disposing);
    }

    // -------------------------------------------------------------------------
    // Event handlers
    // -------------------------------------------------------------------------

    private void OnFormLoad(object? sender, EventArgs e) => Restore();

    private void OnFormClosing(object? sender, FormClosingEventArgs e) => Save();

    // -------------------------------------------------------------------------
    // Save / Restore
    // -------------------------------------------------------------------------

    private void Restore()
    {
        string filePath = FilePath!;
        if (!File.Exists(filePath)) { return; }

        try
        {
            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<FormStateData>(json, s_jsonOptions);
            if (data is null) { return; }

            Apply(data);
        }
        catch (Exception ex) when (ex is JsonException or IOException)
        {
            // Silently fall back to design-time defaults when the state file is corrupt or unreadable.
        }
    }

    private void Apply(FormStateData data)
    {
        int width = _form!.MinimumSize.Width > 0
            ? Math.Max(data.Width, _form.MinimumSize.Width)
            : Math.Max(data.Width, 50);
        int height = _form.MinimumSize.Height > 0
            ? Math.Max(data.Height, _form.MinimumSize.Height)
            : Math.Max(data.Height, 50);
        var bounds = new Rectangle(data.X, data.Y, width, height);

        if (IsOnScreen(bounds))
        {
            _form.StartPosition = FormStartPosition.Manual;
            _form.Bounds = bounds;
        }
        else
        {
            _form.Size = bounds.Size;
            _form.StartPosition = FormStartPosition.CenterScreen;
        }

        if (data.IsMaximized)
        {
            _form.WindowState = FormWindowState.Maximized;
        }
    }

    private void Save()
    {
        // Use RestoreBounds when maximised so the normal window size is preserved across sessions.
        var bounds = _form!.WindowState == FormWindowState.Normal
            ? _form.Bounds
            : _form.RestoreBounds;

        var data = new FormStateData
        {
            X = bounds.X,
            Y = bounds.Y,
            Width = bounds.Width,
            Height = bounds.Height,
            IsMaximized = _form.WindowState == FormWindowState.Maximized,
        };

        string filePath = FilePath!;

        try
        {
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) { Directory.CreateDirectory(dir); }

            File.WriteAllText(filePath, JsonSerializer.Serialize(data, s_jsonOptions));
        }
        catch (IOException)
        {
            // Saving state is best-effort; ignore I/O failures.
        }
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static bool IsOnScreen(Rectangle formRect)
    {
        return Screen.AllScreens.Any(s =>
        {
            var intersection = Rectangle.Intersect(s.WorkingArea, formRect);
            return intersection.Width >= MinVisiblePixels && intersection.Height >= MinVisiblePixels;
        });
    }

    private static string ResolvePath(string formTypeName, string? contextId)
    {
        string appName = Application.ProductName ?? "Application";
        string fileName = string.IsNullOrEmpty(contextId)
            ? $"FormState_{formTypeName}.json"
            : $"FormState_{formTypeName}_{contextId}.json";

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            appName,
            fileName);
    }

    private sealed class FormStateData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMaximized { get; set; }
    }
}
