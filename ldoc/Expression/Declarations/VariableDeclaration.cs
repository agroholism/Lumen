using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    public class VariableDeclaration : Expression {
        public IPattern variableName;
        public Expression assignableExpression;
        Boolean isMutable;
        public String file;
        public Int32 line;

        public VariableDeclaration(IPattern variableName, Expression assignableExpression, Boolean isMutable, String file, Int32 line) {
            this.variableName = variableName;
            this.isMutable = isMutable;
            this.assignableExpression = assignableExpression;
            this.line = line;
            this.file = file;
        }

        public Value Eval(Scope scope) {
            Value value = this.assignableExpression.Eval(scope);

			if(!this.variableName.Match(value, scope).Success) {
				throw new LumenException("pattern broken", line: line, fileName: file);
			}

            return value;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.variableName.GetDeclaredVariables());

            return new VariableDeclaration(
                this.variableName, 
                this.assignableExpression?.Closure(manager),
                this.isMutable,
                this.file, 
                this.line);
        }

        public override String ToString() {
            return $"let {(this.isMutable ? "mut " : "")}{this.variableName} = {this.assignableExpression}";
        }
    }
}