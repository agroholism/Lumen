using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    /// <summary> Assigment operator. </summary>
    public class Assigment : Expression {
        internal String variableName;
        internal Expression assignableExpression;
        internal Int32 line;
        internal String file;

        public Assigment(String variableName, Expression assignableExpression, Int32 line, String file) {
            this.variableName = variableName;
            this.assignableExpression = assignableExpression;
            this.line = line;
            this.file = file;
        }

        public Value Eval(Scope scope) {
            Value value = this.assignableExpression.Eval(scope);

            scope.Set(this.variableName, value);

            return value;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            if (!visible.Contains(this.variableName)) {
                visible.Add(this.variableName);
            }

            return new Assigment(this.variableName, this.assignableExpression.Closure(visible, thread), this.line, this.file);
        }

        public override String ToString() {
            return $"{this.variableName} <- {this.assignableExpression}";
        }
    }
}