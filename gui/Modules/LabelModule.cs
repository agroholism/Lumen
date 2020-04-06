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
			this.Name = "Visual.Label";

			this.SetMember("setStyle", new LambdaFun((e, args) => {
				LabelValue btn = e["lbl"] as LabelValue;
				Label obje = btn.value;
				IType style = e["style"] as IType;

				obje.FlatStyle = StyleModule.ToStyle(style, e);

				return btn;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("style"),
					new NamePattern("lbl"),
				}
			});

			this.SetMember("defaultConstructor", new LambdaFun((ex, argsx) => {
				return new LabelValue(new Label());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("()")
				}
			});

			this.SetMember("liftB", new LambdaFun((scope, args) => {
				if (!(scope["m"] is LabelValue lbl)) {
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
