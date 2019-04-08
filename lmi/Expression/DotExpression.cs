using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace Lumen.Light {
    internal class DotExpression : Expression {
        internal Expression expression;
        internal String nameVariable;
        private readonly String fileName;
        private readonly Int32 line;

        public DotExpression(Expression expression, String nameVariable, String fileName, Int32 line) {
            this.expression = expression;
            this.nameVariable = nameVariable;
            this.fileName = fileName;
            this.line = line;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new DotExpression(this.expression.Closure(visible, thread), this.nameVariable, this.fileName, this.line);
        }

        public Value Eval(Scope e) {
            if (this.expression is IdExpression conste && conste.id == "_") {
                return new UserFun(
                    new List<IPattern> { new NamePattern("x") }, new DotExpression(new IdExpression("x", this.line, this.fileName), this.nameVariable, this.fileName, this.line));
            }

            try {
                Value a = this.expression.Eval(e);

                if (a is Module module) {
                    return module.GetField(this.nameVariable, e);
                }

                if (a is IObject obj) {
                    if(obj.TryGetField(this.nameVariable, out Value f)) {
                        return f;
                    }

                }

              /*  IObject type = a.Type;

                if (type.TryGetField(this.nameVariable, out Value prf) && prf is Fun property && property.Attribute == EntityAttribute.PROPERTY) {
                    return property.Run(new Scope { ["this"] = a });
                }*/

                return Const.UNIT;
            } catch (LumenException hex) {
                if (hex.file == null) {
                    hex.file = this.fileName;
                }

                if (hex.line == -1) {
                    hex.line = this.line;
                }

                throw;
            }
        }

        public override String ToString() {
            return this.expression.ToString() + "." + this.nameVariable;
        }
    }
}