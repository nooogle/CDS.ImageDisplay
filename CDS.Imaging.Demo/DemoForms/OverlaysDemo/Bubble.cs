using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Overlays;
using System;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    public class Bubble
    {
        private Random random = new Random();
        private Overlays.CircleShape shape;


        // Speed (pixels per move, for example)
        public double Velocity { get; set; }

        // Direction in degrees (0° is to the right, 90° is downwards)
        public double Direction { get; private set; }

        // Display dimensions
        private readonly double displayWidth;
        private readonly double displayHeight;

        public Bubble(
            float x, 
            float y,
            float radius,
            double velocity, 
            double direction, 
            double displayWidth, 
            double displayHeight)
        {
            shape = new Overlays.CircleShape()
            {
                Centre = new PointF(x, y),
                Radius = radius,
                PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre,
            };

            Velocity = velocity;
            Direction = direction;
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
        }

        /// <summary>
        /// Moves the bubble one step based on its velocity and direction.
        /// The bubble bounces off the walls with a reflection where the angle of incidence equals the angle of reflection.
        /// </summary>
        public void Move()
        {
            // Convert direction to radians for calculation
            double radians = Direction * Math.PI / 180.0;

            // Calculate new position based on current velocity and direction
            double newX = shape.Centre.X + Velocity * Math.Cos(radians);
            double newY = shape.Centre.Y + Velocity * Math.Sin(radians);


            // Check for collision with vertical walls (left/right)
            if (newX < 0)
            {
                newX = -newX; // Reflect position into the display
                              // Reflect the horizontal component: new angle = 180 - current angle.
                Direction = 180 - Direction;
            }
            else if (newX > displayWidth)
            {
                newX = 2 * displayWidth - newX;
                Direction = 180 - Direction;
            }

            // Check for collision with horizontal walls (top/bottom)
            if (newY < 0)
            {
                newY = -newY;
                // Reflect the vertical component: new angle = -current angle.
                Direction = -Direction;
            }
            else if (newY > displayHeight)
            {
                newY = 2 * displayHeight - newY;
                Direction = -Direction;
            }

            // Normalize the direction so it remains within 0-360 degrees
            Direction = NormalizeAngle(Direction);

            // Update position
            shape.Centre = new PointF((float)newX, (float)newY);
        }

        /// <summary>
        /// Normalises an angle to be within 0 to 360 degrees.
        /// </summary>
        private double NormalizeAngle(double angle)
        {
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }

        internal void Draw(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, DrawingSpec drawingSpec)
        {
            shape.Draw(bitmapDisplayPanel, graphics, drawingSpec);
        }
    }
}
