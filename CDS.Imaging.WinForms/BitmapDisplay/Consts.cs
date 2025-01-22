namespace CDS.Imaging.BitmapDisplay
{
    /// <summary>
    /// Constants for the module
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// Minimum zoom level
        /// </summary>
        public const float MinZoom = 0.01f;


        /// <summary>
        /// Maximum zoom level
        /// </summary>
        public const float MaxZoom = 200.0f;


        /// <summary>
        /// When the target zoom is within this value of 1.0f it will snap
        /// to 1.0f
        /// </summary>
        public const float SnapToZoom1Tolerance = 0.01f;
    }
}
