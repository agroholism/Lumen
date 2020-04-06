using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lumen.Studio {
    public class LumenMenuRenderer : ToolStripProfessionalRenderer {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color ArrowColor { get; set; }
        public Color ArrowSelectedColor { get; set; }

        public LumenMenuRenderer() {
            this.BackColor = Settings.BackgroundColor;
            this.ForeColor = Settings.ForegroundColor;
            this.BorderColor = Settings.LinesColor;
            this.SelectedBackColor = Settings.LinesColor;
            this.ArrowColor = Settings.LinesColor;
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
            // here some bug with e.Graphics.ClipBounds.Width
            e.Graphics.DrawLine(new Pen(this.BorderColor), 0, 2, e.Graphics.ClipBounds.Width - 1, 2);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            if (e.Item.Selected || e.Item.Pressed) {
                e.Graphics.Clear(this.SelectedBackColor);
            } else {
                e.Graphics.Clear(this.BackColor);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e) {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            if (e.ToolStrip is MenuStrip) {
                return;
            }

            e.Graphics.DrawRectangle(new Pen(this.BorderColor), new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, 
                e.AffectedBounds.Width - 1, 
                e.AffectedBounds.Height - 1));
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
            if (e.Item.Selected || e.Item.Pressed) {
                e.ArrowColor = this.ArrowSelectedColor;
            }
            else {
                e.ArrowColor = this.ArrowColor;
            }

            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
            e.TextColor = this.ForeColor;
            base.OnRenderItemText(e);
        }
    }
}