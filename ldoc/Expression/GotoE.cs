using Lumen.Lang;

namespace Lumen.Lang.Expressions {
    public class GotoE : System.Exception {
        public Value[] result;

        public GotoE(Value[] result) {
            this.result = result;
        }
    }

	public class RecursionException : System.Exception {
		public Value[] result;

		public RecursionException(Value[] result) {
			this.result = result;
		}
	}
}