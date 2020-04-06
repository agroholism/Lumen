using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ForCycle : Expression {
		internal Expression expression;
		internal Expression body;
		internal IPattern pattern;

        public ForCycle(IPattern pattern, Expression expressions, Expression statement) {
            this.pattern = pattern;
            this.expression = expressions;
            this.body = statement;
        }

        public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();

			manager2.Declare(this.pattern.GetDeclaredVariables());

			ForCycle res =  new ForCycle(this.pattern, this.expression.Closure(manager), this.body.Closure(manager2));

			if (!manager.HasYield) {
				manager.HasYield = manager2.HasYield;
			}

			return res;
		}

        public Value Eval(Scope scope) {
            Value container = this.expression.Eval(scope);

			List<String> declared = this.pattern.GetDeclaredVariables();
            foreach (Value i in container.ToStream(scope)) {
				MatchResult matchResult = this.pattern.Match(i, scope);
				if (!matchResult.Success) {
					throw new LumenException(matchResult.Note);
				}

				try {
					this.body.Eval(scope);
				} catch (Break bs) {
					if (bs.UseLabel() > 0) {
						throw bs;
					}
					break;
				} catch (Next) {
					continue;
				} finally {
					foreach(String declaration in declared) {
						scope.Remove(declaration);
					}
				}
            }

            return Const.UNIT;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value container = this.expression.Eval(scope);

			List<String> declared = this.pattern.GetDeclaredVariables();
			foreach (Value i in container.ToStream(scope)) {
				MatchResult matchResult = this.pattern.Match(i, scope);
				if (!matchResult.Success) {
					throw new LumenException(matchResult.Note);
				}

				IEnumerable<Value> y = null;

				try {
					y = this.body.EvalWithYield(scope);
				}
				catch (Break bs) {
					if (bs.UseLabel() > 0) {
						throw bs;
					}
					break;
				}
				catch(Return ret) {
					yield break;
				}
				catch (Next) {
					continue;
				}

				if(y != null) {
					foreach (Value it in y) {
						if(it is CurrGeenVal) {
							break;
						}
						yield return it;
					}
				}
			}
		}

		/*public override String ToString() {
            return $"for {this.pattern} in {this.expression}: {Utils.Bodify(this.body)}";
        }*/
	}
}