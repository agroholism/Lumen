using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class LabelModule : Module {
        public LabelModule() {
            this.name = "Visual.Label";

            this.SetField("setStyle", new LambdaFun((e, args) => {
                LabelValue btn = e["lbl"] as LabelValue;
                Label obje = btn.value;
                IObject style = e["style"] as IObject;

                obje.FlatStyle = StyleModule.ToStyle(style, e);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("style"),
                    new NamePattern("lbl"),
                }
            });

            this.SetField("create", new LambdaFun((ex, argsx) => {
                return new LabelValue(new Label());
            }) {
                Arguments = new List<IPattern> {
                    
                }
            });

            this.Derive(Visual.Control);
        }
    }
}
