using System;
using System.Collections;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class UserFunction : Function {
		public Dictionary<String, KsTypeable> arguments = new Dictionary<String, KsTypeable>();
		internal Ast.Expression body;

		public KsTypeable Type => XnStd.FunctionType;

		public XnObject Run(Scope scope, params XnObject[] arguments) {
			if(this.arguments.Count != arguments.Length) {
				throw new XenonException("invalid arg count");
			}

			Int32 index = 0;
			foreach(KeyValuePair<String, KsTypeable> i in this.arguments) {
				if(i.Value.Checker(arguments[index])) {
					scope.DeclareVariable(i.Key, i.Value);
					scope.Assign(i.Key, arguments[index]);
				} else {
					throw new XenonException("invalid arg type");
				}
				index++;
			}

			try {
				return this.body.Eval(scope);
			} catch(Ast.Return ret) {
				return ret.result;
			}
		}
	}

	public class UserSequence : IEnumerable<XnObject> {
		public Scope scope;
		internal Ast.Expression body;

		public IEnumerator<XnObject> GetEnumerator() {
			return body.EvalWithYield(this.scope).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
