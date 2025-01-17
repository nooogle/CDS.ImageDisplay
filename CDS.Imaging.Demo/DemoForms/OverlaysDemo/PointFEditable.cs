using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(PointFEditableConverter))]
    public struct PointFEditable
    {
        private float x;
        private float y;

        public float X
        {
            get => x;
            set => x = value;
        }


        public float Y
        {
            get => y;
            set => y = value;
        }


        public PointFEditable(float x, float y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Returns this as a <see cref="PointF"/>
        /// </summary>
        public static implicit operator PointF(PointFEditable pointFEditable)
        {
            return new PointF(pointFEditable.X, pointFEditable.Y);
        }
    }
}
