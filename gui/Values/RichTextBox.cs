using Lumen.Lang;
using System;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class RichTextBoxValue : VControl {
        public RichTextBox value;

        public RichTextBoxValue(RichTextBox result) {
            this.value = result;
        }

        public IType Type => Visual.RTB;

        public Control Control => this.value;

        public Value Clone() {
            throw new NotImplementedException();
        }

        public Int32 CompareTo(Object obj) {
            throw new NotImplementedException();
        }

        public String ToString(Scope e) {
            throw new NotImplementedException();
        }

		public String ToString(String format, IFormatProvider formatProvider) {
			throw new NotImplementedException();
		}
	}
}
