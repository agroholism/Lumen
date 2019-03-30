using Lumen.Lang.Expressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class RTBModule : Module {
        public RTBModule() {
            this.name = "Visual.RichTextBox";

            this.SetField("create", new LambdaFun((ex, argsx) => {
                return new RichTextBoxValue(new RichTextBox());
            }) {
                Arguments = new List<IPattern> {
                    
                }
            });

            this.Derive(Visual.Control);
        }
    }
}
