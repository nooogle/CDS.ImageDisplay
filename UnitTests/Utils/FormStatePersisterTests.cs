using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Utils;
using UnitTests.BitmapDisplay;

namespace UnitTests.Utils;

/// <summary>
/// Tests for <see cref="FormStatePersister"/>.
/// </summary>
/// <remarks>
/// Each test builds its form the way designer-generated <c>InitializeComponent</c> code does, on an
/// STA thread, and points the persister at a private temporary folder. "Legacy" forms omit the
/// <c>BeginInit()</c>/<c>EndInit()</c> calls, as designer files written before the component
/// implemented <see cref="ISupportInitialize"/> do.
/// </remarks>
[TestClass]
[TestCategory("FormState")]
public sealed class FormStatePersisterTests
{
    private static readonly Size s_designerClientSize = new(300, 200);

    private string _stateDirectory = string.Empty;


    [TestInitialize]
    public void TestInitialize()
    {
        _stateDirectory = Path.Combine(Path.GetTempPath(), "FormStatePersisterTests", Guid.NewGuid().ToString("N"));
    }


    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_stateDirectory)) { Directory.Delete(_stateDirectory, recursive: true); }
    }


    // -------------------------------------------------------------------------
    // Restore before the handle exists
    // -------------------------------------------------------------------------

    /// <summary>
    /// EndInit must apply the saved bounds while the form still has no handle, and switch the
    /// form to a manual start position so its own one cannot move the window again.
    /// </summary>
    [TestMethod]
    public void EndInit_WithSavedState_RestoresBoundsBeforeHandleCreated()
    {
        RunOnUIThread(() =>
        {
            var saved = OnScreenBounds();
            WriteState(saved);

            using var host = CreateDesignerForm(withInit: true);

            host.Form.IsHandleCreated.Should().BeFalse();
            host.Form.Bounds.Should().Be(saved);
            host.Form.StartPosition.Should().Be(FormStartPosition.Manual);
            host.Form.WindowState.Should().Be(FormWindowState.Normal);
        });
    }


    /// <summary>
    /// A saved maximised state must be recorded before the handle exists, so the form's first
    /// ShowWindow applies it rather than a later one made while the form is loading.
    /// </summary>
    [TestMethod]
    public void EndInit_WithSavedMaximized_SetsMaximizedBeforeHandleCreated()
    {
        RunOnUIThread(() =>
        {
            WriteState(OnScreenBounds(), isMaximized: true);

            using var host = CreateDesignerForm(withInit: true);

            host.Form.IsHandleCreated.Should().BeFalse();
            host.Form.WindowState.Should().Be(FormWindowState.Maximized);
        });
    }


    /// <summary>
    /// Nothing may be restored between BeginInit and EndInit, while the designer is still setting
    /// the form's own properties.
    /// </summary>
    [TestMethod]
    public void BeginInit_BeforeEndInit_DoesNotRestore()
    {
        RunOnUIThread(() =>
        {
            WriteState(OnScreenBounds());
            using var form = new TestForm();
            using var components = new Container();
            var persister = new FormStatePersister(components) { StateDirectory = _stateDirectory };

            ((ISupportInitialize)persister).BeginInit();
            persister.Form = form;
            form.ClientSize = s_designerClientSize;

            form.ClientSize.Should().Be(s_designerClientSize);
            form.StartPosition.Should().Be(FormStartPosition.WindowsDefaultLocation);
        });
    }


    /// <summary>
    /// EndInit runs before the designer's ResumeLayout, which auto-scales a form whose
    /// AutoScaleDimensions differ from the current ones. The restored size must survive that
    /// scaling rather than be scaled a second time.
    /// </summary>
    [TestMethod]
    public void EndInit_WithPendingAutoScale_RestoresSavedSizeAfterScaling()
    {
        RunOnUIThread(() =>
        {
            var saved = OnScreenBounds();
            WriteState(saved);
            var current = CurrentFontAutoScaleDimensions();
            var designDimensions = new SizeF(current.Width / 1.5f, current.Height / 1.5f);

            using var host = CreateDesignerForm(withInit: true, autoScaleDimensions: designDimensions);

            host.Form.AutoScaleDimensions.Should().Be(host.Form.CurrentAutoScaleDimensions, "the form should have been scaled");
            host.Form.Location.Should().Be(saved.Location);
            host.Form.Width.Should().BeCloseTo(saved.Width, 2);
            host.Form.Height.Should().BeCloseTo(saved.Height, 2);
        });
    }


    /// <summary>
    /// A size saved at one DPI must be rescaled when the form is restored at another.
    /// </summary>
    [TestMethod]
    public void EndInit_WithDifferentSavedDpi_RescalesSavedSize()
    {
        RunOnUIThread(() =>
        {
            int currentDpi;
            using (var probe = new Form()) { currentDpi = probe.DeviceDpi; }
            var saved = OnScreenBounds();
            WriteState(new Rectangle(saved.X, saved.Y, saved.Width * 2, saved.Height * 2), dpi: currentDpi * 2);

            using var host = CreateDesignerForm(withInit: true);

            host.Form.Bounds.Should().Be(saved);
        });
    }


    /// <summary>
    /// The code-behind constructor restores immediately, before the handle exists.
    /// </summary>
    [TestMethod]
    public void Constructor_WithFormWithoutHandle_RestoresImmediately()
    {
        RunOnUIThread(() =>
        {
            var saved = OnScreenBounds();
            WriteState(saved, isMaximized: true);
            using var form = new TestForm { ClientSize = s_designerClientSize };

            using var persister = new FormStatePersister(form, contextId: null, _stateDirectory);

            form.IsHandleCreated.Should().BeFalse();
            form.Bounds.Should().Be(saved);
            form.StartPosition.Should().Be(FormStartPosition.Manual);
            form.WindowState.Should().Be(FormWindowState.Maximized);
        });
    }


    // -------------------------------------------------------------------------
    // Rejected state
    // -------------------------------------------------------------------------

    /// <summary>
    /// Bounds that are not visible on any current screen must be ignored, leaving the form's own
    /// size and start position in place.
    /// </summary>
    [TestMethod]
    public void EndInit_WithOffScreenBounds_KeepsDesignerDefaults()
    {
        RunOnUIThread(() =>
        {
            var virtualScreen = SystemInformation.VirtualScreen;
            WriteState(new Rectangle(virtualScreen.Right + 1000, virtualScreen.Bottom + 1000, 400, 300));

            using var host = CreateDesignerForm(withInit: true);

            host.Form.ClientSize.Should().Be(s_designerClientSize);
            host.Form.StartPosition.Should().Be(FormStartPosition.CenterScreen);
        });
    }


    /// <summary>
    /// An unreadable state file must leave the form's designer defaults in place.
    /// </summary>
    [TestMethod]
    public void EndInit_WithCorruptStateFile_KeepsDesignerDefaults()
    {
        RunOnUIThread(() =>
        {
            Directory.CreateDirectory(_stateDirectory);
            File.WriteAllText(Path.Combine(_stateDirectory, $"FormState_{nameof(TestForm)}.json"), "{ not json");

            using var host = CreateDesignerForm(withInit: true);

            host.Form.ClientSize.Should().Be(s_designerClientSize);
            host.Form.StartPosition.Should().Be(FormStartPosition.CenterScreen);
        });
    }


    /// <summary>
    /// Sizes of zero or less are unusable.
    /// </summary>
    [TestMethod]
    [DataRow(0, 300)]
    [DataRow(400, -1)]
    public void GetRestoredBounds_WithNonPositiveSize_ReturnsNull(int width, int height)
    {
        var data = new FormStatePersister.FormStateData { X = 10, Y = 10, Width = width, Height = height };

        FormStatePersister.GetRestoredBounds(data, currentDpi: 96).Should().BeNull();
    }


    /// <summary>
    /// A state file written before the DPI was recorded is used as saved.
    /// </summary>
    [TestMethod]
    public void GetRestoredBounds_WithoutSavedDpi_ReturnsSavedBounds()
    {
        var data = new FormStatePersister.FormStateData { X = 10, Y = 20, Width = 400, Height = 300 };

        FormStatePersister.GetRestoredBounds(data, currentDpi: 144).Should().Be(new Rectangle(10, 20, 400, 300));
    }


    /// <summary>
    /// Bounds within a screen's working area are visible; bounds beyond every screen are not.
    /// </summary>
    [TestMethod]
    public void IsOnScreen_OnAndOffScreenBounds_ReturnsExpected()
    {
        var virtualScreen = SystemInformation.VirtualScreen;

        FormStatePersister.IsOnScreen(OnScreenBounds()).Should().BeTrue();
        FormStatePersister.IsOnScreen(new Rectangle(virtualScreen.Left - 5000, virtualScreen.Top, 400, 300)).Should().BeFalse();
    }


    // -------------------------------------------------------------------------
    // Showing the form
    // -------------------------------------------------------------------------

    /// <summary>
    /// The regression this component was changed for: restoring a maximised state from the Load
    /// handler showed the window partway through Load. Nothing may show it while OnLoad runs.
    /// </summary>
    [TestMethod]
    public void ShowDialog_WithSavedMaximized_DoesNotShowWindowDuringLoad()
    {
        RunOnUIThread(() =>
        {
            var saved = OnScreenBounds();
            WriteState(saved, isMaximized: true);
            using var host = CreateDesignerForm(withInit: true);

            host.Form.ShowDialog();

            host.Form.ShowsDuringLoad.Should().BeEmpty();
            host.Form.WindowStateWhenShown.Should().Be(FormWindowState.Maximized);
            host.Form.RestoreBoundsWhenShown.Should().Be(saved);
        });
    }


    /// <summary>
    /// Designer files without an EndInit call fall back to restoring when the handle is created:
    /// the window must still not be shown during Load, the start position must not move it, and a
    /// saved maximised state is applied once the form has been shown.
    /// </summary>
    [TestMethod]
    public void ShowDialog_LegacyDesignerFileWithSavedMaximized_RestoresWithoutShowingDuringLoad()
    {
        RunOnUIThread(() =>
        {
            var saved = OnScreenBounds();
            WriteState(saved, isMaximized: true);
            using var host = CreateDesignerForm(withInit: false);

            host.Form.IsHandleCreated.Should().BeFalse();
            host.Form.ShowDialog();

            host.Form.ShowsDuringLoad.Should().BeEmpty();
            host.Form.BoundsAtLoad.Should().Be(saved);
            host.Form.WindowStateWhenShown.Should().Be(FormWindowState.Maximized);
            host.Form.RestoreBoundsWhenShown.Should().Be(saved);
        });
    }


    // -------------------------------------------------------------------------
    // Save
    // -------------------------------------------------------------------------

    /// <summary>
    /// When a maximised form closes, its normal (restore) bounds must be saved, not the
    /// full-screen ones, along with the maximised flag.
    /// </summary>
    [TestMethod]
    public void Close_WhenMaximized_SavesRestoreBounds()
    {
        RunOnUIThread(() =>
        {
            var normal = OnScreenBounds();
            using var host = CreateDesignerForm(withInit: true, designerBounds: normal);
            host.Form.OnShownAction = f => f.WindowState = FormWindowState.Maximized;

            host.Form.ShowDialog();

            var data = ReadState();
            new Rectangle(data.X, data.Y, data.Width, data.Height).Should().Be(normal);
            data.IsMaximized.Should().BeTrue();
            data.Dpi.Should().Be(host.Form.DeviceDpi);
        });
    }


    /// <summary>
    /// A minimised form must be saved with its normal bounds and restored as a normal window.
    /// </summary>
    [TestMethod]
    public void Close_WhenMinimized_SavesAndRestoresAsNormal()
    {
        RunOnUIThread(() =>
        {
            var normal = OnScreenBounds();
            using (var first = CreateDesignerForm(withInit: true, designerBounds: normal))
            {
                first.Form.OnShownAction = f => f.WindowState = FormWindowState.Minimized;
                first.Form.ShowDialog();
            }

            var data = ReadState();
            new Rectangle(data.X, data.Y, data.Width, data.Height).Should().Be(normal);
            data.IsMaximized.Should().BeFalse();

            using var second = CreateDesignerForm(withInit: true);
            second.Form.WindowState.Should().Be(FormWindowState.Normal);
            second.Form.Bounds.Should().Be(normal);
        });
    }


    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static void RunOnUIThread(Action test) => UIThreadHarness.Run(new QueuedSynchronizationContext(), test);


    /// <summary>
    /// Bounds comfortably inside the primary screen's working area, even on a 1024 × 768 CI desktop.
    /// </summary>
    private static Rectangle OnScreenBounds()
    {
        var area = Screen.PrimaryScreen!.WorkingArea;
        return new Rectangle(area.X + 40, area.Y + 30, 420, 310);
    }


    private static SizeF CurrentFontAutoScaleDimensions()
    {
        using var probe = new Form { AutoScaleMode = AutoScaleMode.Font };
        return probe.CurrentAutoScaleDimensions;
    }


    /// <summary>
    /// Mirrors designer-generated <c>InitializeComponent</c> code, with or without the
    /// <see cref="ISupportInitialize"/> calls the designer emits for this component.
    /// </summary>
    private DesignerForm CreateDesignerForm(bool withInit, SizeF? autoScaleDimensions = null, Rectangle? designerBounds = null)
    {
        var form = new TestForm();
        var components = new Container();
        var persister = new FormStatePersister(components) { StateDirectory = _stateDirectory };

        if (withInit) { ((ISupportInitialize)persister).BeginInit(); }
        form.SuspendLayout();

        persister.Form = form;

        if (autoScaleDimensions is { } dimensions)
        {
            form.AutoScaleDimensions = dimensions;
            form.AutoScaleMode = AutoScaleMode.Font;
        }

        if (designerBounds is { } bounds)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Bounds = bounds;
        }
        else
        {
            form.ClientSize = s_designerClientSize;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        form.ShowInTaskbar = false;
        form.Controls.Add(new Button { Dock = DockStyle.Fill });

        if (withInit) { ((ISupportInitialize)persister).EndInit(); }
        form.ResumeLayout(false);

        return new DesignerForm(form, components);
    }


    private void WriteState(Rectangle bounds, bool isMaximized = false, int dpi = 0)
    {
        var data = new FormStatePersister.FormStateData
        {
            X = bounds.X,
            Y = bounds.Y,
            Width = bounds.Width,
            Height = bounds.Height,
            IsMaximized = isMaximized,
            Dpi = dpi,
        };

        Directory.CreateDirectory(_stateDirectory);
        File.WriteAllText(Path.Combine(_stateDirectory, $"FormState_{nameof(TestForm)}.json"), JsonSerializer.Serialize(data));
    }


    private FormStatePersister.FormStateData ReadState()
    {
        string json = File.ReadAllText(Path.Combine(_stateDirectory, $"FormState_{nameof(TestForm)}.json"));
        return JsonSerializer.Deserialize<FormStatePersister.FormStateData>(json)!;
    }


    private sealed class DesignerForm(TestForm form, Container components) : IDisposable
    {
        public TestForm Form { get; } = form;

        public void Dispose()
        {
            components.Dispose();
            Form.Dispose();
        }
    }


    /// <summary>
    /// Records every message that would show the window while <see cref="OnLoad"/> runs, and
    /// closes itself once shown.
    /// </summary>
    private sealed class TestForm : Form
    {
        private const int WM_SHOWWINDOW = 0x0018;
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int SWP_SHOWWINDOW = 0x0040;

        private bool _inLoad;

        public List<string> ShowsDuringLoad { get; } = [];
        public Rectangle BoundsAtLoad { get; private set; }
        public FormWindowState WindowStateWhenShown { get; private set; }
        public Rectangle RestoreBoundsWhenShown { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<TestForm>? OnShownAction { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            _inLoad = true;
            try
            {
                BoundsAtLoad = Bounds;
                base.OnLoad(e);
            }
            finally
            {
                _inLoad = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            WindowStateWhenShown = WindowState;
            RestoreBoundsWhenShown = RestoreBounds;
            OnShownAction?.Invoke(this);
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (_inLoad)
            {
                if (m.Msg == WM_SHOWWINDOW && m.WParam != IntPtr.Zero)
                {
                    ShowsDuringLoad.Add("WM_SHOWWINDOW");
                }
                else if (m.Msg == WM_WINDOWPOSCHANGING
                    && (Marshal.PtrToStructure<WINDOWPOS>(m.LParam).flags & SWP_SHOWWINDOW) != 0)
                {
                    ShowsDuringLoad.Add("WM_WINDOWPOSCHANGING+SWP_SHOWWINDOW");
                }
            }

            base.WndProc(ref m);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
    }
}
