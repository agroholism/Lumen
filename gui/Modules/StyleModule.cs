using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class StyleModule : Module {

        public static IType Flat { get; private set; }
        public static IType Popup { get; private set; }
        public static IType Standard { get; private set; }
        public static IType System { get; private set; }

        public StyleModule() {
            this.Name = "Visual.Style";

            Flat = Helper.CreateConstructor("Visual.Style.Flat", this, new List<String>());
            Popup = Helper.CreateConstructor("Visual.Style.Popup", this, new List<String>());
            Standard = Helper.CreateConstructor("Visual.Style.Standard", this, new List<String>());
            System = Helper.CreateConstructor("Visual.Style.System", this, new List<String>());

            this.SetMember("Flat", Flat);
            this.SetMember("Popup", Popup);
            this.SetMember("Standard", Standard);
            this.SetMember("System", System);
        }

        public static FlatStyle ToStyle(IType obj, Scope s) {
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
