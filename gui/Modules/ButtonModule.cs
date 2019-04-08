using Lumen.Lang.Expressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class ButtonModule : Module {
        public ButtonModule() {
            this.name = "Visual.Button";

            this.SetField("setStyle", new LambdaFun((e, args) => {
                ButtonValue btn = e["btn"] as ButtonValue;
                Button obje = btn.value;
                IObject style = e["style"] as IObject;

                obje.FlatStyle = StyleModule.ToStyle(style, e);

                return btn;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("style"),
                    new NamePattern("btn"),
                }
            });

            this.SetField("defaultConstructor", new LambdaFun((ex, argsx) => {
                return new ButtonValue(new Button());
            }) {
                Arguments = new List<IPattern> {

                }
            });

			this.SetField("liftB", new LambdaFun((scope, args) => {
				if (!(scope["m"] is ButtonValue lbl)) {
					return scope["m"];
				}

				Fun f = scope["f"] as Fun;

				return f.Run(new Scope(scope), lbl);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});

			this.Derive(Prelude.Monad);
			this.Derive(Visual.Control);
        }
    }
}
