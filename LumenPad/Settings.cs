using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using FastColoredTextBoxNS;
using Lumen.Lang.Std;
using Lumen.Tomen;

namespace Lumen.Studio {
	internal static class Settings {
		public static String DefaultFileName { get; private set; } = "default.lm";

		public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
		public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
		public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.OrangeRed), Brushes.Transparent, FontStyle.Regular);
		public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.Green), Brushes.Transparent, FontStyle.Regular);
		public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);

		public static Color BackgroundColor { get; set; } = Color.White;
		public static Color LinesColor { get; set; } = Color.Silver;
		public static Color ForegroundColor { get; set; } = Color.Black;

		public static List<Language> Languages { get; set; } = new List<Language>();

		public static String CmdPath { get; set; } = "C:\\WINDOWS\\system32\\cmd.exe";
		public static Int32 CmdCodePage { get; set; } = 1251;

		public static TomlTable mainTable;
		public static TomlTable colorSchemeTable;

		internal static void Load() {
			mainTable = Tomen.Tomen.ReadFile("settings\\main.toml");

			if(mainTable.Contains("cmd")) {
				LoadCmdSettings(mainTable["cmd"]);
			}

			LoadColorScheme();
			LoadLanguages();
		}

		private static void LoadCmdSettings(ITomlValue tomlValue) {
			TomlTable table = tomlValue as TomlTable;

			if(table.Contains("path")) {
				CmdPath = (table["path"] as TomlString).Value;
			}

			if (table.Contains("cp")) {
				CmdCodePage = (Int32)(table["cp"] as TomlInt).Value;
			}
		}

		private static void LoadLanguages() {
			if (mainTable.Contains("languages")) {
				List<ITomlValue> langArray = (mainTable["languages"] as TomlArray).Value;
				foreach(ITomlValue lang in langArray) {
					if (System.IO.File.Exists("settings\\languages\\" + (lang as TomlString).Value + ".toml")) {
						LoadLanguage("settings\\languages\\", (lang as TomlString).Value);
					} else {
						LoadLanguage("settings\\languages\\" + (lang as TomlString).Value + "\\", (lang as TomlString).Value);
					}
				}
			}
		}

		private static void LoadLanguage(String path, String name) {
			TomlTable langInfo = Tomen.Tomen.ReadFile(path+ name + ".toml");

			Language result = new Language();

			if (langInfo.Contains("name")) {
				result.Name = (langInfo["name"] as TomlString).Value;
			}

			if (langInfo.Contains("on_run")) {
				result.RunCommand = (langInfo["on_run"] as TomlString).Value;
			}

			if (langInfo.Contains("extensions")) {
				result.Extensions = (langInfo["extensions"] as TomlArray).Value.Select(i => (i as TomlString).Value).ToList();
			}

			if (langInfo.Contains("folding")) {
				foreach(ITomlValue i in (langInfo["folding"] as TomlArray).Value) {
					List<ITomlValue> arr = (i as TomlArray).Value;
					result.Folding.Add(
						((arr[0] as TomlString).Value,
						(arr[1] as TomlString).Value));
				}
			}

			if (langInfo.Contains("styles")) {
				foreach (ITomlValue i in (langInfo["styles"] as TomlArray).Value) {
					List<ITomlValue> arr = (i as TomlArray).Value;
					result.Styles[(arr[0] as TomlString).Value] = NameToStyle((arr[1] as TomlString).Value);
				}
			}

			if (langInfo.Contains("actor")) {
				String f = path + (langInfo["actor"] as TomlString).Value;

				result.Actor = f;
			}


			Languages.Add(result);
		}

		private static Style NameToStyle(String value) {
			switch(value) {
				case "string":
					return Settings.String;
				case "comment":
					return Settings.Comment;
				case "keyword":
					return Settings.Keyword;
				case "class":
					return Settings.Type;
				default:
					return null;
			}
		}

		private static void LoadColorScheme() {
			if (mainTable.Contains("colorscheme")) {
				colorSchemeTable = Tomen.Tomen.ReadFile("settings\\color-schemes\\" + (mainTable["colorscheme"] as TomlString).Value + ".toml");

				if (colorSchemeTable.Contains("bgcolor")) {
					BackgroundColor = ParseColor((colorSchemeTable["bgcolor"] as TomlString).Value);
				}

				if (colorSchemeTable.Contains("gcolor")) {
					LinesColor = ParseColor((colorSchemeTable["gcolor"] as TomlString).Value);
				}

				if (colorSchemeTable.Contains("fgcolor")) {
					ForegroundColor = ParseColor((colorSchemeTable["fgcolor"] as TomlString).Value);
				}

				if (colorSchemeTable.Contains("syntax")) {
					TomlTable syntax = colorSchemeTable["syntax"] as TomlTable;

					if(syntax.Contains("keyword1")) {
						Keyword = new TextStyle(new SolidBrush(ParseColor((syntax["keyword1"] as TomlString).Value)), Brushes.Transparent, FontStyle.Regular);
					}

					if (syntax.Contains("text")) {
						String = new TextStyle(new SolidBrush(ParseColor((syntax["text"] as TomlString).Value)), Brushes.Transparent, FontStyle.Regular);
					}

					if (syntax.Contains("comment1")) {
						Comment = new TextStyle(new SolidBrush(ParseColor((syntax["comment1"] as TomlString).Value)), Brushes.Transparent, FontStyle.Regular);
					}

					if (syntax.Contains("class")) {
						Type = new TextStyle(new SolidBrush(ParseColor((syntax["class"] as TomlString).Value)), Brushes.Transparent, FontStyle.Regular);
					}
				}
			}
		}

		private static Color ParseColor(String input) {
			if (Regex.IsMatch(input, @"(?<A>\d+)[;,]\s*(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?")) {
				Match match = Regex.Match(input, @"(?<A>\d+)[;,]\s*(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?");
				return Color.FromArgb(Convert.ToInt32(match.Groups["A"].Value), Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
			}
			else if (Regex.IsMatch(input, @"(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?")) {
				Match match = Regex.Match(input, @"(?<R>\d+)[;,]\s*(?<G>\d+)[,;]\s*(?<B>\d+)");
				return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
			}
			else if (Regex.IsMatch(input, @"#\s*(?<R>[0-9A-Fa-f]{1,2})\s*(?<G>[0-9A-Fa-f]{1,2})\s*(?<B>[0-9A-Fa-f]{1,2})")) {
				Match match = Regex.Match(input, @"#\s*(?<R>[0-9A-Fa-f]{1,2})\s*(?<G>[0-9A-Fa-f]{1,2})\s*(?<B>[0-9A-Fa-f]{1,2})");
				return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value, 16), Convert.ToInt32(match.Groups["G"].Value, 16), Convert.ToInt32(match.Groups["B"].Value, 16));
			}

			return Color.FromName(input);
		}
	}
}
