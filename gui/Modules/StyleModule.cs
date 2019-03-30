using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class StyleModule : Module {

        public static IObject Flat { get; private set; }
        public static IObject Popup { get; private set; }
        public static IObject Standard { get; private set; }
        public static IObject System { get; private set; }

        public StyleModule() {
            this.name = "Visual.Style";

            Flat = Prelude.CreateConstructor("Visual.Style.Flat", this, new List<String>());
            Popup = Prelude.CreateConstructor("Visual.Style.Popup", this, new List<String>());
            Standard = Prelude.CreateConstructor("Visual.Style.Standard", this, new List<String>());
            System = Prelude.CreateConstructor("Visual.Style.System", this, new List<String>());

            this.SetField("Flat", Flat);
            this.SetField("Popup", Popup);
            this.SetField("Standard", Standard);
            this.SetField("System", System);
        }

        public static FlatStyle ToStyle(IObject obj, Scope s) {
            if(obj == Flat) {
                return FlatStyle.Flat;
            }

            if (obj == Popup) {
                return FlatStyle.Popup;
            }

            if (obj == Standard) {
                return FlatStyle.Standard;
            }

            if (obj == System) {
                return FlatStyle.System;
            }

            return FlatStyle.Standard;
        }
    }
}
