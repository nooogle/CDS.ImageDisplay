namespace CDS.Imaging.WinForms
{
    public static class Consts
    {
        public const float MinZoom = 0.01f;
        public const float MaxZoom = 200.0f;

        /// <summary>
        /// When the target zoom is within this value of 1.0f it will snap
        /// to 1.0f
        /// </summary>
        public const float SnapToZoom1Tolerance = 0.01f;
    }
}
