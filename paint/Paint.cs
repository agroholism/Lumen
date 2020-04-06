using System.Drawing;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Lumen.Lang.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace Lumen.Lang.Libraries.Paint {
	public class PaintModule : Module {
		private static Form mainForm;
		private static PictureBox pictureBox;
		private static Graphics graphics;

		public PaintModule() {
			this.Name = "Paint";

			mainForm = new Form();
			pictureBox = new PictureBox {
				Dock = DockStyle.Fill
			};
			mainForm.Controls.Add(pictureBox);

			this.SetMember("show", new LambdaFun((scope, args) => {
				Task.Factory.StartNew((() => mainForm.ShowDialog()));
				graphics = mainForm.CreateGraphics();
				graphics.Clear(Color.White);
				return Const.UNIT;
			}) {
				Arguments = new List<Expressions.IPattern> {
					
				}
			});

			this.SetMember("line", new LambdaFun((e, args) => {
				List<Int32> color = e["color"].ToStream(e).Take(3).Select(i => i.ToInt(e)).ToList();
				List<Int32> point1 = e["point1"].ToStream(e).Take(2).Select(i => i.ToInt(e)).ToList();
				List<Int32> point2 = e["point2"].ToStream(e).Take(2).Select(i => i.ToInt(e)).ToList();

				graphics.DrawLine(
					new Pen(Color.FromArgb(color[0], color[1], color[2])), 
					new Point(point1[0], point1[1]), 
					new Point(point2[0], point2[1]));

				return Const.UNIT;
			}) {
				Arguments= new List<IPattern> {
					new NamePattern("color"),
					new NamePattern("point1"),
					new NamePattern("point2")
				}  
			});
		}
	}
}
