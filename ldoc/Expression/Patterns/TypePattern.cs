using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	internal class TypePattern : IPattern {
		private readonly IPattern innerExpression;
		private readonly List<Expression> requirements;
		public Boolean IsNotEval => false;
		public TypePattern(IPattern innerExpression, List<Expression> requirements) {
			this.innerExpression = innerExpression;
			this.requirements = requirements;
		}
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
			return new TypePattern(this.innerExpression.Closure(manager) as IPattern, this.requirements.Select(i => i.Closure(manager)).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return this.innerExpression.GetDeclaredVariables();
		}

		public MatchResult Match(Value value, Scope scope) {
			foreach(Expression requirement in this.requirements) {
				Value requiredType = requirement.Eval(scope);

				if (requiredType is Module module && module.IsParentOf(value)) {
					continue;
				}

				if (requiredType is Module typeClass && GetModule(value.Type).HasMixin(typeClass)) {
					continue;
				}

				return new MatchResult {
					Success = false,
					Note = $"wait value of type {requiredType} given {value.Type}"
				};
			}

			return this.innerExpression.Match(value, scope);
		}

		public Module GetModule(IType obj) {
			if (obj is Module m) {
				return m;
			}

			if (obj is Instance instance) {
				return (instance.Type as Constructor).Parent as Module;
			}

			if(obj is Constructor constructor) {
				return constructor.Parent;
			}

			if (obj is SingletonConstructor singleton) {
				return singleton.Parent;
			}

			return GetModule(obj.Type);
		}

		public override String ToString() {
			return $"({this.innerExpression}: {String.Join(", ", this.requirements)})";
		}
	}
}