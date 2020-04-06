using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

using FastColoredTextBoxNS;

namespace EuIDE {
	internal static class Params {
		public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.Black), Brushes.Transparent, FontStyle.Bold);
		public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
		public static Style Message { get; set; } = new TextStyle(new SolidBrush(Color.White), Brushes.Transparent, FontStyle.Italic);
		public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.OrangeRed), Brushes.Transparent, FontStyle.Regular);
		public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.Green), Brushes.Transparent, FontStyle.Italic);
		public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.Black), Brushes.Transparent, FontStyle.Bold);
		public static Style Function { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);

		public static Dictionary<Regex, Style> Styles { get; } = new Dictionary<Regex, Style>();

		static Params() {
			Styles.Add(new Regex("(\"\")|\".*?[^\\\\]\"", RegexOptions.Compiled | RegexOptions.Singleline), Params.String);
			Styles.Add(new Regex("--.*"), Params.Comment);
			Styles.Add(new Regex("\\b(type|override|before|after|as|yield|auto|nil|namespace|error|fail|exit|function|contract|end|if|elif|else|while|for|to|in|do|then|return|constant|or|xor|and|not)\\b"), Params.Keyword);
			Styles.Add(new Regex("\\b(atom|mut|var|task|void|integer|object|list|mutable_list|bool|text|sequence|iterator)\\b"), Params.Type);
		}
	}
}
