using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class DockModule : Module {

        public static IObject Bottom { get; private set; }
        public static IObject Fill { get; private set; }
        public static IObject Left { get; private set; }
        public static IObject Right { get; private set; }
        public static IObject Top { get; private set; }
        public static IObject None { get; private set; }

        public DockModule() {
            this.name = "Visual.Dock";

            Bottom = Prelude.CreateConstructor("Visual.Dock.Bottom", this, new List<String>());
            Fill = Prelude.CreateConstructor("Visual.Dock.Fill", this, new List<String>());
            Left = Prelude.CreateConstructor("Visual.Dock.Left", this, new List<String>());
            Right = Prelude.CreateConstructor("Visual.Dock.Right", this, new List<String>());
            None = Prelude.CreateConstructor("Visual.Dock.None", this, new List<String>());
            Top = Prelude.CreateConstructor("Visual.Dock.Top", this, new List<String>());

            this.SetField("Bottom", Bottom);
            this.SetField("Fill", Fill);
            this.SetField("Left", Left);
            this.SetField("Right", Right);
            this.SetField("Top", Top);
            this.SetField("None", None);
        }

        public static DockStyle ToStyle(IObject obj, Scope s) {
            if(obj == Bottom) {
                return DockStyle.Bottom;
            }

            if (obj == Fill) {
                return DockStyle.Fill;
            }

            if (obj == Left) {
                return DockStyle.Left;
            }

            if (obj == Right) {
                return DockStyle.Right;
            }

            if (obj == Top) {
                return DockStyle.Top;
            }

            if (obj == None) {
                return DockStyle.None;
            }

            return DockStyle.None;
        }
    }
}
