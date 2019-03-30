using Lumen.Lang;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class ControlTC : TypeClass {
        public ControlTC() {
            this.name = "Visual.Control";

            this.SetField("addOnClick", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;

                Fun fun = scope["f"] as Fun;
                control.Click += (sender, obj) => fun.Run(new Scope(scope.parent), vcontrol);
                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("ctrl"),
                }
            });

            this.SetField("setBackColor", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                IObject o = scope["c"] as IObject;

                control.BackColor = ColorModule.ToSystemColor(o, scope);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("c"),
                    new NamePattern("ctrl")
                }
            });

            this.SetField("setForeColor", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                IObject o = scope["c"] as IObject;

                control.ForeColor = ColorModule.ToSystemColor(o, scope);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("c"),
                    new NamePattern("ctrl")
                }
            });

            this.SetField("setDock", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                DockStyle o = DockModule.ToStyle(scope["d"] as IObject, scope);

                control.Dock = o;

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("d"),
                    new NamePattern("ctrl"),
                }
            });

            this.SetField("setLocation", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                List<Value> point = scope["point"].ToList(scope);

                Int32 x = point[0].ToInt(scope);
                Int32 y = point[1].ToInt(scope);

                control.Location = new Point(x, y);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("point"),
                     new NamePattern("ctrl")
                }
            });

            this.SetField("getLocation", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;

                return new List(LinkedList.Create(new List<Value> {
                    new Number(control.Location.X),
                    new Number(control.Location.Y),
                }));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("ctrl")
                }
            });

            this.SetField("setText", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                String o = scope["t"].ToString(scope);

                control.Text = o;

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("t"),
                    new NamePattern("ctrl")
                }
            });

            this.SetField("getText", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;

                return new Text(control.Text);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("ctrl")
                }
            });
        }
    }
}
