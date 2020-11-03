using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class TypePattern : IPattern {
		private IPattern subpattern;
		private List<Expression> requirements;

		public Boolean IsNotEval => false;

		public TypePattern(IPattern innerExpression, List<Expression> requirements) {
			this.subpattern = innerExpression;
			this.requirements = requirements;
		}

		public Expression Closure(ClosureManager manager) {
			String typeParameter = this.FindTypeParameter();
			if (typeParameter != null) {
				manager.Declare(typeParameter);
			}
			return new TypePattern(this.subpattern.Closure(manager) as IPattern, this.requirements.Select(i => i.Closure(manager)).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public String FindTypeParameter() {
			foreach (Expression i in this.requirements) {
				if (i is IdExpression idExpression && idExpression.id.StartsWith("'")) {
					return idExpression.id;
				}
			}

			return null;
		}

		public List<String> GetDeclaredVariables() {
			String typeParameter = this.FindTypeParameter();
			if (typeParameter != null) {
				List<String> result = new List<String> { typeParameter };
				result.AddRange(this.subpattern.GetDeclaredVariables());
				return result;
			}
			return this.subpattern.GetDeclaredVariables();
		}

		public MatchResult Match(Value value, Scope scope) {
			foreach (Expression requirement in this.requirements) {
				if (requirement is IdExpression idExpression && idExpression.id.StartsWith("'")) {
					scope[idExpression.id] = value.Type;
					continue;
				}

				Value requiredType = requirement.Eval(scope);

				if (requiredType is Constructor ctor) {
					requiredType = this.GetModule(ctor);
				}

				if (requiredType is Module module && module.IsParentOf(value)) {
					continue;
				}

				if (requiredType is Module typeClass && this.GetModule(value.Type).HasImplementation(typeClass)) {
					continue;
				}

				return new MatchResult {
					Success = false,
					Note = $"wait value of type {requiredType} given {value.Type}"
				};
			}

			return this.subpattern.Match(value, scope);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Module GetModule(IType obj) {
			if (obj is Module m) {
				return m;
			}

			if (obj is Instance instance) {
				return (instance.Type as Constructor).Parent as Module;
			}

			if (obj is Constructor constructor) {
				return constructor.Parent;
			}

			if (obj is SingletonConstructor singleton) {
				return singleton.Parent;
			}

			return this.GetModule(obj.Type);
		}

		public override String ToString() {
			return $"({this.subpattern}: {String.Join(", ", this.requirements)})";
		}
	}
}