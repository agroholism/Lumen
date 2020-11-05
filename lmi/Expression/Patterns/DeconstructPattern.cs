using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	internal class DeconstructPattern : IPattern {
		private Expression constructor;
		private List<IPattern> subpatterns;

		public Boolean IsNotEval { get; set; } = false;

		public DeconstructPattern(Expression constructor, List<IPattern> subPatterns) {
			this.constructor = constructor;
			this.subpatterns = subPatterns;
		}

		public Expression Closure(ClosureManager manager) {
			return new DeconstructPattern(this.constructor.Closure(manager), this.subpatterns.Select(i => i.Closure(manager) as IPattern).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			if(this.constructor is IdExpression idExpression && idExpression.id.StartsWith("'")) {
				result.Add(idExpression.id);
			}

			foreach (IPattern i in this.subpatterns) {
				result.AddRange(i.GetDeclaredVariables());
			}

			return result;
		}

		public MatchResult Match(Value value, Scope scope) {
			Value requiredType = null;
			String typeParameter = null;

			if (this.constructor is IdExpression idExpression && idExpression.id.StartsWith("'")) {
				typeParameter = idExpression.id;
			} else {
				requiredType = this.constructor.Eval(scope);
			}

			if(typeParameter != null) {
				Instance ins = value as Instance;
				scope[typeParameter] = ins.Type;
				for (Int32 i = 0; i < this.subpatterns.Count; i++) {
					MatchResult res = this.subpatterns[i].Match(ins.Items[i], scope);
					if (!res.Success) {
						return res;
					}
				}

				return MatchResult.True;
			}

			if (requiredType is ExceptionConstructor exceptionConstructor 
				&& value is LumenException exceptionValue) {
				if (exceptionConstructor.IsParentOf(value)) {
					for (Int32 i = 0; i < exceptionConstructor.Fields.Count; i++) {
						MatchResult res = this.subpatterns[i].Match(exceptionValue.items[i], scope);
						if (!res.Success) {
							return res;
						}
					}

					return MatchResult.True;
				}

				return new MatchResult {
					Success = false,
					Note = $"can not deconstruct a value of type {exceptionValue.Type}"
				};
			}

			if (requiredType is Constructor ctor && value is Instance instance) {
				if (ctor.IsParentOf(value)) {
					for (Int32 i = 0; i < ctor.Fields.Count; i++) {
						MatchResult res = this.subpatterns[i].Match(instance.Items[i], scope);
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

			return new MatchResult { Success = false };
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return $"({this.constructor} {String.Join(" ", this.subpatterns)})";
		}
	}
}