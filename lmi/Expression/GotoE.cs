using Lumen.Lang;

namespace Lumen.Lang.Expressions {
    public class GotoE : System.Exception {
        public Value[] result;

        public GotoE(Value[] result) {
            this.result = result;
        }
    }
}