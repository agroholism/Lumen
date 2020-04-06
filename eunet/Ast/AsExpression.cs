using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class AsExpression : Expression {
		private Expression result;
		private Expression type;
		private System.Boolean isSafe;
		private System.Int32 line;
		private System.String fileName;

		public AsExpression(Expression result, Expression type, System.Boolean isSafe, System.Int32 line, System.String fileName) {
			this.result = result;
			this.type = type;
			this.isSafe = isSafe;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new AsExpression(this.result.Closure(manager), this.type.Closure(manager), this.isSafe, this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject obj = this.result.Eval(scope);
			XnObject type = this.type.Eval(scope);

			if (type == obj.Type) {
				return obj;
			}

			if (type == XnStd.ObjectType && !(obj is Nil)) {
				return obj;
			}

			if (type == XnStd.GetNullable(XnStd.ObjectType)){
				return obj;
			}

			KsType objType = obj.Type as KsType;
			if(objType.EigenNamespace.variables.TryGetValue("as", out XnObject converter)
				&& converter is Function fn) {
				XnObject converted = fn.Run(new Scope(), obj, type);

				if(converted is Nil) {
					if (this.isSafe) {
						return Nil.NilIns;
					}

					throw new XenonException("convert error", line: this.line, fileName: this.fileName);
				} else {
					return converted;
				}
			}

			if(this.isSafe) {
				return Nil.NilIns;
			}

			throw new XenonException("convert error", line: this.line, fileName: this.fileName);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}