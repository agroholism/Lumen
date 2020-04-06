using Lumen.Lang.Expressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class RTBModule : Module {
        public RTBModule() {
            this.Name = "Visual.RichTextBox";

            this.SetMember("defaultConstructor", new LambdaFun((ex, argsx) => {
                return new RichTextBoxValue(new RichTextBox());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("()")
				}
			});

			this.Derive(Visual.Control);
        }
    }
}
