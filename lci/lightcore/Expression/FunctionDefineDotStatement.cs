using System;
using System.Collections.Generic;
using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	public class ArgumentMetadataGenerator : Expression {
		public string name;
		public Expression type;
		public Expression defaultValue;

		public ArgumentMetadataGenerator(string name, Expression type, Expression defaultValue) {
			this.name = name;
			this.type = type;
			this.defaultValue = defaultValue;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			return Const.NULL;
		}

		public FunctionArgument EvalArgumnet(Scope e) {
			return new FunctionArgument(name,  defaultValue == null ? null : (defaultValue is ExpressionE expe ? (Object)expe.expression : defaultValue.Eval(e)));
		}

		public Expression Optimize(Scope scope) {
			return this;
		}
	}
}