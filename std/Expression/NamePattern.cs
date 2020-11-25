using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Expressions {
	public class GenericLater : Module {
		public static Value Instance { get; private set; } = new GenericLater();
	}

	public class ContextPattern : IPattern {
		internal readonly String identifier;
		internal readonly List<Expression> implements;

		public ContextPattern(String identifier) : this(identifier, new List<Expression>()) {
		}

		public ContextPattern(String identifier, List<Expression> implements) {
			this.identifier = identifier;
			this.implements = implements;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.identifier);

			return new ContextPattern(this.identifier, this.implements.Select(i => i.Closure(manager)).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return new List<String> { this.identifier };
		}

		public MatchResult Match(Value value, Scope scope) {
			if (value is GenericLater) {
				return new MatchResult (
					MatchResultKind.Fail,
					$"impossible to infer the type of context {this.identifier}|Try to specify the type of context { this.identifier } in call place"
				);
			}

			if (value is not IType itype) {
				scope[this.identifier] = GenericLater.Instance;

				return MatchResult.Delayed;
			}
			else {
				foreach (Module i in this.implements.Select(i => i.Eval(scope) as Module)) {
					if (!itype.HasImplementation(i)) {
						return new MatchResult (
							MatchResultKind.Fail,
							 $"context {this.identifier} requires a type that implements the class {i} given type {value}|"
							+ $"Probably you can implement class {i} for type {itype}?"
						);
					}

				}
			}

			scope[this.identifier] = value;
			return MatchResult.Success;
		}

		public override String ToString() {
			return $".<{this.identifier}{(this.implements.Count > 0 ? " implements " + String.Join(", ", this.implements) : "")}>";
		}

		public void AddImplements(Expression expression) {
			this.implements.Add(expression);
		}
	}

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
			if(scope.ExistsInThisScope(this.identifier)) {
				Console.WriteLine($"WARNING: there are rebinding {this.identifier} please don't use rebindings");
			}

			scope[this.identifier] = value;
			return MatchResult.Success;
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

			return new MatchResult (
				MatchResultKind.Fail,
				$"wait value of type {this.requirement} given {value.Type}"
			);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return $"({this.subpattern}: {this.requirement})";
		}
	}

	internal class TypesPattern : IPattern {
		private IPattern subpattern;
		private List<IType> requirements;

		public TypesPattern(String subpattern, List<IType> requirements) {
			this.subpattern = new NamePattern(subpattern);
			this.requirements = requirements;
		}

		public TypesPattern(IPattern subpattern, List<IType> requirements) {
			this.subpattern = subpattern;
			this.requirements = requirements;
		}

		public Expression Closure(ClosureManager manager) {
			return new TypesPattern(this.subpattern.Closure(manager) as IPattern, this.requirements);
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public MatchResult Match(Value value, Scope scope) {
			if(this.requirements.All(i => i.IsParentOf(value))) {
				return this.subpattern.Match(value, scope);
			}

			return new MatchResult(
				MatchResultKind.Fail,
				$"wait value of type {String.Join(", ", this.requirements)} given {value.Type}"
			);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return $"({this.subpattern}: {String.Join(", ", this.requirements)})";
		}
	}
}
