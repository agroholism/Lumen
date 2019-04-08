using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public partial class RegexTester : LumenStudioForm {
		public RegexTester() {
			InitializeComponent();

			this.BackColor = Settings.BackgroundColor;
			this.ForeColor = Settings.BackgroundColor;

			this.fastColoredTextBox1.BackColor = Settings.BackgroundColor;
			this.fastColoredTextBox1.ForeColor = Settings.ForegroundColor;
			this.fastColoredTextBox2.BackColor = Settings.BackgroundColor;
			this.fastColoredTextBox2.ForeColor = Settings.ForegroundColor;

			this.label1.BackColor = Settings.BackgroundColor;
			this.label1.ForeColor = Settings.ForegroundColor;

			Style s = new TextStyle(new SolidBrush(Color.White), new SolidBrush(Color.BlueViolet), FontStyle.Regular);

			fastColoredTextBox1.TextChanged += (sender, e) => {
				try {
					if(s != null) {
						this.fastColoredTextBox2.Range.ClearStyle(s);
					}
					this.fastColoredTextBox2.Range.SetStyle(s, new Regex(fastColoredTextBox1.Text));

				} catch (Exception ex) {
					this.label1.Text = ex.Message;
				}
			};
		}
	}
}
