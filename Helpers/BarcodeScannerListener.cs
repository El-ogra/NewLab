using System;
using System.Text;
using System.Windows.Input;

namespace NewLab.Helpers
{
    public class BarcodeScannerListener
    {
        private readonly StringBuilder _buffer = new();
        private DateTime _lastKeyTime = DateTime.MinValue;
        private const int MaxIntervalMs = 50;

        public event Action<string>? BarcodeScanned;

        public void OnPreviewKeyDown(KeyEventArgs e)
        {
            var now = DateTime.Now;
            if ((now - _lastKeyTime).TotalMilliseconds > MaxIntervalMs)
                _buffer.Clear();
            _lastKeyTime = now;

            if (e.Key == Key.Enter)
            {
                var raw = _buffer.ToString();
                _buffer.Clear();
                if (!string.IsNullOrEmpty(raw))
                    BarcodeScanned?.Invoke(raw);
                return;
            }

            var ch = KeyToChar(e.Key);
            if (ch != null) _buffer.Append(ch);
        }

        private static char? KeyToChar(Key k) => k switch
        {
            >= Key.D0 and <= Key.D9 => (char)('0' + (k - Key.D0)),
            >= Key.NumPad0 and <= Key.NumPad9 => (char)('0' + (k - Key.NumPad0)),
            _ => null
        };
    }
}
