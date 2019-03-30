using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    public class VariableDeclaration : Expression {
        public String variableName;
        public Expression assignableExpression;
        Boolean isMutable;
        public String file;
        public Int32 line;


        public VariableDeclaration(String variableName, Expression assignableExpression, Boolean isMutable, String file, Int32 line) {
            this.variableName = variableName;
            this.isMutable = isMutable;
            this.assignableExpression = assignableExpression;
            this.line = line;
            this.file = file;
        }

        public Value Eval(Scope scope) {
            Value value = this.assignableExpression.Eval(scope);

            scope.Bind(this.variableName, value);
            if (this.isMutable) {
                scope.mutable.Add(this.variableName);
            }

            return value;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            visible.Add(this.variableName);

            return new VariableDeclaration(
                this.variableName, 
                this.assignableExpression?.Closure(visible, scope),
                this.isMutable,
                this.file, 
                this.line);
        }

        public override String ToString() {
            return $"let {(this.isMutable ? "mut " : "")}{this.variableName} = {this.assignableExpression}";
        }
    }
}