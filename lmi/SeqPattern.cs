using Lumen.Lang;
using Lumen.Lang.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lmi {
	internal class SeqPattern : IPattern {
		private List<IPattern> subpatterns;

		public SeqPattern(List<IPattern> patterns) {
			this.subpatterns = patterns;
		}

		public MatchResult Match(IValue value, Scope scope) {
			if (value is Flow flow) {
				IEnumerable<IValue> castedSeq = flow.ToSeq(scope);

				IEnumerator<IValue> enumerator = castedSeq.GetEnumerator();

				for (Int32 i = 0; i < this.subpatterns.Count; i++) {
					if (!enumerator.MoveNext()) {
						return new MatchResult(
							MatchResultKind.Fail,
							$"function wait a sequence with length {this.subpatterns.Count}"
						);
					}

					MatchResult result = this.subpatterns[i].Match(enumerator.Current, scope);
					if (!result.IsSuccess) {
						return result;
					}
				}

				return MatchResult.Success;
			}

			return new MatchResult(
				MatchResultKind.Fail,
				$"function wait a sequence with length {this.subpatterns.Count}"
			);
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			foreach (IPattern pattern in this.subpatterns) {
				result.AddRange(pattern.GetDeclaredVariables());
			}

			return result;
		}

		public IPattern Closure(ClosureManager manager) {
			return new SeqPattern(this.subpatterns.Select(i => i.Closure(manager)).ToList());
		}

		public override String ToString() {
			return $"({String.Join(", ", this.subpatterns)})";
		}
	}
}