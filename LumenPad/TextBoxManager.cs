using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public class TextBoxManager {
		public FastColoredTextBox TextBox { get; private set; }
		public AutocompleteMenu Menu { get; private set; }
		public Language Language { get; set; }

		public TextBoxManager(FastColoredTextBox textBox) {
			this.TextBox = textBox;

			this.Menu = new AutocompleteMenu(textBox) {
				ShowItemToolTips = true,
				AllowTabKey = true,
				MinFragmentLength = 1,
				AppearInterval = 50,
				RenderMode = ToolStripRenderMode.Professional,
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.FromArgb(200, 200, 200),
				SelectedColor = Color.FromArgb(112, 112, 112),
				ImageList = MainForm.Instance.imageList1,
				AlwaysShowTooltip = true
			};
		}

		internal void RebuildMenu() {
			if (this.Menu == null) {
				return;
			}

			this.Menu.Items.SetAutocompleteItems(this.Language.GetAutocompleteItems(this));
		}

		internal void OnTextChanged() {
			this.Language.OnTextChanged(this);
		}
	}
}
