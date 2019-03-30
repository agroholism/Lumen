using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lumen.Studio {
    public class LumenStudioForm : Form {
        private Boolean mouseDown;
        private Point lastLocation;

        public LumenStudioForm() {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected void FormMouseDown(Object sender, MouseEventArgs e) {
            this.mouseDown = true;
            this.lastLocation = e.Location;
        }

        protected void FormMouseMove(Object sender, MouseEventArgs e) {
            if (this.mouseDown) {
                this.Location = new Point(
                    (this.Location.X - this.lastLocation.X) + e.X, (this.Location.Y - this.lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        protected void FormMouseUp(Object sender, MouseEventArgs e) {
            this.mouseDown = false;
        }

        protected override void OnPaint(PaintEventArgs e) // you can safely omit this method if you want
{
            e.Graphics.FillRectangle(new SolidBrush(Settings.LinesColor), this.Top);
            e.Graphics.FillRectangle(new SolidBrush(Settings.LinesColor), this.Left);
            e.Graphics.FillRectangle(new SolidBrush(Settings.LinesColor), this.Right);
            e.Graphics.FillRectangle(new SolidBrush(Settings.LinesColor), this.Bottom);
        }

        private const Int32
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const Int32 _ = 10; // you can rename this variable if you like

        new Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }

        new Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }

        new Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }

        new Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }
        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message) {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (this.TopLeft.Contains(cursor))
                    message.Result = (IntPtr)HTTOPLEFT;
                else if (this.TopRight.Contains(cursor))
                    message.Result = (IntPtr)HTTOPRIGHT;
                else if (this.BottomLeft.Contains(cursor))
                    message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (this.BottomRight.Contains(cursor))
                    message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (this.Top.Contains(cursor))
                    message.Result = (IntPtr)HTTOP;
                else if (this.Left.Contains(cursor))
                    message.Result = (IntPtr)HTLEFT;
                else if (this.Right.Contains(cursor))
                    message.Result = (IntPtr)HTRIGHT;
                else if (this.Bottom.Contains(cursor))
                    message.Result = (IntPtr)HTBOTTOM;
            }
        }
    }
}