using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class DotAssigment : Expression {
        internal Expression rigth;
        internal DotExpression left;
        internal String file;
        internal Int32 line;

        internal DotAssigment(DotExpression left, Expression rigth, String file, Int32 line) {
            this.left = left;
            this.rigth = rigth;
            this.file = file;
            this.line = line;
        }

        public Value Eval(Scope e) {
            Value value = this.rigth.Eval(e);

            String name = this.left.nameVariable;
            Value obj = this.left.expression.Eval(e);

            if (obj is IObject iobj) {
                iobj.SetField(name, value, e);
            } else {
                throw new LumenException($"object of type  does not have a field/property '{name}'") {
                    file = this.file,
                    line = this.line
                };
            }

            return value;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new DotAssigment(this.left.Closure(visible, thread) as DotExpression, this.rigth.Closure(visible, thread), this.file, this.line);
        }

        public override String ToString() {
            return $"{this.left} <- {this.rigth}";
        }
    }
}