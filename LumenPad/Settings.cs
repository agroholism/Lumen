using System;
using System.IO;
using System.Drawing;

using FastColoredTextBoxNS;

namespace LumenPad {
	internal static class Settings {
		public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(179, 123, 178)), Brushes.Transparent, FontStyle.Regular);
		public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
		public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(255, 147, 89)), Brushes.Transparent, FontStyle.Regular);
		public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(87, 166, 74)), Brushes.Transparent, FontStyle.Regular);
		public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.FromArgb(179, 123, 178)), Brushes.Transparent, FontStyle.Regular);

		internal static void Load() {
			String[] settings = File.ReadAllLines("settings.cdl");

			foreach (var item in settings) {
				String[] splited = item.Split('=');
				Apply(splited[0].Trim(), splited[1].Trim());
			}
		}

		private static void Apply(String parameter, String value) {
			if (parameter == "color-scheme") {
				ApplyColorScheme(value);
			}
		}

		private static void ApplyColorScheme(String name) {
			String[] settings = File.ReadAllLines(name + ".cdl");

			foreach (String item in settings) {
				String[] splited = item.Split('=');
				String parameter = splited[0].Trim();
				String value = splited[1].Trim();

				switch (parameter) {
					case "keyword":
						Keyword = new TextStyle(new SolidBrush(Color.FromArgb(179, 123, 178)), Brushes.Transparent, FontStyle.Regular);
						break;
				}

			}
		}
	}
}
