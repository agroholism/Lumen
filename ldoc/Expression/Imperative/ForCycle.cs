using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;
using System;
using Lumen;
using System.Linq;

namespace ldoc {
    internal class ForCycle : Expression {
        public Expression expression;
        public Expression body;
        private readonly IPattern varName;
        private readonly Expression varType;
        private readonly Boolean declaredVar;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public ForCycle(IPattern varName, Expression varType, Boolean declaredVar, Expression expressions, Expression statement) {
            this.varName = varName;
            this.varType = varType;
            this.declaredVar = declaredVar;
            this.expression = expressions;
            this.body = statement;
        }

        public Expression Optimize(Scope scope) {
            return this;
        }

        public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();

			manager2.Declare(this.varName.GetDeclaredVariables());

            Expression result = new ForCycle(this.varName, this.varType?.Closure(manager), this.declaredVar, this.expression.Closure(manager), this.body.Closure(manager2));

			return result;
		}

        public Value Eval(Scope scope) {
            Value container = this.expression.Eval(scope);

            foreach (Value i in container.ToStream(scope)) {
				this.varName.Match(i, scope);
   
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

        public override String ToString() {
            return "";
        }
    }
}