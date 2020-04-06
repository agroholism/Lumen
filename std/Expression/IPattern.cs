using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public interface IPattern : Expression {
		MatchResult Match(Value value, Scope scope);

        List<String> GetDeclaredVariables();
    }

	public class MatchResult {
		public Boolean Success { get; set; }
		public String Note { get; set; }

		public static MatchResult True { get; } = new MatchResult { Success = true };
	}
}
