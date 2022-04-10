using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
	public interface IPattern {
		MatchResult Match(IValue value, Scope scope);

		List<String> GetDeclaredVariables();

		IPattern Closure(ClosureManager manager);
	}

	public enum MatchResultKind {
		Success,
		Fail,
		Delayed
	}

	public class MatchResult {
		public MatchResultKind Kind { get; private set; }
		public String Note { get; private set; }

		public Boolean IsSuccess => this.Kind == MatchResultKind.Success;
		public Boolean IsFail => this.Kind == MatchResultKind.Fail;
		public Boolean IsDelayed => this.Kind == MatchResultKind.Delayed;

		public MatchResult() {
			this.Kind = MatchResultKind.Fail;
		}

		public MatchResult(MatchResultKind kind) {
			this.Kind = kind;
		}

		public MatchResult(MatchResultKind kind, String note) {
			this.Kind = kind;
			this.Note = note;
		}

		public MatchResult(Boolean isSuccess) {
			this.Kind = isSuccess ? MatchResultKind.Success : MatchResultKind.Fail;
		}

		public MatchResult(Boolean isSuccess, String note) {
			this.Kind = isSuccess ? MatchResultKind.Success : MatchResultKind.Fail;
			this.Note = note;
		}

		public static MatchResult Success { get; } = new MatchResult(MatchResultKind.Success);
		public static MatchResult Delayed { get; } = new MatchResult(MatchResultKind.Delayed);
	}
}
