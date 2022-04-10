using System;

namespace Lumen.Lang {
	public class Future : BaseValueImpl {
		public System.Threading.Tasks.Task<IValue> Task { get; set; }

		public override IType Type => Prelude.Future;

		public Future(System.Threading.Tasks.Task<IValue> task) {
			this.Task = task;
		}

		public override String ToString() {
			return $"[Future #{this.GetHashCode()}]";
		}
	}
}
