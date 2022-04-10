#nullable enable

using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
	internal class RangePattern : IPattern {
		private readonly IPattern? left;
		private readonly IPattern? right;
		private readonly Boolean isInclusive;

		public RangePattern(IPattern? left, IPattern? right, Boolean isInclusive) {
			this.left = left;
			this.right = right;
			this.isInclusive = isInclusive;
		}

		public MatchResult Match(IValue value, Scope scope) {
			IValue? leftValue = (this.left as ValuePattern)?.Value;
			IValue? rightValue = (this.right as ValuePattern)?.Value;

			if (leftValue == null && rightValue == null) {
				return MatchResult.Success;
			}

			if (leftValue == null) {
				try {
					Fun comparator = rightValue!.Type.GetMember("compare", scope).ToFunction(scope);

					Double rightResult = comparator.Call(scope, value, rightValue).ToDouble(scope);

					Boolean result = this.isInclusive ? rightResult <= 0 : rightResult < 0;

					if (result) {
						return MatchResult.Success;
					}
				}
				catch {

				}

				return new MatchResult (
					MatchResultKind.Fail,
					$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				);
			}

			if (rightValue == null) {
				try {
					Fun comparator = leftValue.Type.GetMember("compare", scope).ToFunction(scope);

					Boolean result = comparator.Call(new Scope(scope), leftValue, value).ToDouble(scope) <= 0;

					if (result) {
						return MatchResult.Success;
					}
				}
				catch {

				}

				return new MatchResult(
					MatchResultKind.Fail,
					$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				);
			}

			try {
				Fun comparator = leftValue.Type.GetMember("compare", scope).ToFunction(scope);

				Double leftResult = comparator.Call(new Scope(scope), leftValue, value).ToDouble(scope);
				Double rightResult = comparator.Call(scope, value, rightValue).ToDouble(scope);

				Boolean result = leftResult <= 0 && (this.isInclusive ? rightResult <= 0 : rightResult < 0);

				if (result) {
					return MatchResult.Success;
				}
			}
			catch {

			}

			return new MatchResult (
				MatchResultKind.Fail,
				$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
			);
		}


		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public IPattern Closure(ClosureManager manager) {
			return new RangePattern(this.left?.Closure(manager), this.right?.Closure(manager), this.isInclusive);
		}

		public override String ToString() {
			return $"{this.left}{(this.isInclusive ? "..." : "..")}{this.right}";
		}
	}
}