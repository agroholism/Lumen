using System;
using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	public class PartialFun : IValue, Fun {
		public Fun InnerFunction { get; set; }
		public IValue[] Args { get; set; }
		public Int32 restArgs;

		public String Name { get; set; }
		public List<IPattern> Parameters { get; set; }
		public IType Parent { get; set; }
		public IType Type => Prelude.Function;

		public IValue Call(Scope scope, params IValue[] args) {
			if (this.restArgs > args.Length) {
				return new PartialFun {
					InnerFunction = this,
					Args = args,
					restArgs = this.restArgs - args.Length
				};
			}

			this.SetActualRecFunction(scope);

			List<IValue> values = new List<IValue>();
			values.AddRange(args);

			for (Int32 i = 0; i < this.Args.Length; i++) {
				values.Insert(i, this.Args[i]);
			}

			return this.InnerFunction.Call(scope, values.ToArray());
		}

		private void SetActualRecFunction(Scope scope) {
			if (scope.IsExistsInThisScope("rec") && scope["rec"] is not PartialFun) {
				return;
			}

			IValue actualRec = this.InnerFunction;

			while (actualRec is PartialFun partial) {
				actualRec = partial.InnerFunction;
			}

			scope["rec"] = actualRec;
		}

		public override String ToString() {
			return $"[Function {this.Name ?? this.GetHashCode().ToString()} partial of {this.InnerFunction}]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}
	}
}
