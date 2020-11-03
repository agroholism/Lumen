using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace ldoc {
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

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
            return new DotExpression(this.expression.Closure(manager), this.nameVariable, this.fileName, this.line);
        }

        public Value Eval(Scope e) {
            if (this.expression is IdExpression conste && conste.id == "_") {
                return new UserFun(
                    new List<IPattern> { new NamePattern("x") }, new DotExpression(new IdExpression("x", this.line, this.fileName), this.nameVariable, this.fileName, this.line));
            }

            try {
                Value a = this.expression.Eval(e);

                if (a is Module module) {
                    return module.GetMember(this.nameVariable, e);
                }

                if (a is IType obj) {
                    if(obj.TryGetMember(this.nameVariable, out Value f)) {
                        return f;
                    }

                }

              /*  IObject type = a.Type;

                if (type.TryGetField(this.nameVariable, out Value prf) && prf is Fun property && property.Attribute == EntityAttribute.PROPERTY) {
                    return property.Run(new Scope { ["this"] = a });
                }*/

                return Const.UNIT;
            } catch (LumenException hex) {
                if (hex.File == null) {
                    hex.File = this.fileName;
                }

                if (hex.Line == -1) {
                    hex.Line = this.line;
                }

                throw;
            }
        }

        public override String ToString() {
            return this.expression.ToString() + "." + this.nameVariable;
        }
    }
}