using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
	internal class DeconstructPattern : IPattern {
		private readonly String typeName;
		private readonly List<IPattern> subPatterns;
		private readonly Expression exp;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Boolean IsNotEval => false;
		public DeconstructPattern(String typeName, List<IPattern> subPatterns) {
			this.typeName = typeName;
			this.subPatterns = subPatterns;
		}

		public DeconstructPattern(Expression exp, List<IPattern> subPatterns) {
			this.exp = exp;
			this.subPatterns = subPatterns;
		}

		public Expression Closure(ClosureManager manager) {
			return new DeconstructPattern(new ValueE(manager.Scope[typeName]), this.subPatterns.Select(i => i.Closure(manager) as IPattern).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			if (this.typeName.StartsWith("'")) {
				result.Add(this.typeName);
			}

			foreach (IPattern i in this.subPatterns) {
				result.AddRange(i.GetDeclaredVariables());
			}

			return result;
		}

		public MatchResult Match(Value value, Scope scope) {
			Value requiredType;

			if (this.exp == null) {
				// if it's not generic type
				if (!this.typeName.StartsWith("'")) {
					requiredType = scope[this.typeName];
				}
				else {
					if (scope.ExistsInThisScope(this.typeName)) {
						requiredType = scope[this.typeName];
					}
					else {
						requiredType = value.Type;
						scope[this.typeName] = value.Type;
					}
				}
			} else {
				requiredType = exp.Eval(scope);
			}

			if (requiredType is Constructor ctor && value is Instance instance) {
				if (ctor.IsParentOf(value)) {
					for (Int32 i = 0; i < ctor.Fields.Count; i++) {
						MatchResult res = this.subPatterns[i].Match(instance.items[i], scope);
						if (!res.Success) {
							return res;
						}
					}

					return MatchResult.True;
				}

				return new MatchResult {
					Success = false,
					Note = $"can not deconstruct a value of type {instance.Type}"
				};
			}

			if (requiredType is SingletonConstructor) {
				return new MatchResult { Success = requiredType == value };
			}

			if (requiredType is Module m) {
				if (m.IsParentOf(value)) {
					return this.subPatterns[0].Match(value, scope);
				}
			}

			return new MatchResult { Success = false };
		}

		public override String ToString() {
			return $"({this.typeName} {String.Join(" ", this.subPatterns)})";
		}
	}
}