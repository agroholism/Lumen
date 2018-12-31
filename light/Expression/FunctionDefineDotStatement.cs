using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class ArgumentMetadataGenerator : Expression {
		public String name;
		public Expression type;
		public Expression defaultValue;

		public ArgumentMetadataGenerator(String name, Expression type, Expression defaultValue) {
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
			FunctionArgument res = new FunctionArgument(this.name, this.defaultValue == null ? null : (this.defaultValue is ExpressionE expe ? (Object)expe.expression : this.defaultValue.Eval(e)));

			if(this.type != null) {
				Value t = this.type.Eval(e);
				if(t is Record kt) {
					res.Attributes = new Dictionary<String, Value> { ["type"] = kt };
				}
			}

			return res;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}
	}
}