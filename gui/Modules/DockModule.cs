using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class DockModule : Module {

        public static IType Bottom { get; private set; }
        public static IType Fill { get; private set; }
        public static IType Left { get; private set; }
        public static IType Right { get; private set; }
        public static IType Top { get; private set; }
        public static IType None { get; private set; }

        public DockModule() {
            this.Name = "Visual.Dock";

            Bottom = Helper.CreateConstructor("Visual.Dock.Bottom", this, new List<String>());
            Fill = Helper.CreateConstructor("Visual.Dock.Fill", this, new List<String>());
            Left = Helper.CreateConstructor("Visual.Dock.Left", this, new List<String>());
            Right = Helper.CreateConstructor("Visual.Dock.Right", this, new List<String>());
            None = Helper.CreateConstructor("Visual.Dock.None", this, new List<String>());
            Top = Helper.CreateConstructor("Visual.Dock.Top", this, new List<String>());

            this.SetMember("Bottom", Bottom);
            this.SetMember("Fill", Fill);
            this.SetMember("Left", Left);
            this.SetMember("Right", Right);
            this.SetMember("Top", Top);
            this.SetMember("None", None);
        }

        public static DockStyle ToStyle(IType obj, Scope s) {
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
