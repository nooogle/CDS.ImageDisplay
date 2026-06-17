using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.ImageBrowsing;

namespace UnitTests.ImageBrowsing;

/// <summary>
/// Tests for the <see cref="ImageItemStatus"/> type and the status-indicator wiring on
/// <see cref="ImageListPanel"/>.
/// </summary>
[TestClass]
public sealed class ImageListPanelStatusTests
{
    // -----------------------------------------------------------------------
    // ImageItemStatus
    // -----------------------------------------------------------------------

    /// <summary>
    /// Verifies that constructing with only a colour leaves <see cref="ImageItemStatus.BadgeText"/> null.
    /// </summary>
    [TestMethod]
    [TestCategory("ImageItemStatus")]
    public void ImageItemStatus_ColorOnly_BadgeTextIsNull()
    {
        // Arrange / Act
        var status = new ImageItemStatus(Color.Red);

        // Assert
        status.Color.Should().Be(Color.Red);
        status.BadgeText.Should().BeNull();
    }

    /// <summary>
    /// Verifies that both properties are stored when badge text is supplied.
    /// </summary>
    [TestMethod]
    [TestCategory("ImageItemStatus")]
    public void ImageItemStatus_WithBadgeText_BothPropertiesStored()
    {
        // Arrange / Act
        var status = new ImageItemStatus(Color.Green, "OK");

        // Assert
        status.Color.Should().Be(Color.Green);
        status.BadgeText.Should().Be("OK");
    }

    /// <summary>
    /// Verifies that record value equality holds for two instances with identical fields.
    /// </summary>
    [TestMethod]
    [TestCategory("ImageItemStatus")]
    public void ImageItemStatus_SameValues_AreEqual()
    {
        // Arrange
        var a = new ImageItemStatus(Color.Blue, "X");
        var b = new ImageItemStatus(Color.Blue, "X");

        // Act / Assert
        a.Should().Be(b);
    }

    /// <summary>
    /// Verifies that instances with different colours are not equal.
    /// </summary>
    [TestMethod]
    [TestCategory("ImageItemStatus")]
    public void ImageItemStatus_DifferentColor_AreNotEqual()
    {
        // Arrange
        var a = new ImageItemStatus(Color.Red, "X");
        var b = new ImageItemStatus(Color.Blue, "X");

        // Act / Assert
        a.Should().NotBe(b);
    }

    /// <summary>
    /// Verifies that instances with different badge text are not equal.
    /// </summary>
    [TestMethod]
    [TestCategory("ImageItemStatus")]
    public void ImageItemStatus_DifferentBadgeText_AreNotEqual()
    {
        // Arrange
        var a = new ImageItemStatus(Color.Red, "OK");
        var b = new ImageItemStatus(Color.Red, "ERR");

        // Act / Assert
        a.Should().NotBe(b);
    }

    // -----------------------------------------------------------------------
    // StatusProvider wiring
    // -----------------------------------------------------------------------

    /// <summary>
    /// Verifies that <see cref="ImageListPanel.StatusProvider"/> is null by default.
    /// </summary>
    [TestMethod]
    [TestCategory("StatusProvider")]
    public void StatusProvider_Default_IsNull()
    {
        // Arrange / Act
        using var panel = new ImageListPanel();

        // Assert
        panel.StatusProvider.Should().BeNull();
    }

    /// <summary>
    /// Verifies that the assigned delegate is returned by the property getter.
    /// </summary>
    [TestMethod]
    [TestCategory("StatusProvider")]
    public void StatusProvider_AfterAssignment_ReturnsSameDelegate()
    {
        // Arrange
        using var panel = new ImageListPanel();
        Func<string, ImageItemStatus?> provider = _ => null;

        // Act
        panel.StatusProvider = provider;

        // Assert
        (panel.StatusProvider == provider).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that assigning null is accepted and clears the provider.
    /// </summary>
    [TestMethod]
    [TestCategory("StatusProvider")]
    public void StatusProvider_SetToNull_ClearsProvider()
    {
        // Arrange
        using var panel = new ImageListPanel();
        panel.StatusProvider = _ => null;

        // Act
        panel.StatusProvider = null;

        // Assert
        panel.StatusProvider.Should().BeNull();
    }

    /// <summary>
    /// Verifies that assigning the same delegate reference twice does not throw.
    /// </summary>
    [TestMethod]
    [TestCategory("StatusProvider")]
    public void StatusProvider_AssignSameReferenceAgain_DoesNotThrow()
    {
        // Arrange
        using var panel = new ImageListPanel();
        Func<string, ImageItemStatus?> provider = _ => null;
        panel.StatusProvider = provider;

        // Act
        var act = () => { panel.StatusProvider = provider; };

        // Assert
        act.Should().NotThrow();
    }

    // -----------------------------------------------------------------------
    // RefreshStatuses
    // -----------------------------------------------------------------------

    /// <summary>
    /// Verifies that <see cref="ImageListPanel.RefreshStatuses"/> does not throw when the
    /// control has no window handle (e.g. before it is shown).
    /// </summary>
    [TestMethod]
    [TestCategory("RefreshStatuses")]
    public void RefreshStatuses_NoHandle_DoesNotThrow()
    {
        // Arrange
        using var panel = new ImageListPanel();

        // Act
        var act = () => panel.RefreshStatuses();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Verifies that <see cref="ImageListPanel.RefreshStatuses"/> does not throw when
    /// <see cref="ImageListPanel.StatusProvider"/> is null.
    /// </summary>
    [TestMethod]
    [TestCategory("RefreshStatuses")]
    public void RefreshStatuses_NullProvider_DoesNotThrow()
    {
        // Arrange
        using var panel = new ImageListPanel();
        panel.StatusProvider = null;

        // Act
        var act = () => panel.RefreshStatuses();

        // Assert
        act.Should().NotThrow();
    }
}
