using System.Drawing;

using FastColoredTextBoxNS;

namespace LumenPad {
	internal static class Settings {
		public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(179, 123, 178)), Brushes.Transparent, FontStyle.Regular);
		public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
		public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(255, 147, 89)), Brushes.Transparent, FontStyle.Regular);
		public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(87, 166, 74)), Brushes.Transparent, FontStyle.Regular);
		public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(179, 123, 178)), Brushes.Transparent, FontStyle.Regular);
	}
}
