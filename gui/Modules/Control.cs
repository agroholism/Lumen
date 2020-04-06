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
            this.Name = "Visual.Control";

			this.Requirements = new List<Fun>();

			this.SetMember("addControl", new LambdaFun((scope, args) => {
				VControl vcontrol = scope["ctrl"] as VControl;
				Control control = vcontrol.Control;

				VControl btn = scope["ctrl'"] as VControl;

				control.Controls.Add(btn.Control);

				return vcontrol;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("ctrl'"),
					new NamePattern("ctrl")
				}
			});

			this.SetMember("addOnClick", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;

                Fun fun = scope["f"] as Fun;
                control.Click += (sender, obj) => fun.Run(new Scope(scope.parent), vcontrol);
                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("ctrl"),
                }
            });

            this.SetMember("setBackColor", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                IType o = scope["c"] as IType;

                control.BackColor = ColorModule.ToSystemColor(o, scope);

                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("c"),
                    new NamePattern("ctrl")
                }
            });

            this.SetMember("setForeColor", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                IType o = scope["c"] as IType;

                control.ForeColor = ColorModule.ToSystemColor(o, scope);

                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("c"),
                    new NamePattern("ctrl")
                }
            });

            this.SetMember("setDock", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                DockStyle o = DockModule.ToStyle(scope["d"] as IType, scope);

                control.Dock = o;

                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("d"),
                    new NamePattern("ctrl"),
                }
            });

            this.SetMember("setLocation", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                List<Value> point = scope["point"].ToList(scope);

                Int32 x = point[0].ToInt(scope);
                Int32 y = point[1].ToInt(scope);

                control.Location = new Point(x, y);

                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("point"),
                     new NamePattern("ctrl")
                }
            });

            this.SetMember("getLocation", new LambdaFun((scope, args) => {
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

            this.SetMember("setText", new LambdaFun((scope, args) => {
                VControl vcontrol = scope["ctrl"] as VControl;
                Control control = vcontrol.Control;
                String o = scope["t"].ToString(scope);

                control.Text = o;

                return vcontrol;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("t"),
                    new NamePattern("ctrl")
                }
            });

            this.SetMember("getText", new LambdaFun((scope, args) => {
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
