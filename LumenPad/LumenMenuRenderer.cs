using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lumen.Studio {
	public class LumenMenuRenderer : ToolStripProfessionalRenderer {
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
			e.Graphics.DrawRectangle(new Pen(Settings.LinesColor), new Rectangle(0, 2, (Int32)e.Graphics.ClipBounds.Width-3, (Int32)e.Graphics.ClipBounds.Width - 1));
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			if (e.Item.Selected || e.Item.Pressed) {
				e.Graphics.Clear(Settings.LinesColor);
			} else {
				e.Graphics.Clear(Settings.BackgroundColor);
			}
		}

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e) {
			e.Graphics.Clear(Settings.BackgroundColor);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
			if (e.ToolStrip is MenuStrip) {
				return;
			}

			if (e.ToolStrip is ToolStripDropDown tsi 
				&& tsi.OwnerItem is ToolStripDropDownItem tsi2 
				&& tsi2.OwnerItem is ToolStripDropDownItem) {
				e.Graphics.DrawRectangle(new Pen(Settings.LinesColor), new Rectangle(e.AffectedBounds.X - 1, e.AffectedBounds.Y,
					e.AffectedBounds.Width - 1,
					e.AffectedBounds.Height - 2));
				return;
			}

			e.Graphics.DrawRectangle(new Pen(Settings.LinesColor), new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, 
				e.AffectedBounds.Width - 2, 
				e.AffectedBounds.Height - 2));
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
			e.ArrowColor = Settings.LinesColor;
			base.OnRenderArrow(e);
		}
	}
}