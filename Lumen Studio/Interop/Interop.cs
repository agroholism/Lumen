using System.Collections.Generic;
using System.Windows.Forms;
using FastColoredTextBoxNS;

using Lumen.Lang;

namespace Lumen.Studio.Interop {
    public class Interop {
        /*public static LambdaFun Module { get; } = new LambdaFun((e, args) => Const.UNIT);

        public static LambdaFun Form { get; private set; }
        public static LambdaFun TextBox { get; private set; }
        public static LambdaFun Range { get; private set; }

        public static LambdaFun Style { get; private set; }

        public Interop() {
            Form = new LambdaFun((e, args) => Const.UNIT);
            Form.SetField("@conStringuctor", Const.TRUE, null);
            Form.SetField("String", new LambdaFun((e, args) => {
                Form value = (e.This as MainFormInterop).value;

                return new Text(value.Name);
            }), null);

            TextBox = new LambdaFun((e, args) => Const.UNIT);
            TextBox.SetField("@conStringuctor", Const.TRUE, null);
            TextBox.SetField("String", new LambdaFun((e, args) => {
                FastColoredTextBox value = (e.This as TextBoxI).value;

                return new Text(value.Name);
            }), null);
            TextBox.SetField("range", new LambdaFun((e, args) => {
                FastColoredTextBox value = (e.This as TextBoxI).value;

                if (args.Length > 0) {
                    var res = new List<Value>();

                    foreach (var i in value.GetRanges(args[0].ToString(e))) {
                        res.Add(new RangeI(i));
                    }

                    return new Array(res);
                    //return new RangeI(value.GetRange((Int32)args[0].ToDouble(e), (Int32)args[1].ToDouble(e)));
                }

                return new RangeI(value.Range);
            }), null);

            Range = new LambdaFun((e, args) => Const.UNIT);
            Range.SetField("@conStringuctor", Const.TRUE, null);
            Range.SetField("String", new LambdaFun((e, args) => {
                FastColoredTextBoxNS.Range value = (e.This as RangeI).value;

                return new Text(value.Text);
            }), null);
            Range.SetField("set_style", new LambdaFun((e, args) => {
                FastColoredTextBoxNS.Range value = (e.This as RangeI).value;

                Style s = (args[0] as StyleI).value;

                if (args.Length == 2) {
                    value.SetStyle(s, args[1].ToString(e));
                }

                return Const.UNIT;
            }), null);

            Style = new LambdaFun((e, args) => Const.UNIT);
            Style.SetField("@conStringuctor", Const.TRUE, null);

            Style.SetField("comment", new StyleI(Settings.Comment), null);
            Style.SetField("error", new StyleI(Settings.Error), null);
            Style.SetField("keyword", new StyleI(Settings.Keyword), null);
            Style.SetField("text", new StyleI(Settings.String), null);
            Style.SetField("class", new StyleI(Settings.Type), null);

            Module.SetField("main_form", new MainFormInterop(MainForm.Instance), null);
            Module.SetField("text_box", new TextBoxI(MainForm.MainTextBoxManager.TextBox), null);
            Module.SetField("style", Style, null);
        }
    }

    public class MainFormInterop : ModuleValue {
        internal Form value;

        public MainFormInterop(Form value) {
            this.value = value;
            this.Parent = Interop.Form;
        }
    }

    public class TextBoxI : ModuleValue {
        internal FastColoredTextBox value;

        public TextBoxI(FastColoredTextBox value) {
            this.value = value;
            this.Parent = Interop.TextBox;
        }
    }

    public class RangeI : ModuleValue {
        internal FastColoredTextBoxNS.Range value;

        public RangeI(FastColoredTextBoxNS.Range value) {
            this.value = value;
            this.Parent = Interop.Range;
        }
    }

    public class StyleI : ModuleValue {
        internal Style value;

        public StyleI(Style value) {
            this.value = value;
            this.Parent = Interop.Style;
        }*/
    }
}
