using Lumen.Lang;
using System;
using System.Windows.Forms;

namespace Lumen.Lang.Libraries.Visual {
    public class FormValue : Value, VControl {
        public Form value;

        public FormValue(Form result) {
            this.value = result;
        }

        public IObject Type => Visual.RForm;

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

    public class LabelValue : Value, VControl {
        public Label value;

        public LabelValue(Label result) {
            this.value = result;
        }

        public IObject Type => Visual.Label;

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
