using Newtonsoft.Json;
using System.Drawing;

namespace CDS.Imaging.WinFormsTests.Draw;


[TestClass]
public partial class LayerTests
{
    [TestMethod]
    public Task SingleEmptyLayer_SerialisesToJSON_AndBack()
    {
        Imaging.Draw.Layer layer = new CDS.Imaging.Draw.Layer();
        var json = JsonConvert.SerializeObject(layer, Formatting.Indented);
        var deserialisedLayer = JsonConvert.DeserializeObject<Imaging.Draw.Layer>(json);

        var reviewData = new
        {
            layer,
            json,
            deserialisedLayer,
        };

        return Verify(reviewData);
    }


    [TestMethod]
    public Task LayerWithShapes_SerialisesToJSON_AndBack()
    {
        Imaging.Draw.Layer layer = new CDS.Imaging.Draw.Layer();

        layer.Shapes.Add(new CDS.Imaging.Draw.RectangleShape()
        {
            Rect = new Rectangle(1, 2, 3, 4),
            PixelAlign = Imaging.BitmapDisplay.DisplayPixelAlign.Centre,
            Visible = true
        });

        layer.Shapes.Add(new CDS.Imaging.Draw.RectangleShape()
        {
            Rect = new Rectangle(100, 200, 300, 400),
            PixelAlign = Imaging.BitmapDisplay.DisplayPixelAlign.TopLeft,
            Visible = false
        });

        var json = JsonConvert.SerializeObject(layer, Formatting.Indented);
        var deserialisedLayer = JsonConvert.DeserializeObject<Imaging.Draw.Layer>(json);

        var reviewData = new
        {
            layer,
            json,
            deserialisedLayer,
        };

        return Verify(reviewData);
    }

    [TestMethod]
    public Task LayerWithChildLayersAndShapes_SerialisesToJSONAndBack()
    {

        var childLayer1 = new CDS.Imaging.Draw.Layer() { Name = "Child layer 1" };

        childLayer1.Shapes.Add(new CDS.Imaging.Draw.RectangleShape()
        {
            Rect = new Rectangle(1, 2, 3, 4),
            PixelAlign = Imaging.BitmapDisplay.DisplayPixelAlign.Centre,
            Visible = true
        });

        var childLayer2 = new CDS.Imaging.Draw.Layer() { Name = "Child layer 2" };

        childLayer2.Shapes.Add(new CDS.Imaging.Draw.RectangleShape()
        {
            Rect = new Rectangle(100, 200, 300, 400),
            PixelAlign = Imaging.BitmapDisplay.DisplayPixelAlign.TopLeft,
            Visible = false
        });

        Imaging.Draw.Layer layer = new CDS.Imaging.Draw.Layer() { Name = "Root" };
        layer.ChildLayers.Add(childLayer1);
        layer.ChildLayers.Add(childLayer2);

        var json = JsonConvert.SerializeObject(layer, Formatting.Indented);
        var deserialisedLayer = JsonConvert.DeserializeObject<Imaging.Draw.Layer>(json);

        var reviewData = new
        {
            layer,
            json,
            deserialisedLayer,
        };

        return Verify(reviewData);
    }    

}
