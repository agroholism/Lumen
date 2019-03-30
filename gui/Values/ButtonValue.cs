using Lumen.Lang;
using System;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class ButtonValue : Value, VControl {
        public Button value;

        public ButtonValue(Button result) {
            this.value = result;
        }

        public IObject Type => Visual.Button;

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
    }
}
