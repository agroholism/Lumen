using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;
using System;
using Lumen;
using System.Linq;

namespace Lumen.Light {
    internal class ForCycle : Expression {
        public Expression expression;
        public Expression body;
        private readonly String varName;
        private readonly Expression varType;
        private readonly Boolean declaredVar;

        public ForCycle(String varName, Expression varType, Boolean declaredVar, Expression expressions, Expression statement) {
            this.varName = varName;
            this.varType = varType;
            this.declaredVar = declaredVar;
            this.expression = expressions;
            this.body = statement;
        }

        public Expression Optimize(Scope scope) {
            return this;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            List<String> bodyVariables = new List<String>();

            bodyVariables.AddRange(visible);

            bodyVariables.Add(this.varName);

            return new ForCycle(this.varName, this.varType?.Closure(visible, thread), this.declaredVar, this.expression.Closure(visible, thread), this.body.Closure(bodyVariables, thread));
        }

        public Value Eval(Scope scope) {
            Value container = this.expression.Eval(scope);

            Value saver = null;

            if (this.declaredVar) {
                if (scope.ExistsInThisScope(this.varName)) {
                    saver = scope[this.varName];
                }

                scope.Set(this.varName, Const.UNIT);
            }

            foreach (Value i in container.ToSequence(scope)) {
                scope[this.varName] = i;

                try {
                    this.body.Eval(scope);
                } catch (Break bs) {
                    if (bs.UseLabel() > 0) {
                        throw bs;
                    }
                    break;
                } catch (Next) {
                    continue;
                }
            }

            if (saver != null) {
                scope[this.varName] = saver;
            }

            return Const.UNIT;
        }

        public override String ToString() {
            return "";
        }
    }
}