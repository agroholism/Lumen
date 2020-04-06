using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using FastColoredTextBoxNS;
using Lumen.Lang;
using Regex = System.Text.RegularExpressions.Regex;

namespace Lumen.Studio {
	public static class Settings {
		public static String DefaultFileName { get; private set; } = "default.lm";

		public static Font DefaultInterfaceFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);

		// Syntax hightlight
		public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
		public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
		public static Style Message { get; set; } = new TextStyle(new SolidBrush(Color.White), Brushes.Transparent, FontStyle.Italic);
		public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.OrangeRed), Brushes.Transparent, FontStyle.Regular);
		public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.Green), Brushes.Transparent, FontStyle.Regular);
		public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
		public static Style Function { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
		public static Style SelectedIdentifier { get; set; } = new SelectionStyle(new SolidBrush(Color.DarkBlue));

		// Main colors
		public static Color BackgroundColor { get; set; } = Color.White;
		public static Color LinesColor { get; set; } = Color.Silver;
		public static Color ForegroundColor { get; set; } = Color.Black;

		// Top menu colors
		public static class TopMenu {
			public static Color Background { get; set; } = Color.White;
			public static Color SelectedBackground { get; set; } = Color.White;
			public static Color BordersColor { get; set; } = Color.Silver;
			public static Color ArrowColor { get; set; } = Color.Silver;
			public static Color ArrowSelectedColor { get; set; } = Color.Silver;
			public static Color ForegroundColor { get; set; } = Color.Black;
		}

		// Autocomplete menu colors
		public static class AutocompleteMenu {
			public static Color Background { get; set; } = Color.White;
			public static Color SelectedBackground { get; set; } = Color.White;
			public static Color BordersColor { get; set; } = Color.Silver;
			public static Color ArrowColor { get; set; } = Color.Silver;
			public static Color ForegroundColor { get; set; } = Color.Black;
			public static Color ToolTipForeColor { get; set; } = Color.Black;
			public static Color ToolTipBackColor { get; set; } = Color.Silver;
		}

		// Breakpoints settings
		public static Color TextBoxBookMarkColor { get; set; } = Color.FromArgb(159, 65, 65);
		public static Color TextBoxActiveBookMarkColor { get; set; } = Color.FromArgb(255, 221, 56);
		public static Style UnactiveBreakpoint { get; set; } = new TextStyle(Brushes.White, new SolidBrush(Color.FromArgb(159, 65, 65)), FontStyle.Regular);
		public static Style ActiveBreakpoint { get; set; } = new TextStyle(Brushes.Black, new SolidBrush(Color.FromArgb(255, 221, 56)), FontStyle.Regular);


		public static String LastOpenedProject { get; set; }

		public static List<Language> Languages { get; set; } = new List<Language> { };
		public static List<ProjectType> ProjectTypes { get; set; } = new List<ProjectType>();

		public static XmlDocument MainSettings { get; private set; }
		public static XmlDocument ThemeDocument { get; set; }
		public static Style Interface { get; internal set; } = new TextStyle(new SolidBrush(Color.IndianRed), Brushes.Transparent, FontStyle.Regular);

		public static Dictionary<String, Int32> ProjectManagerImagesDictionary { get; } = new Dictionary<string, int>();
		public static List<Object> ProjectManagerImages { get; } = new List<Object>();

		public static String ExecutablePath { get; private set; }

		public static Image EmptyImage { get; private set; }

		public static VerticalScrollMode ScrollBars { get; private set; }

		public enum VerticalScrollMode {
			ON,
			OFF,
			MAP
		}

		public static class Loader {
			internal static void Load() {

				if (ExecutablePath == null) {
					ExecutablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
				}

				EmptyImage = Image.FromFile(ExecutablePath + $"\\settings\\themes\\icons\\standart\\empty.png");

				MainSettings = new XmlDocument();
				MainSettings.Load(ExecutablePath + "\\settings\\main.xml");

				LoadLastData();
				Theme();
				LoadLanguages();
				LoadProjectTypes();
			}

			internal static void LoadExtenstions() {
				XmlNode lastData = MainSettings.DocumentElement["Extensions"];
				if (lastData != null) {
					foreach (XmlNode item in lastData.ChildNodes) {
						String extensionName = item.Attributes["name"]?.Value;
						if (extensionName != null) {
							LoadExtenstion(extensionName);
						}
					}
				}
			}

			private static void LoadExtenstion(String extensionName) {
				if (System.IO.File.Exists($"settings\\extensions\\{extensionName}.dll")) {
					Assembly ass = Assembly.LoadFile(System.IO.Path.GetFullPath($"settings\\extensions\\{extensionName}.dll"));
					ass.GetType($"{extensionName}.Intitializer").GetMethod("Inititalize").Invoke(null, new Object[] { });
				} else {
					MessageBox.Show($"f {extensionName} dne");
				}
			}

			private static void LoadProjectTypes() {
				ProjectTypes.Add(new LumenConsoleApplication());
				ProjectTypes.Add(new LumenFormsApplication());
				ProjectTypes.Add(new AnamomyWebApplication());
			}

			private static void LoadLastData() {
				XmlNode lastData = MainSettings.DocumentElement["LastData"];
				if (lastData != null) {
					String nameProject = lastData.Attributes["project"]?.Value;
					if (nameProject != null) {
						LastOpenedProject = nameProject;
					}
				}
			}

			private static void LoadLanguages() {
				Languages.Add(new LumenLanguage());

				XmlNode aviable = MainSettings.DocumentElement["AviableLanguages"];

				if (aviable != null) {
					foreach (XmlNode i in aviable.ChildNodes) {
						String name = i.Attributes["name"]?.Value;

						if (System.IO.File.Exists(ExecutablePath + "\\settings\\languages\\" + name + ".xml")) {
							LoadLanguage(ExecutablePath + "\\settings\\languages\\", name);
						}
						else {
							LoadLanguage(ExecutablePath + "\\settings\\languages\\" + name + "\\", name);
						}
					}
				}
			}

			private static void LoadLanguage(String path, String name) {
				Language result = new Language();

				XmlDocument langDocument = new XmlDocument();
				langDocument.Load(path + name + ".xml");

				result.Name = langDocument.DocumentElement.Attributes["name"]?.Value;
				result.Extensions = langDocument.DocumentElement.Attributes["extensions"]?.Value?.Split('|')?.ToList() ?? new List<String>();

				XmlNode foldings = langDocument.DocumentElement["Foldings"];
				if (foldings != null) {
					foreach (XmlNode node in foldings.ChildNodes) {
						result.Folding.Add((node.Attributes["begin"]?.Value, node.Attributes["end"]?.Value));
					}
				}

				XmlNode styles = langDocument.DocumentElement["Styles"];
				if (styles != null) {
					foreach (XmlNode node in styles.ChildNodes) {
						if (node.Attributes["options"]?.Value == "sinln")
							result.Styles.Add(new Regex(node.Attributes["pattern"]?.Value, RegexOptions.Singleline | RegexOptions.Compiled), NameToStyle(node.Attributes["name"]?.Value));
						else
							result.Styles.Add(new Regex(node.Attributes["pattern"]?.Value, RegexOptions.Compiled), NameToStyle(node.Attributes["name"]?.Value));
					}
				}

				XmlNode events = langDocument.DocumentElement["Events"];
				if (events != null) {
					result.Build = events["Build"]?.Attributes["action"]?.Value;
				}

				/*if (langInfo.Contains("on_run")) {
					result.RunCommand = (langInfo["on_run"] as TomlString).Value;
				}
				*/

				XmlNode identOn = langDocument.DocumentElement["IndentOn"];
				if (identOn != null) {
					result.IdentOn = Boolean.Parse(identOn.Attributes["value"]?.Value);
				}

				/* if (langInfo.Contains("actor")) {
					 String f = path + (langInfo["actor"] as TomlString).Value;

					 result.Actor = f;
				 }
				 */
				Languages.Add(result);
			}

			private static Style NameToStyle(String value) {
				switch (value) {
					case "string":
						return Settings.String;
					case "comment":
						return Settings.Comment;
					case "keyword":
						return Settings.Keyword;
					case "type":
						return Settings.Type;
					case "function":
						return Settings.Function;
					default:
						return null;
				}
			}

			private static void Theme() {
				String colorSchemeName = MainSettings.DocumentElement["EnabledTheme"]?.Attributes["name"]?.Value;

				if (colorSchemeName != null) {
					ThemeDocument = new XmlDocument();
					ThemeDocument.Load(ExecutablePath + "\\settings\\themes\\" + colorSchemeName + ".xml");

					String value = ThemeDocument.DocumentElement["Background"]?.Attributes["color"]?.Value;

					if (value != null) {
						BackgroundColor = ParseColor(value);
					}

					value = ThemeDocument.DocumentElement["Grid"]?.Attributes["color"]?.Value;
					if (value != null) {
						LinesColor = ParseColor(value);
					}

					value = ThemeDocument.DocumentElement["Foreground"]?.Attributes["color"]?.Value;
					if (value != null) {
						ForegroundColor = ParseColor(value);
					}

					value = ThemeDocument.DocumentElement["IconPack"]?.Attributes["path"]?.Value;
					if (value != null) {
						LoadIconPack(value);
					}

					XmlNode node = ThemeDocument.DocumentElement["AutocompleteMenu"];
					if (node != null) {
						LoadAutocompleteMenuSettings(node);
					}

					node = ThemeDocument.DocumentElement["TopMenu"];
					if (node != null) {
						LoadTopMenuSettings(node);
					}

					XmlNode syntax = ThemeDocument.DocumentElement["Syntax"];
					if (syntax != null) {

						XmlNode n = syntax["Keyword1"];
						if (n != null) {
							Keyword = ParseStyle(n);
						}

						n = syntax["Text1"];
						if (n != null) {
							String = ParseStyle(n);
						}

						n = syntax["Comment1"];
						if (n != null) {
							Comment = ParseStyle(n);
						}

						n = syntax["Class"];
						if (n != null) {
							Type = ParseStyle(n);
						}

						n = syntax["Function"];
						if (n != null) {
							Function = ParseStyle(n);
						}
					}

					XmlNode textBox = ThemeDocument.DocumentElement["TextBox"];
					if (textBox != null) {
						XmlNode n = textBox["ScrollBars"];
						if (n != null) {
							ScrollBars = n.Value == "on" ? VerticalScrollMode.ON : VerticalScrollMode.OFF;
						}
					}
				}
			}

			private static void LoadTopMenuSettings(XmlNode node) {
				String value = node["Background"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.Background = ParseColor(value);
				}

				value = node["SelectedBackground"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.SelectedBackground = ParseColor(value);
				}

				value = node["Foreground"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.ForegroundColor = ParseColor(value);
				}

				value = node["Borders"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.BordersColor = ParseColor(value);
				}

				value = node["ArrowColor"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.ArrowColor = ParseColor(value);
				}

				value = node["SelectedArrowColor"]?.Attributes["color"]?.Value;
				if (value != null) {
					TopMenu.ArrowSelectedColor = ParseColor(value);
				}
			}

			private static void LoadAutocompleteMenuSettings(XmlNode node) {
				String value = node["Background"]?.Attributes["color"]?.Value;
				if (value != null) {
					AutocompleteMenu.Background = ParseColor(value);
				}

				value = node["SelectedBackground"]?.Attributes["color"]?.Value;
				if (value != null) {
					AutocompleteMenu.SelectedBackground = ParseColor(value);
				}

				value = node["Foreground"]?.Attributes["color"]?.Value;
				if (value != null) {
					AutocompleteMenu.ForegroundColor = ParseColor(value);
				}

				value = node["Borders"]?.Attributes["color"]?.Value;
				if (value != null) {
					AutocompleteMenu.BordersColor = ParseColor(value);
				}

				value = node["ArrowColor"]?.Attributes["color"]?.Value;
				if (value != null) {
					AutocompleteMenu.ArrowColor = ParseColor(value);
				}

				XmlNode subNode = node["ToolTip"];
				if (subNode != null) {
					value = subNode["Background"]?.Attributes["color"]?.Value;
					if (value != null) {
						AutocompleteMenu.ToolTipBackColor = ParseColor(value);
					}

					value = subNode["Foreground"]?.Attributes["color"]?.Value;
					if (value != null) {
						AutocompleteMenu.ToolTipForeColor = ParseColor(value);
					}
				}
			}

			private static void LoadIconPack(String value) {
				XmlDocument document = new XmlDocument();
				document.Load(ExecutablePath + "\\settings\\themes\\" + value + "\\icons.xml");

				Int32 i = 0;

				XmlElement manager = document.DocumentElement["ProjectManager"];
				String folderIcon = manager["Folder"]?.Attributes["icon"]?.Value;
				if (folderIcon != null) {
					ProjectManagerImagesDictionary["[FOLDER]"] = i;
					i++;
					ProjectManagerImages.Add(Image.FromFile(ExecutablePath + $"\\settings\\themes\\{value}\\{folderIcon}"));
				}

				folderIcon = manager["File"]?.Attributes["icon"]?.Value;
				if (folderIcon != null) {
					ProjectManagerImagesDictionary["[FILE]"] = i;
					i++;
					ProjectManagerImages.Add(Image.FromFile(ExecutablePath + $"\\settings\\themes\\{value}\\{folderIcon}"));
				}

				foreach (XmlElement item in manager["Extensions"].ChildNodes) {
					folderIcon = item.Attributes["icon"]?.Value;
					String extenstions = item.Attributes["name"]?.Value;
					if (folderIcon != null && extenstions != null) {
						ProjectManagerImagesDictionary[extenstions] = i;
						i++;
						if (folderIcon.EndsWith(".png")) {
							ProjectManagerImages.Add(Image.FromFile(ExecutablePath + $"\\settings\\themes\\{value}\\{folderIcon}"));
						}
						else {
							ProjectManagerImages.Add(new Icon(ExecutablePath + $"\\settings\\themes\\{value}\\{folderIcon}"));
						}
					}
				}
			}

			public static Int32 GetImageIndex(String extension) {
				foreach (var i in ProjectManagerImagesDictionary) {
					String[] exts = i.Key.Split('|');
					if (exts.Contains(extension)) {
						return i.Value;
					}
				}

				return 1;
			}

			private static TextStyle ParseStyle(XmlNode node) {
				if (node == null) {
					return null;
				}

				String foreColor = node.Attributes["foreColor"]?.Value;
				String backColor = node.Attributes["backColor"]?.Value;
				String style = node.Attributes["textStyle"]?.Value;

				if (foreColor != null) {
					if (backColor != null) {
						return new TextStyle(new SolidBrush(ParseColor(foreColor)), new SolidBrush(ParseColor(backColor)), StringToStyle(style));
					}
					else {
						return new TextStyle(new SolidBrush(ParseColor(foreColor)), Brushes.Transparent, StringToStyle(style));
					}
				}

				return new TextStyle(new SolidBrush(ForegroundColor), new SolidBrush(BackgroundColor), FontStyle.Regular);
			}

			private static FontStyle StringToStyle(String str) {
				if (str == null) {
					return FontStyle.Regular;
				}

				switch (str.ToLower()) {
					case "bold":
						return FontStyle.Bold;
					case "italic":
						return FontStyle.Italic;
					case "regular":
						return FontStyle.Regular;
					case "strikeout":
						return FontStyle.Strikeout;
					case "underline":
						return FontStyle.Underline;
					default:
						return FontStyle.Regular;
				}
			}

			private static Color ParseColor(String input) {
				Match match = Regex.Match(input, @"(?<A>\d+)[;,]\s*(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?");

				if (match.Success) {
					return Color.FromArgb(Convert.ToInt32(match.Groups["A"].Value), Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
				}

				match = Regex.Match(input, @"(?<R>\d+)[;,]\s*(?<G>\d+)[,;]\s*(?<B>\d+)");

				if (match.Success) {
					return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
				}

				match = Regex.Match(input, @"#\s*(?<R>[0-9A-Fa-f]{1,2})\s*(?<G>[0-9A-Fa-f]{1,2})\s*(?<B>[0-9A-Fa-f]{1,2})");

				if (match.Success) {
					return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value, 16), Convert.ToInt32(match.Groups["G"].Value, 16), Convert.ToInt32(match.Groups["B"].Value, 16));
				}

				return Color.FromName(input);
			}

		}
	}
}
