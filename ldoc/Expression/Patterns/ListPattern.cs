using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace ldoc {
	internal class ListPattern : IPattern {
        private List<IPattern> patterns;
		public Boolean IsNotEval => false;
		public ListPattern(List<IPattern> patterns) {
            this.patterns = patterns;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return new ListPattern(this.patterns.Select(i => i.Closure(manager) as IPattern).ToList());
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String>();

            foreach(IPattern i in this.patterns) {
                res.AddRange(i.GetDeclaredVariables());
            }

            return res;
        }

        public MatchResult Match(Value value, Scope scope) {
			List list;

			if(value is Lazy lazy) {
				Value val = lazy.Force();
				if(val is List) {
					list = val as List;
				} else {
					return new MatchResult {
						Success = false,
						Note = $"function wait a List with length {this.patterns.Count}"
					};
				}
			}

            if (value is List) {
				list = value as List;
                if(LinkedList.Count(list.value) != this.patterns.Count) {
                    return new MatchResult {
						Success = false,
						Note = $"function wait a List with length {this.patterns.Count}"
					};
                }

                Int32 index = 0;
                foreach(Value i in list.value) {
					MatchResult result = this.patterns[index].Match(i, scope);
					if (!result.Success) {
                        return result;
                    }
                    index++;
                }
                return MatchResult.True;
            }

            return new MatchResult {
				Success = false,
				Note = $"function wait a List with length {this.patterns.Count}"
			};
		}

        public override String ToString() {
            return $"[{String.Join("; ", this.patterns)}]";
        }
    }
}