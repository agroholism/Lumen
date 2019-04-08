using Lumen.Lang.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class FormModule : Module {
        public FormModule() {
            this.name = "Visual.Form";

            this.SetField("defaultConstructor", new LambdaFun((ex, argsx) => {
                return new FormValue(new Form());
            }) {
                Arguments = new List<IPattern> {

                }
            });

            this.SetField("close", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("this") as FormValue).value;
                item.Close();
                return ex.Get("this");
            }) {
                Arguments = Const.This
            });

            this.SetField("show", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("this") as FormValue).value;
                Task<DialogResult> task = Task.Factory.StartNew(() => item.ShowDialog());
                return new LambdaFun((e, args) => {
                    task.Wait();
                    return ex.Get("this");
                });
            }) {
                Arguments = Const.This
            });

            this.SetField("showDialog", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("this") as FormValue).value;
                item.ShowDialog();
                return ex.Get("this");
            }) {
                Arguments = Const.This
            });

			this.SetField("liftB", new LambdaFun((scope, args) => {
				if (!(scope["m"] is FormValue lbl)) {
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
