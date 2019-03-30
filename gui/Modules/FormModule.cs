using Lumen.Lang.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class FormModule : Module {
        public FormModule() {
            this.name = "Visual.Form";

            this.SetField("addControl", new LambdaFun((e, args) => {
                FormValue form = e["form"] as FormValue;
                Form obje = form.value;

                VControl btn = e["ctrl"] as VControl;

                obje.Controls.Add(btn.Control);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("ctrl"),
                    new NamePattern("form")
                }
            });

            this.SetField("create", new LambdaFun((ex, argsx) => {
                return new FormValue(new Form());
            }) {
                Arguments = new List<IPattern> {

                }
            });

            this.SetField("close", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("this") as FormValue).value;
                item.Close();
                return Const.UNIT;
            }) {
                Arguments = Const.This
            });

            this.SetField("show", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("form") as FormValue).value;
                Task<DialogResult> task = Task.Factory.StartNew(() => item.ShowDialog());
                return new LambdaFun((e, args) => {
                    task.Wait();
                    return Const.UNIT;
                });
            }) {
                Arguments = Const.This
            });

            this.SetField("showDialog", new LambdaFun((ex, argsx) => {
                Form item = (ex.Get("this") as FormValue).value;
                item.ShowDialog();
                return Const.UNIT;
            }) {
                Arguments = Const.This
            });

            this.Derive(Visual.Control);
        }
    }
}
