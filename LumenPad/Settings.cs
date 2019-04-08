using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using FastColoredTextBoxNS;
using Lumen.Lang;

namespace Lumen.Studio {
    public static class Settings {
        public static String DefaultFileName { get; private set; } = "default.lm";

        public static Style Type { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
        public static Style Error { get; set; } = new WavyLineStyle(255, Color.Red);
        public static Style String { get; set; } = new TextStyle(new SolidBrush(Color.OrangeRed), Brushes.Transparent, FontStyle.Regular);
        public static Style Comment { get; set; } = new TextStyle(new SolidBrush(Color.Green), Brushes.Transparent, FontStyle.Regular);
        public static Style Keyword { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);
        public static Style Function { get; set; } = new TextStyle(new SolidBrush(Color.Blue), Brushes.Transparent, FontStyle.Regular);

        public static Color BackgroundColor { get; set; } = Color.White;
        public static Color LinesColor { get; set; } = Color.Silver;
        public static Color ForegroundColor { get; set; } = Color.Black;

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

        internal static void Load() {
            MainSettings = new XmlDocument();
            MainSettings.Load("settings\\main.xml");

            LoadLastData();
            Theme();
            LoadLanguages();
            LoadProjectTypes();
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

                    if (System.IO.File.Exists("settings\\languages\\" + name + ".xml")) {
                        LoadLanguage("settings\\languages\\", name);
                    } else {
                        LoadLanguage("settings\\languages\\" + name + "\\", name);
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
            if(styles != null) {
                foreach (XmlNode node in styles.ChildNodes) {
					if(node.Attributes["options"]?.Value == "sinln")
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
            String colorSchemeName = MainSettings.DocumentElement["Theme"]?.Attributes["name"]?.Value;

            if (colorSchemeName != null) {
                ThemeDocument = new XmlDocument();
                ThemeDocument.Load("settings\\themes\\" + colorSchemeName + ".xml");

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
            }
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
                } else {
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
            if (Regex.IsMatch(input, @"(?<A>\d+)[;,]\s*(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?")) {
                Match match = Regex.Match(input, @"(?<A>\d+)[;,]\s*(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?");
                return Color.FromArgb(Convert.ToInt32(match.Groups["A"].Value), Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
            } else if (Regex.IsMatch(input, @"(?<R>\d+)[;,]\s*(?<G>\d+)[;,]\s*(?<B>\d+)[,;]?")) {
                Match match = Regex.Match(input, @"(?<R>\d+)[;,]\s*(?<G>\d+)[,;]\s*(?<B>\d+)");
                return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value), Convert.ToInt32(match.Groups["G"].Value), Convert.ToInt32(match.Groups["B"].Value));
            } else if (Regex.IsMatch(input, @"#\s*(?<R>[0-9A-Fa-f]{1,2})\s*(?<G>[0-9A-Fa-f]{1,2})\s*(?<B>[0-9A-Fa-f]{1,2})")) {
                Match match = Regex.Match(input, @"#\s*(?<R>[0-9A-Fa-f]{1,2})\s*(?<G>[0-9A-Fa-f]{1,2})\s*(?<B>[0-9A-Fa-f]{1,2})");
                return Color.FromArgb(Convert.ToInt32(match.Groups["R"].Value, 16), Convert.ToInt32(match.Groups["G"].Value, 16), Convert.ToInt32(match.Groups["B"].Value, 16));
            }

            return Color.FromName(input);
        }
    }
}
