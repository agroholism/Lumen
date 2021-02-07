using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
	public class ContextPattern : IPattern {
		internal readonly String identifier;
		internal readonly List<Expression> implements;

		public ContextPattern(String identifier) : this(identifier, new List<Expression>()) {
		}

		public ContextPattern(String identifier, List<Expression> implements) {
			this.identifier = identifier;
			this.implements = implements;
		}

		public IPattern Closure(ClosureManager manager) {
			manager.Declare(this.identifier);

			return new ContextPattern(this.identifier, this.implements.Select(i => i.Closure(manager)).ToList());
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
				foreach (Class i in this.implements.Select(i => i.Eval(scope) as Class)) {
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
}
