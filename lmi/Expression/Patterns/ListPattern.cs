using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	/// <summary> Pattern [..., ...] </summary>
	internal class ListPattern : IPattern {
        private List<IPattern> subpatterns;
		public Boolean IsNotEval => false;

		public ListPattern(List<IPattern> patterns) {
            this.subpatterns = patterns;
        }

        public MatchResult Match(Value value, Scope scope) {
            if (value is List) {
				List list = value as List;

                if(LinkedList.Count(list.value) != this.subpatterns.Count) {
                    return new MatchResult {
						Success = false,
						Note = $"function wait a list with length {this.subpatterns.Count}"
					};
                }

                Int32 index = 0;
                foreach(Value i in list.value) {
					MatchResult result = this.subpatterns[index].Match(i, scope);
					if (!result.Success) {
                        return result;
                    }
                    index++;
                }
                return MatchResult.True;
            }

            return new MatchResult {
				Success = false,
				Note = $"function wait a list with length {this.subpatterns.Count}"
			};
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			foreach (IPattern pattern in this.subpatterns) {
				result.AddRange(pattern.GetDeclaredVariables());
			}

			return result;
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public Expression Closure(ClosureManager manager) {
			return new ListPattern(this.subpatterns.Select(i => i.Closure(manager) as IPattern).ToList());
		}

		public override String ToString() {
            return $"[{String.Join(", ", this.subpatterns)}]";
        }
    }
}