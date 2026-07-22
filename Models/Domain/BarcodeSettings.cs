namespace NewLab.Models.Domain
{
    public class BarcodeSettings
    {
        public int Id { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool PrintFileCodeWithAll { get; set; }
        public int LabelWidth { get; set; } = 38;
        public int LabelHeight { get; set; } = 25;
    }
}
