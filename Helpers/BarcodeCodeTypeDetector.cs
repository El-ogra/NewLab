namespace NewLab.Helpers
{
    public enum BarcodeCodeType
    {
        VisitCode,
        FileCode,
        LabId,
        Fallback
    }

    public static class BarcodeCodeTypeDetector
    {
        public static BarcodeCodeType Detect(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return BarcodeCodeType.Fallback;

            var trimmed = code.Trim();
            if (trimmed.Length < 1) return BarcodeCodeType.Fallback;

            return trimmed[0] switch
            {
                '1' => BarcodeCodeType.VisitCode,
                '3' => BarcodeCodeType.FileCode,
                '5' => BarcodeCodeType.LabId,
                _ => BarcodeCodeType.Fallback
            };
        }
    }
}
