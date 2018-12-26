using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class CombineAssigment : Expression {
		public String id;
		public Expression exp;
		public String operation;
		public Int32 line;
		public String file;

		public CombineAssigment(String name, Expression expression, String operation, Int32 line, String file) {
			this.id = name;
			this.exp = expression;
			this.operation = operation;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope scope) {
			if (scope.IsConstant(this.id)) {
				throw new Lumen.Lang.Std.Exception($"cannot change constant '{this.id}'", stack: scope) {
					line = this.line,
					file = this.file
				};
			}

			if (!scope.ExistsInThisScope(this.id)) {
				throw new Lumen.Lang.Std.Exception($"variable '{this.id}' is not declared", stack: scope) {
					line = this.line,
					file = this.file
				};
			}

			Value oldValue = scope[this.id];
			KType type = oldValue.Type;

			Value newValue;

			if (type.AttributeExists(this.operation) && type.GetAttribute(this.operation, scope) is Fun fun) {
				newValue = fun.Run(new Scope(scope) { This = oldValue }, this.exp.Eval(scope));
			}
			else {
				newValue = new BinaryExpression(new IdExpression(this.id, this.line, this.file), this.exp, this.operation.Replace("=", ""), this.line, this.file).Eval(scope);
			}

			scope[this.id] = newValue;

			return newValue;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new CombineAssigment(this.id, this.exp.Closure(visible, thread), this.operation, this.line, this.file);
		}

		public Expression Optimize(Scope scope) {
			return new CombineAssigment(this.id, this.exp.Optimize(scope), this.operation, this.line, this.file);
		}

		public override String ToString() {
			return $"{this.id} {this.operation} {this.exp}";
		}
	}
}
