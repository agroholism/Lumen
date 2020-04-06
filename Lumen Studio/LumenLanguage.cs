using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public class LumenLanguage : Language {
		internal LumenLanguage() {
			this.Name = "Lumen";
			this.Extensions = new List<String> { ".lm" };
			this.IdentOn = true;
			this.Styles.Add(new Regex("<\\[.*?\\]>", RegexOptions.Compiled | RegexOptions.Singleline), Settings.Interface);
			this.Styles.Add(new Regex("(\"\")|\".*?[^\\\\]\"", RegexOptions.Compiled | RegexOptions.Singleline), Settings.String);
			this.Styles.Add(new Regex("//.*"), Settings.Comment);
			this.Styles.Add(new Regex("@[\\w\\d\\.]+"), Settings.Keyword);
			this.Styles.Add(new Regex("'\\w+"), Settings.Keyword);
			this.Styles.Add(new Regex("\\b(yield|alias|variant|class|extension|ref|use|import|rec|implements|tailrec|as|type|match|next|break|not|and|is|let|or|xor|if|else|for|in|while|from|return|true|false)\\b"), Settings.Keyword);
			this.Styles.Add(new Regex("\\b(Number|Unit|Ref|Fail|Success|Result|Stream|Text|Some|None|Option|Array|Boolean|Map|List|Function|Pair)\\b"), Settings.Type);
			this.Styles.Add(new Regex("\\b(Functor|Context|Format|Monad|Applicative|Alternative|Ord|Collection)\\b"), Settings.Interface);
		}

		internal override void OnTextChanged(TextBoxManager manager, Range rng) {
			FastColoredTextBox textBox = manager.TextBox;

			textBox.Range.ClearFoldingMarkers();
			manager.TextBox.Range.ClearStyle(Settings.SelectedIdentifier);
			foreach ((String, String) i in this.Folding) {
				textBox.Range.SetFoldingMarkers(i.Item1, i.Item2);
			}

			if (this.IdentOn) {
				this.Ident(manager.TextBox);
			}

			try {
				this.RefsAndCommentsFolding(manager.TextBox);
			}
			catch { }

			rng.ClearStyle(StyleIndex.ALL);

			foreach (KeyValuePair<Regex, Style> i in this.Styles) {
				if (i.Key.Options.HasFlag(RegexOptions.Singleline)) {
					manager.TextBox.Range.SetStyle(i.Value, i.Key); // or visible range?
				}
				else {
					rng.SetStyle(i.Value, i.Key);
				}
			}

			foreach (String i in textBox.Lines) {
				Match match = Regex.Match(i, @"\s*type\s+(?<range>[A-Za-z0-9]+)\s*");

				if (match.Success) {
					textBox.Range.SetStyle(Settings.Type, $"\\b{match.Groups["range"].Value}\\b");
				}
				else {
					match = Regex.Match(i, @"\s*class\s+(?<range>[A-Za-z0-9]+)\s*");
					if (match.Success) {
						textBox.Range.SetStyle(Settings.Interface, $"\\b{match.Groups["range"].Value}\\b");
					}
					else {
						match = Regex.Match(i, @"\s*let\s+(?<range>[A-Za-z0-9]+)\s*=\s*ref[^\w]");
						if (match.Success) {
							textBox.Range.SetStyle(Settings.Message, $"\\b{match.Groups["range"].Value}\\b");
						}
					}
				}
			}

			foreach (Bookmark i in textBox.Bookmarks) {
				textBox.GetLine(i.LineIndex).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.Keyword, Settings.String);
				textBox.GetLine(i.LineIndex).SetStyle(Settings.UnactiveBreakpoint);
			}

			manager.RebuildMenu();
		}

		private void RefsAndCommentsFolding(FastColoredTextBox textBox) {
			for (Int32 i = 0; i < textBox.LinesCount; i++) {
				Line line = textBox[i];

				if (Regex.IsMatch(line.Text, "import.*")) {
					Int32 importSectionBegin = i;
					i++;

					Int32 lastImportPosition = 0;
					Boolean isMatched = true;
					while (i < textBox.LinesCount - 1 && (isMatched = Regex.IsMatch(textBox[i].Text, "import.*")) || String.IsNullOrWhiteSpace(textBox[i].Text.Trim(new[] { '\r', '\n', ' ' }))) {
						if (isMatched) {
							lastImportPosition = i;
						}
						i++;
					}

					Int32 importSectionEnd = lastImportPosition;

					if (importSectionEnd - importSectionBegin > 0) {
						textBox[importSectionBegin].FoldingStartMarker = "m" + importSectionBegin;
						textBox[importSectionEnd].FoldingEndMarker = "m" + importSectionBegin;
					}
				}
				else if (Regex.IsMatch(line.Text, "//.*", RegexOptions.Multiline)) {
					Int32 commentBegin = i;
					i++;
					try {
						while (Regex.IsMatch(textBox[i].Text, "^//.*")) {
							i++;
						}
					}
					catch {

					}
					Int32 commentEnd = i - 1;

					if (commentEnd - commentBegin > 0) {
						textBox[commentBegin].FoldingStartMarker = "m" + commentBegin;
						textBox[commentEnd].FoldingEndMarker = "m" + commentBegin;
					}
				}
			}
		}

		internal override List<AutocompleteItem> GetAutocompleteItems(TextBoxManager manager) {
			List<AutocompleteItem> items = new List<AutocompleteItem> {
				new AutocompleteItem("class", (Int32)Images.FUNCTION, "class", "class", "class keyword"),
				new AutocompleteItem("type", (Int32)Images.FUNCTION, "type", "type", "type keyword"),
				new AutocompleteItem("where", (Int32)Images.FUNCTION, "where", "where", "where keyword"),
				new AutocompleteItem("deriving", (Int32)Images.FUNCTION, "deriving", "deriving", "deriving keyword"),
				new AutocompleteItem("import", (Int32)Images.FUNCTION, "import", "import", "import keyword"),
				new AutocompleteItem("let", (Int32)Images.FUNCTION, "let", "let", "let keyword"),
				new AutocompleteItem("constructor", (Int32)Images.FUNCTION, "constructor", "constructor", "constructor keyword"),
			};

			String RestoreFunctionHeader(String name, Lang.Fun fun) {
				String header = "\0let \0" + name;

				if (fun.Arguments.Count == 0) {
					header += "()";
				}
				else {
					foreach (var i in fun.Arguments) {
						header += " " + i.ToString();
					}
				}

				return header;
			}

			List<Lang.Value> memory = new List<Lang.Value>();

			String RestoreConstructorHeader(String name, Lang.Constructor fun) {
				String header = name;

				foreach (var i in fun.Arguments) {
					header += " " + i.ToString();
				}

				return header;
			}

			AutocompleteItem Process(String name, String baseName, Lang.Value value) {
				memory.Add(value);
				if (value is Lang.Fun fun) {
					if (baseName == null) {
						return new AutocompleteItem(name, (Int32)Images.FUNCTION, name, RestoreFunctionHeader(name, fun), "this is built-in fun");
					}
					else {
						return new Mod(baseName + "." + name, (Int32)Images.FUNCTION, name, RestoreFunctionHeader(name, fun), "this is built-in dfun");
					}
				}

				if (value is Lang.Constructor ctor) {
					return new AutocompleteItem(name, (Int32)Images.SUBTYPE, name, RestoreConstructorHeader(name, ctor), "");
				}

				if (value is Lang.SingletonConstructor sctor) {
					return new AutocompleteItem(name, (Int32)Images.SUBTYPE, name, name, "");
				}

				if (value is Lang.Module module) {
					foreach (var i in module.Members) {
						if (!memory.Contains(i.Value)) {
							items.Add(Process(i.Key, name, i.Value));
						}
					}

					foreach (var i in module.Mixins) {
						foreach (var j in i.Members) {
							items.Add(Process(j.Key, name, j.Value));
						}
					}

					return new AutocompleteItem(name, (Int32)Images.SUBTYPE, name, "\0module \0" + name, "\t\t");
				}

				return new AutocompleteItem(name, 0, name, name, "\t\t");
			}

			memory.Add(Lang.Prelude.Instance);
			foreach (var i in Lumen.Lang.Prelude.Instance.Members) {
				if (!memory.Contains(i.Value))
					items.Add(Process(i.Key, null, i.Value));
			}

			/*
			 * String f(String s) {
				String[] ss = s.Split('.');

				return ss[ss.Length - 1];
			}
			 * try {
				  var res = ldoc.Program.Parse(manager.TextBox.Text);

				  foreach (var i in res) {
					  if(i.Type == ldoc.Program.Type.FUNCTION) {
						  if (i.Name.Contains(".")) {
							  items.Add(new Mod(i.Name, (Int32)Images.FUNCTION, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
						  } else {
							  items.Add(new AutocompleteItem(i.Name, (Int32)Images.FUNCTION, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
						  }
					  } else if (i.Type == ldoc.Program.Type.MODULE) {
						  if (i.Name.Contains(".")) {
							  items.Add(new Mod(i.Name, (Int32)Images.MODULE, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
						  } else {
							  items.Add(new AutocompleteItem(i.Name, (Int32)Images.MODULE, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
						  }
					  } else if (i.Type == ldoc.Program.Type.CONSTRUCTOR) {
						  if (i.Name.Contains(".")) {
							  items.Add(new Mod(i.Name, (Int32)Images.SUBTYPE, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
						  } else {
							  items.Add(new AutocompleteItem(i.Name, (Int32)Images.SUBTYPE, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
						  }
					  }
				  }
			  } catch { }*/

			return items;
		}
	}
}


