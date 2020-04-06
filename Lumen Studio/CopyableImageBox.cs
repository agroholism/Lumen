using System.ComponentModel;
using System.Windows.Forms;

namespace Lumen.Studio {
	public class CopyableImageBox : PictureBox, ICopyable {
		public CopyableImageBox() {
			this.Dock = DockStyle.Fill;
			this.BorderStyle = BorderStyle.None;
			this.WaitOnLoad = false;
			this.Visible = false;
			this.SizeMode = PictureBoxSizeMode.AutoSize;
		}

		public void Copy() {
			Clipboard.SetImage(this.Image);
		}
	}
}