using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using System.Collections.Generic;
using String = System.String;

namespace Lumen.Light {
    internal class WhileExpression : Expression {
        internal Expression condition;
        internal Expression body;

        internal WhileExpression(Expression condition, Expression body) {
            this.condition = condition;
            this.body = body;
        }

        public Value Eval(Scope scope) {
            while (this.condition.Eval(scope).ToBoolean()) {
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

            return Const.UNIT;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new WhileExpression(this.condition.Closure(visible, thread), this.body.Closure(visible, thread));
        }

        public override String ToString() {
            return "while(" + this.condition.ToString() + ") { " + Environment.NewLine + this.body.ToString() + Environment.NewLine + "}";
        }
    }
}