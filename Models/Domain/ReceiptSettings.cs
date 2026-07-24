namespace NewLab.Models.Domain
{
    public class ReceiptSettings
    {
        public int Id { get; set; }

        public bool AutoPrintAfterSave { get; set; }

        public bool ShowTestsDetails { get; set; }
    }
}
