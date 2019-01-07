using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
namespace Stereotype {
	[Serializable]
	public class GenericFunctionMaker : Expression {
		public FunctionDeclaration functionDefineStatement;
		public Dictionary<String, Expression> generic;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public GenericFunctionMaker(FunctionDeclaration functionDefineStatement, Dictionary<String, Expression> generic) {
			this.functionDefineStatement = functionDefineStatement;
			this.generic = generic;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return new GenericFunctionMaker(this.functionDefineStatement.Closure(visible, scope) as FunctionDeclaration, this.generic);
		}

		public Value Eval(Scope e) {
			Dictionary<String, Value> args = new Dictionary<String, Value>();
			foreach (KeyValuePair<String, Expression> i in this.generic) {
				args.Add(i.Key, i.Value?.Eval(e));
			}

			e.Set(this.functionDefineStatement.NameFunction, new GenericFunc(args, this.functionDefineStatement, this.functionDefineStatement.NameFunction));
			return Const.VOID;
		}
	}
}