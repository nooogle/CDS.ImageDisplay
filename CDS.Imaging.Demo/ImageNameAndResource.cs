using System.Drawing;

namespace CDS.Imaging.Demo
{
    /// <summary>
    /// Encapsulates a resource image name and the actual resource - for use with a 
    /// combo box which will use the ToString override for the text content
    /// </summary>
    class ImageNameAndResource
    {
        public string Name { get; }
        public Bitmap Bitmap { get; }


        public ImageNameAndResource(string name, Bitmap bitmap)
        {
            Name = name;
            Bitmap = bitmap;
        }

        public override string ToString() => Name;
    }
}
