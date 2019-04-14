using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public class TextBoxManager {
        public FastColoredTextBox TextBox { get; private set; }
        public AutocompleteMenu Menu { get; private set; }
        public Language Language { get; set; }

        public Boolean ChangesSaved { get; set; }

        public TextBoxManager(FastColoredTextBox textBox) {
            this.TextBox = textBox;

            this.Menu = new AutocompleteMenu(textBox) {
                ShowItemToolTips = true,
                AllowTabKey = true,
                MinFragmentLength = 1,
                AppearInterval = 50,
                RenderMode = ToolStripRenderMode.Professional,
                BackColor = Settings.BackgroundColor,
                ForeColor = Settings.ForegroundColor,
                SelectedColor = Color.FromArgb(112, 112, 112),
                ImageList = MainForm.Instance.imageList1,
                AlwaysShowTooltip = true,
				BorderColor = Settings.LinesColor,
				Font = Settings.DefaultInterfaceFont,
				DropShadowEnabled = false
            };
        }

        internal void RebuildMenu() {
            if (this.Menu == null) {
                return;
            }

            List<AutocompleteItem> items = this.Language.GetAutocompleteItems(this);

            if (items != null) {
                this.Menu.Items.SetAutocompleteItems(items);
            }
        }

        internal void OnTextChanged(TextChangedEventArgs eventArgs) {
            if (ProjectManager.AllowRegistrationChanges) {
                this.ChangesSaved = false;
            }

            this.Language.OnTextChanged(this, eventArgs.ChangedRange);
        }
    }
}
