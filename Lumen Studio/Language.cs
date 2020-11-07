using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

using Lumen.Lang;
using Regex = System.Text.RegularExpressions.Regex;

namespace Lumen.Studio {
	public class Language {
		public String Name { get; set; }
		public List<String> Extensions { get; set; } = new List<String>();
		public Dictionary<Regex, Style> Styles = new Dictionary<Regex, Style>();
		public List<(String, String)> Folding { get; set; } = new List<(String, String)>();

		public String Build { get; set; }
		public Module Fn { get; internal set; }
		public String Actor { get; set; }
		public Boolean IdentOn;

		public event Action OnCustomize;
		public event Action OnChanged;

		internal virtual List<AutocompleteItem> GetAutocompleteItems(TextBoxManager manager) {
			if (this.Fn != null && this.Fn.TryGetMember("get_autocomplete_items", out Value fn)) {
				Value res = (fn as Fun).Call(new Scope());
				List<AutocompleteItem> result = new List<AutocompleteItem>();

				foreach (Value i in res.ToList(null)) {
					if (i is Lang.MutableArray) {
						List<Value> lst = i.ToList(null);
						AutocompleteItem aci = new AutocompleteItem {
							Text = lst[0].ToString(),
							ImageIndex = (Int32)lst[1].ToDouble(null),
							MenuText = lst[2].ToString(),
							ToolTipTitle = lst[3].ToString(),
							ToolTipText = lst[4].ToString()
						};

						result.Add(aci);
					}
				}

				return result;
			}

			return null;
		}

		internal virtual void OnTextChanged(TextBoxManager manager, FastColoredTextBoxNS.Range rng) {
			FastColoredTextBox textBox = manager.TextBox;

			textBox.Range.ClearFoldingMarkers();

			textBox.Range.ClearStyle(StyleIndex.All);

			foreach ((String, String) i in this.Folding) {
				textBox.Range.SetFoldingMarkers(i.Item1, i.Item2);
			}

			manager.TextBox.Range.ClearStyle(Settings.SelectedIdentifier);

			if (this.IdentOn) {
				this.Ident(manager.TextBox);
			}

			rng.ClearStyle(StyleIndex.All);

			foreach (KeyValuePair<Regex, Style> i in this.Styles) {
				textBox.Range.SetStyle(i.Value, i.Key);
			}


			foreach (Bookmark i in textBox.Bookmarks) {
				textBox.GetLine(i.LineIndex).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type);
				textBox.GetLine(i.LineIndex).SetStyle(Settings.UnactiveBreakpoint);
			}

			/*if (this.Fn != null && this.Fn.TryGetField("on_text_changed", out var fn)) {
                (fn as Fun).Run(new Scope(), new Interop.RangeI(rng));
            }*/

			this.OnChanged?.Invoke();
			manager.RebuildMenu();
		}

		internal virtual void OnSelectedChanged(TextBoxManager manager) {

		}

		protected void Ident(FastColoredTextBox textBox) {
			Int32 currentIndent = 0;
			Int32 lastNonEmptyLine = 0;

			for (Int32 i = 0; i < textBox.LinesCount; i++) {
				Line line = textBox[i];

				Int32 spacesCount = line.StartSpacesCount;

				if (spacesCount == line.Count) {
					continue;
				}

				if (currentIndent < spacesCount) {
					textBox[lastNonEmptyLine].FoldingStartMarker = "m" + currentIndent;
				}
				else if (currentIndent > spacesCount) {
					textBox[lastNonEmptyLine].FoldingEndMarker = "m" + spacesCount;
				}

				currentIndent = spacesCount;
				lastNonEmptyLine = i;
			}
		}

		internal void Customize() {
			OnCustomize?.Invoke();
		}
	}

	public enum Images {
		FUNCTION,
		TYPE,
		VARIABLE,
		SUBTYPE,
		MODULE,
		KEYWORD
	}

	class Mod : AutocompleteItem {
		public Mod(String s) : base(s) { }

		public Mod(String text, Int32 imageIndex) : base(text, imageIndex) {
		}

		public Mod(String text, Int32 imageIndex, String menuText) : base(text, imageIndex, menuText) {
		}

		public Mod(String text, Int32 imageIndex, String menuText, String toolTipTitle, String toolTipText) : base(text, imageIndex, menuText, toolTipTitle, toolTipText) {
		}

		public override CompareResult Compare(String fragmentText) {
			Int32 i = fragmentText.LastIndexOf('.');
			if (i < 0 || this.LastPartContainsPoints(this.Text, fragmentText)) {
				return CompareResult.Hidden;
			}

			if (this.Text.Contains(".")) {
				if (!this.Text.StartsWith(fragmentText) /*&& !this.Extended(fragmentText)*/) {
					return CompareResult.Hidden;
				}
				return CompareResult.Visible;
			}

			String lastPart = fragmentText.Substring(i + 1);
			if (lastPart == "") {
				return CompareResult.Visible;
			}

			if (this.Text.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase)  /*|| this.Extended(lastPart)*/) {
				return CompareResult.VisibleAndSelected;
			}

			return CompareResult.Hidden;
		}

		public Boolean LastPartContainsPoints(String s, String fragment) {
			String[] ss = s.Split('.');

			Int32 count = fragment.Count(i => i == '.');

			return ss.Length - count > 1;
		}

		public override String GetTextForReplace() {
			return this.Text;
		}
	}
}
