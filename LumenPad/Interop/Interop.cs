using System.Collections.Generic;
using System.Windows.Forms;
using FastColoredTextBoxNS;

using Lumen.Lang.Std;

namespace Lumen.Studio.Interop {
	public class Interop {
		public static LambdaFun Module { get; } = new LambdaFun((e, args) => Const.VOID);

		public static LambdaFun Form { get; private set; }
		public static LambdaFun TextBox { get; private set; }
		public static LambdaFun Range { get; private set; }

		public static LambdaFun Style { get; private set; }

		public Interop() {
			Form = new LambdaFun((e, args) => Const.VOID);
			Form.Set("@constructor", Const.TRUE, null);
			Form.Set("str", new LambdaFun((e, args) => {
				Form value = (e.This as MainFormInterop).value;

				return new Str(value.Name);
			}), null);

			TextBox = new LambdaFun((e, args) => Const.VOID);
			TextBox.Set("@constructor", Const.TRUE, null);
			TextBox.Set("str", new LambdaFun((e, args) => {
				FastColoredTextBox value = (e.This as TextBoxI).value;

				return new Str(value.Name);
			}), null);
			TextBox.Set("range", new LambdaFun((e, args) => {
				FastColoredTextBox value = (e.This as TextBoxI).value;

				if(args.Length > 0) {
					var res = new List<Value>();

					foreach(var i in value.GetRanges(args[0].ToString(e))) {
						res.Add(new RangeI(i));
					}

					return new Vec(res);
					//return new RangeI(value.GetRange((Int32)args[0].ToDouble(e), (Int32)args[1].ToDouble(e)));
				}

				return new RangeI(value.Range);
			}), null);

			Range = new LambdaFun((e, args) => Const.VOID);
			Range.Set("@constructor", Const.TRUE, null);
			Range.Set("str", new LambdaFun((e, args) => {
				FastColoredTextBoxNS.Range value = (e.This as RangeI).value;

				return new Str(value.Text);
			}), null);
			Range.Set("set_style", new LambdaFun((e, args) => {
				FastColoredTextBoxNS.Range value = (e.This as RangeI).value;

				Style s = (args[0] as StyleI).value;

				if (args.Length == 2) {
					value.SetStyle(s, args[1].ToString(e));
				}

				return Const.VOID;
			}), null);

			Style = new LambdaFun((e, args) => Const.VOID);
			Style.Set("@constructor", Const.TRUE, null);

			Style.Set("comment", new StyleI(Settings.Comment), null);
			Style.Set("error", new StyleI(Settings.Error), null);
			Style.Set("keyword", new StyleI(Settings.Keyword), null);
			Style.Set("text", new StyleI(Settings.String), null);
			Style.Set("class", new StyleI(Settings.Type), null);

			Module.Set("main_form", new MainFormInterop(MainForm.Instance), null);
			Module.Set("text_box", new TextBoxI(MainForm.MainTextBoxManager.TextBox), null);
			Module.Set("style", Style, null);
		}
	}

	public class MainFormInterop : RecordValue {
		internal Form value;

		public MainFormInterop(Form value) {
			this.value = value;
			this.Prototype = Interop.Form;
		}
	}

	public class TextBoxI : RecordValue {
		internal FastColoredTextBox value;

		public TextBoxI(FastColoredTextBox value) {
			this.value = value;
			this.Prototype = Interop.TextBox;
		}
	}

	public class RangeI : RecordValue {
		internal FastColoredTextBoxNS.Range value;

		public RangeI(FastColoredTextBoxNS.Range value) {
			this.value = value;
			this.Prototype = Interop.Range;
		}
	}

	public class StyleI : RecordValue {
		internal Style value;

		public StyleI(Style value) {
			this.value = value;
			this.Prototype = Interop.Style;
		}
	}
}
