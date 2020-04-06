using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace EuIDE {
	public class TextBoxManager {
        public FastColoredTextBox TextBox { get; private set; }
        public AutocompleteMenu Menu { get; private set; }
		
        public Boolean ChangesSaved { get; set; }

        public TextBoxManager(FastColoredTextBox textBox) {
            this.TextBox = textBox;

			this.TextBox.TextChanged += this.OnTextChanged;

			this.Menu = new AutocompleteMenu(textBox) {
				ShowItemToolTips = true,
				AllowTabKey = true,
				MinFragmentLength = 1,
				AppearInterval = 50,
				RenderMode = ToolStripRenderMode.Professional,
				/*ImageList = MainForm.Instance.imageList1,*/
				AlwaysShowTooltip = true,
				/*Font = Settings.DefaultInterfaceFont,*/
				DropShadowEnabled = false,
				Renderer = new MenuRenderer(),
			};

			this.Menu.Items.SetAutocompleteItems(new List<String> {
				"atom", "list", "text", "bool",
				"mutable_list", "type", "contract",
				"function", "constant", "return",
				"if", "elif", "else", "end", "print",
				"object", "iterator", "nil",
				"sequence", "yield", "integer", "after", "before", "override"
			});
		}

        internal void RebuildMenu() {
            if (this.Menu == null) {
                return;
            }

           /* List<AutocompleteItem> items = this.Language.GetAutocompleteItems(this);

            if (items != null) {
                this.Menu.Items.SetAutocompleteItems(items);
            }*/
        }

        internal void OnTextChanged(Object sender, TextChangedEventArgs eventArgs) {
			FastColoredTextBox textBox = this.TextBox;

			textBox.Range.ClearFoldingMarkers();
			
			/*foreach ((String, String) i in this.Folding) {
				textBox.Range.SetFoldingMarkers(i.Item1, i.Item2);
			}*/

			eventArgs.ChangedRange.ClearStyle(StyleIndex.ALL);

			foreach (KeyValuePair<Regex, Style> i in Params.Styles) {
				if (i.Key.Options.HasFlag(RegexOptions.Singleline)) {
					this.TextBox.Range.SetStyle(i.Value, i.Key); // or visible range?
				}
				else {
					eventArgs.ChangedRange.SetStyle(i.Value, i.Key);
				}
			}
/*
			foreach (Bookmark i in textBox.Bookmarks) {
				textBox.GetLine(i.LineIndex).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.Keyword, Settings.String);
				textBox.GetLine(i.LineIndex).SetStyle(Settings.UnactiveBreakpoint);
			}
			*/
			//manager.RebuildMenu();
		}
    }
}
