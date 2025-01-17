using CDS.Imaging.WinForms.BitmapDisplay;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Polygon
    {
        public PointFEditable[] Points { get; set; } = [];


        public DisplayPixelAlign CentreDisplayMode { get; set; } = DisplayPixelAlign.Centre;
    }
}
