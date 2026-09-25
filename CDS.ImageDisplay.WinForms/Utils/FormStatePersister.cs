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
/// after <c>InitializeComponent()</c> and hold the instance for the form's lifetime; the state
/// is restored immediately and event wiring is handled automatically.
/// </para>
/// <para>
/// <b>When the state is restored:</b> the component implements <see cref="ISupportInitialize"/>,
/// so the designer wraps <c>InitializeComponent</c> in <c>BeginInit()</c>/<c>EndInit()</c> calls.
/// <c>EndInit()</c> runs after the form's designer properties are set but before its window
/// handle exists, so the saved bounds and maximised state are applied without showing or
/// re-laying-out the window. Designer files written before this component implemented
/// <see cref="ISupportInitialize"/> have no <c>EndInit()</c> call until the form is re-saved in the
/// designer; for those the bounds are restored when the handle is created (still before the
/// window is first shown), but a saved maximised state can only be applied once the form has been
/// shown. Re-save such forms in the designer to restore maximised forms without that extra step.
/// </para>
/// <para>
/// The saved bounds are only used when at least 100 × 100 pixels of the window would be visible in
/// the working area of a current screen; otherwise (e.g. a monitor has been disconnected) the
/// form keeps its own default size and position. A minimised form is saved, and so restored, as a
/// normal window. The DPI the state was saved at is recorded, and the saved size is rescaled when
/// the form's DPI differs on restore; the position is used as saved and relies on the visibility
/// check above.
/// </para>
/// </remarks>
[DesignerCategory("Component")]
[ToolboxItem(true)]
public sealed class FormStatePersister : Component, ISupportInitialize
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private const int MinVisiblePixels = 100;
    private const int MinRestoredSize = 50;

    private Form? _form;
    private string? _contextId;
    private bool _initializing;
    private bool _restored;
    private bool _pendingMaximize;

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
    /// <remarks>
    /// Set this before the state is restored: in the designer, or through the
    /// <see cref="FormStatePersister(Form, string?)"/> constructor.
    /// </remarks>
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
                _form.HandleCreated -= OnFormHandleCreated;
                _form.Shown -= OnFormShown;
                _form.FormClosing -= OnFormClosing;
            }

            _form = value;
            _restored = false;
            _pendingMaximize = false;

            if (_form != null && !DesignMode)
            {
                _form.HandleCreated += OnFormHandleCreated;
                _form.Shown += OnFormShown;
                _form.FormClosing += OnFormClosing;
            }
        }
    }

    /// <summary>
    /// Overrides the folder the state file is written to. Tests use this to keep state files
    /// out of the user's profile.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal string? StateDirectory { get; set; }

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
        if (container == null) { throw new ArgumentNullException(nameof(container)); }
        container.Add(this);
    }

    /// <summary>
    /// Initialises a new instance, associates it with the given form and restores the form's
    /// saved state. Convenient for code-behind use when the Visual Studio designer is not involved;
    /// call it after <c>InitializeComponent()</c> so the designer's size and start position do not
    /// overwrite the restored ones.
    /// </summary>
    /// <param name="form">The form whose state will be saved and restored.</param>
    /// <param name="contextId">
    /// Optional identifier appended to the state file name so that the same form class can
    /// maintain separate states for different usage contexts.
    /// </param>
    public FormStatePersister(Form form, string? contextId = null)
        : this(form, contextId, stateDirectory: null)
    {
    }

    internal FormStatePersister(Form form, string? contextId, string? stateDirectory) : this()
    {
        if (form == null) { throw new ArgumentNullException(nameof(form)); }
        _contextId = string.IsNullOrEmpty(contextId) ? null : contextId;
        StateDirectory = stateDirectory;
        Form = form;
        TryRestore();
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

    /// <summary>
    /// Signals that the designer is about to set this component's properties; the saved state is
    /// not restored until <see cref="ISupportInitialize.EndInit"/>.
    /// </summary>
    void ISupportInitialize.BeginInit() => _initializing = true;

    /// <summary>
    /// Restores the form's saved state. The designer calls this at the end of
    /// <c>InitializeComponent</c>, after the form's own properties are set and before its handle
    /// is created, so the window is first shown at the restored size, position and state.
    /// </summary>
    void ISupportInitialize.EndInit()
    {
        _initializing = false;
        TryRestore();
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

    // Fallback for designer files without an EndInit() call. The Visible flag is only set when
    // the window processes WM_SHOWWINDOW, so at this point nothing has been shown yet.
    private void OnFormHandleCreated(object? sender, EventArgs e) => TryRestore();

    private void OnFormShown(object? sender, EventArgs e)
    {
        if (!_pendingMaximize) { return; }

        _pendingMaximize = false;
        _form!.WindowState = FormWindowState.Maximized;
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e) => Save();

    // -------------------------------------------------------------------------
    // Save / Restore
    // -------------------------------------------------------------------------

    private void TryRestore()
    {
        if (_restored || _initializing || _form is null || DesignMode) { return; }
        _restored = true;

        var data = Load();
        if (data is not null) { Apply(data); }
    }

    private FormStateData? Load()
    {
        string filePath = FilePath!;
        if (!File.Exists(filePath)) { return null; }

        try
        {
            return JsonSerializer.Deserialize<FormStateData>(File.ReadAllText(filePath), s_jsonOptions);
        }
        catch (Exception ex) when (ex is JsonException or IOException)
        {
            // Fall back to design-time defaults when the state file is corrupt or unreadable.
            return null;
        }
    }

    private void Apply(FormStateData data)
    {
        var form = _form!;
        bool beforeHandle = !form.IsHandleCreated;
        var bounds = GetRestoredBounds(data, form.DeviceDpi);

        if (bounds is { } restored && IsOnScreen(restored))
        {
            // Without Manual, the form's own StartPosition (CenterParent, WindowsDefaultLocation, ...)
            // would move the window again when it is first shown.
            form.StartPosition = FormStartPosition.Manual;
            form.Bounds = beforeHandle ? ToPendingScaleBounds(form, restored) : restored;
        }

        if (!data.IsMaximized) { return; }

        if (beforeHandle)
        {
            // Without a handle the setter only records the state; the first ShowWindow applies it.
            form.WindowState = FormWindowState.Maximized;
        }
        else if (form.Visible)
        {
            form.WindowState = FormWindowState.Maximized;
        }
        else
        {
            // Form.CreateHandle reads the real window state back after creating the handle, which
            // resets a maximised state set now to Normal, so it has to wait until the form is shown.
            _pendingMaximize = true;
        }
    }

    private void Save()
    {
        var form = _form!;

        // A maximised or minimised form reports its full-screen or iconic bounds; RestoreBounds is
        // the normal window. Minimised is deliberately saved as Normal.
        var bounds = form.WindowState == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;

        var data = new FormStateData
        {
            X = bounds.X,
            Y = bounds.Y,
            Width = bounds.Width,
            Height = bounds.Height,
            IsMaximized = form.WindowState == FormWindowState.Maximized || _pendingMaximize,
            Dpi = form.DeviceDpi,
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

    /// <summary>
    /// Returns the saved window bounds in the form's current pixels, or <see langword="null"/>
    /// when the saved size is unusable.
    /// </summary>
    internal static Rectangle? GetRestoredBounds(FormStateData data, int currentDpi)
    {
        if (data.Width <= 0 || data.Height <= 0) { return null; }

        double dpiScale = data.Dpi > 0 && currentDpi > 0 ? (double)currentDpi / data.Dpi : 1.0;
        int width = Math.Max((int)Math.Round(data.Width * dpiScale), MinRestoredSize);
        int height = Math.Max((int)Math.Round(data.Height * dpiScale), MinRestoredSize);

        return new Rectangle(data.X, data.Y, width, height);
    }

    /// <summary>
    /// Converts window bounds into the units the form is in before its pending auto-scale.
    /// </summary>
    /// <remarks>
    /// The designer's <c>EndInit()</c> runs before its <c>ResumeLayout</c>, which is where a form
    /// whose <see cref="ContainerControl.AutoScaleDimensions"/> differ from the current ones is
    /// scaled; that scales the client area (not the location or the window frame). Pre-dividing the
    /// client size makes the window land on <paramref name="target"/> after scaling. Once scaling
    /// has run, <see cref="ContainerControl.AutoScaleDimensions"/> equals
    /// <see cref="ContainerControl.CurrentAutoScaleDimensions"/>, so the factor is 1 and the bounds
    /// pass through unchanged.
    /// </remarks>
    private static Rectangle ToPendingScaleBounds(Form form, Rectangle target)
    {
        var factor = GetPendingAutoScaleFactor(form);
        if (factor == new SizeF(1f, 1f)) { return target; }

        var frame = form.Size - form.ClientSize;
        int width = (int)Math.Round((target.Width - frame.Width) / factor.Width) + frame.Width;
        int height = (int)Math.Round((target.Height - frame.Height) / factor.Height) + frame.Height;

        return new Rectangle(target.Location, new Size(width, height));
    }

    private static SizeF GetPendingAutoScaleFactor(Form form)
    {
        if (form.AutoScaleMode is not (AutoScaleMode.Font or AutoScaleMode.Dpi)) { return new SizeF(1f, 1f); }

        var design = form.AutoScaleDimensions;
        var current = form.CurrentAutoScaleDimensions;
        if (design.Width <= 0 || design.Height <= 0 || current.Width <= 0 || current.Height <= 0)
        {
            return new SizeF(1f, 1f);
        }

        return new SizeF(current.Width / design.Width, current.Height / design.Height);
    }

    internal static bool IsOnScreen(Rectangle formRect)
    {
        return Screen.AllScreens.Any(s =>
        {
            var intersection = Rectangle.Intersect(s.WorkingArea, formRect);
            return intersection.Width >= MinVisiblePixels && intersection.Height >= MinVisiblePixels;
        });
    }

    private string ResolvePath(string formTypeName, string? contextId)
    {
        string fileName = string.IsNullOrEmpty(contextId)
            ? $"FormState_{formTypeName}.json"
            : $"FormState_{formTypeName}_{contextId}.json";

        string directory = StateDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Application.ProductName ?? "Application");

        return Path.Combine(directory, fileName);
    }

    internal sealed class FormStateData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMaximized { get; set; }

        /// <summary>The form's DPI when saved; 0 in files written before it was recorded.</summary>
        public int Dpi { get; set; }
    }
}
