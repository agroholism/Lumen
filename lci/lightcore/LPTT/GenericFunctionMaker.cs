using System;
using System.Collections.Generic;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	[Serializable]
	public class GenericFunctionMaker : Expression {
		public FunctionDefineStatement functionDefineStatement;
		public Dictionary<String, Expression> generic;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public GenericFunctionMaker(FunctionDefineStatement functionDefineStatement, Dictionary<String, Expression> generic) {
			this.functionDefineStatement = functionDefineStatement;
			this.generic = generic;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return new GenericFunctionMaker(functionDefineStatement.Closure(visible, scope) as FunctionDefineStatement, this.generic);
		}

		public Value Eval(Scope e) {
			Dictionary<String, Value> args = new Dictionary<string, Value>();
			foreach (var i in this.generic) {
				args.Add(i.Key, i.Value?.Eval(e));
			}

			e.Set(functionDefineStatement.NameFunction, new GenericFunc(args, functionDefineStatement, functionDefineStatement.NameFunction));
			return Const.NULL;
		}
	}
}