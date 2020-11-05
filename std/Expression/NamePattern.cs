using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lumen.Lang.Expressions {
	public class NamePattern : IPattern {
		private readonly String identifier;



		public NamePattern(String identifier) {
			this.identifier = identifier;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.identifier);

			return this;
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
			return new List<String> { this.identifier };
		}

		public MatchResult Match(Value value, Scope scope) {
			scope[this.identifier] = value;
			return MatchResult.True;
		}

		public override String ToString() {
			return this.identifier.ToString();
		}
	}

	internal class TypePattern : IPattern {
		private IPattern subpattern;
		private IType requirement;

		public TypePattern(String subpattern, IType requirement) {
			this.subpattern = new NamePattern(subpattern);
			this.requirement = requirement;
		}

		public TypePattern(IPattern subpattern, IType requirement) {
			this.subpattern = subpattern;
			this.requirement = requirement;
		}

		public Expression Closure(ClosureManager manager) {
			return new TypePattern(this.subpattern.Closure(manager) as IPattern, this.requirement);
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public MatchResult Match(Value value, Scope scope) {
			if (this.requirement.IsParentOf(value)) {
				return this.subpattern.Match(value, scope);
			}

			return new MatchResult {
				Success = false,
				Note = $"wait value of type {requirement} given {value.Type}"
			};
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return $"({this.subpattern}: {this.requirement})";
		}
	}
}
