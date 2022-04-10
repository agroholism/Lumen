using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;

namespace Lumen.Lang.Patterns {
	/// <summary> Pattern @[..., ...] </summary>
	internal class ArrayPattern : IPattern {
		private List<IPattern> subpatterns;

		public ArrayPattern(List<IPattern> patterns) {
			this.subpatterns = patterns;
		}

		public MatchResult Match(IValue value, Scope scope) {
			if (value is MutArray array) {
				List<IValue> castedArray = array.ToList(scope);

				if (castedArray.Count != this.subpatterns.Count) {
					return new MatchResult(
						MatchResultKind.Fail,
						$"function wait an array with length {this.subpatterns.Count}"
					);
				}

				Int32 index = 0;
				foreach (IValue i in castedArray) {
					MatchResult result = this.subpatterns[index].Match(i, scope);
					if (!result.IsSuccess) {
						return result;
					}
					index++;
				}
				return MatchResult.Success;
			}

			return new MatchResult(
				MatchResultKind.Fail,
				$"function wait an array with length {this.subpatterns.Count}"
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
			return new ArrayPattern(this.subpatterns.Select(i => i.Closure(manager)).ToList());
		}

		public override String ToString() {
			return $"@[{String.Join(", ", this.subpatterns)}]";
		}
	}
}