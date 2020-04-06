using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xmltools {
	public static class Intitializer {
		public static void Inititalize() {
			ToolStripItem xmlize = new ToolStripButton("Xmlize");
			xmlize.Click += (sender, e) => {
				Lumen.Studio.MainForm.MainTextBoxManager.TextBox.SelectedText = Lumen.Studio.MainForm.MainTextBoxManager.TextBox.SelectedText.Replace("&", "&amp;")
				.Replace("<", "&lt;").Replace(">", "&gt;")
				.Replace("\"", "&quot;");
			};

			ToolStripItem dexmlize = new ToolStripButton("Dexmlize");
			dexmlize.Click += (sender, e) => {
				Lumen.Studio.MainForm.MainTextBoxManager.TextBox.SelectedText = Lumen.Studio.MainForm.MainTextBoxManager.TextBox.SelectedText
				.Replace("&lt;", "<").Replace("&gt;", ">")
				.Replace("&quot;", "\"").Replace("&amp;", "&");
			};

			Lumen.Studio.MainForm.Instance.TextBoxContextMenu.Items.Add(xmlize);
			Lumen.Studio.MainForm.Instance.TextBoxContextMenu.Items.Add(dexmlize);

			Lumen.Studio.Language lang = Lumen.Studio.Settings.Languages.Find(l => l.Name == "XML");
			if(lang != null) {
				lang.OnChanged += () => {
					var d = new System.Xml.XmlDocument();
					Lumen.Studio.MainForm.Instance.Output.Clear();
					try {
						d.LoadXml(Lumen.Studio.MainForm.MainTextBoxManager.TextBox.Text);
						Lumen.Studio.MainForm.Instance.Output.Write("OK");
					} catch (System.Xml.XmlException e) {
						var rng = Lumen.Studio.MainForm.MainTextBoxManager.TextBox.GetRange(
							new FastColoredTextBoxNS.Place(e.LinePosition - 1, e.LineNumber - 1),
							new FastColoredTextBoxNS.Place(e.LinePosition, e.LineNumber-1));
						rng.SetStyle(Lumen.Studio.Settings.Error);
						Lumen.Studio.MainForm.Instance.Output.Write(e.Message);
					}
				};
			}

			// Solve a problem
			/*Lumen.Studio.MainForm.Instance.TextBoxContextMenu.Opening += (sender, e) => {
				if (!Lumen.Studio.MainForm.MainTextBoxManager.TextBox.Selection.IsEmpty) {
					xmlize.Visible = true;
					dexmlize.Visible = true;
				} else {
					xmlize.Visible = false;
					dexmlize.Visible = false;
				}
			};
			*/
			/*Lumen.Studio.MainForm.Instance.TextBoxContextMenu.Closing += (sender, e) => {
				xmlize.Visible = false;
				dexmlize.Visible = false;
			};*/
		}
	}
}
