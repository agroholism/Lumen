using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lmi;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
	internal class TypePattern : IPattern {
		private IPattern subpattern;
		private List<Expression> requirements;

		public TypePattern(IPattern innerExpression, List<Expression> requirements) {
			this.subpattern = innerExpression;
			this.requirements = requirements;
		}

		public IPattern Closure(ClosureManager manager) {
			String typeParameter = this.FindTypeParameter();
			if (typeParameter != null) {
				manager.Declare(typeParameter);
			}
			return new TypePattern(this.subpattern.Closure(manager), this.requirements.Select(i => i.Closure(manager)).ToList());
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

		public MatchResult Match(IValue value, Scope scope) {
			foreach (Expression requirement in this.requirements) {
				if (requirement is IdExpression idExpression && idExpression.id.StartsWith("'")) {
					scope[idExpression.id] = value.Type;
					continue;
				}

				IValue requiredType = requirement.Eval(scope);

				if(requiredType is GenericLater) {
					scope[(requirement as IdExpression).id] = value.Type;
					continue;
				}

				if (requiredType is Constructor ctor) {
					requiredType = this.GetModule(ctor);
				}

				if (requiredType is Module module && module.IsParentOf(value)) {
					continue;
				}

				if (requiredType is Module typeClass) {
					if (!this.GetModule(value.Type).HasImplementation(typeClass)) {
						return new MatchResult(
							MatchResultKind.Fail,
							$"value of type {value.Type} should implement class {requiredType}"
						);
					}

					continue;
				} 

				if (requiredType is IType type && type is FunctionalType ft) {
					if (type.IsParentOf(value)) {
						continue;
					}

					return new MatchResult(
						MatchResultKind.Fail,
						$"value does not fits to the requirements of the functional type {ft.Name}"
					);
				}

				return new MatchResult(
					MatchResultKind.Fail,
					$"wait value of type {requiredType} given {value.Type}"
				);
			}

			return this.subpattern.Match(value, scope);
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