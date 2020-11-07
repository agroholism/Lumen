using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class DispatcherFunction : Fun {
		private List<Fun> targets;
		public String Name { get; set; }
		public List<IPattern> Parameters { get; set; }

		public IType Type => Prelude.Function;

		public DispatcherFunction(String name, Fun target) {
			this.Name = name;
			this.targets = new List<Fun> { target };
			this.Parameters = new List<IPattern>();

			for (Int32 i = 0; i < target.Parameters.Count; i++) {
				this.Parameters.Add(new NamePattern($"[dispatcher-function-argument #{1}]"));
			}
		}

		public DispatcherFunction(String name, Fun firstTarget, Fun secondTarget) : this(name, firstTarget) {
			this.AppendTargetFunction(secondTarget);
		}

		public void AppendTargetFunction(Fun targetFunction) {
			if (!this.targets.Contains(targetFunction)) {
				if (!this.IsCompatibleTarget(targetFunction)) {
					throw Helper.InvalidOperation("it is impossible to create such overload");
				}

				this.targets.Add(targetFunction);
			}
		}

		/// <summary>
		/// Checks if function is compatible to other target functions.
		/// </summary>
		/// <remarks>Compatible means that functions have same parameters count.</remarks>
		/// <param name="targetFunction">Potential target.</param>
		/// <returns>true if targetFunction is compatible.</returns>
		public Boolean IsCompatibleTarget(Fun targetFunction) {
			return targetFunction.Parameters.Count == this.Parameters.Count;
		}

		public Value Call(Scope scope, params Value[] arguments) {
tail_recursion_entry:

			List<String> notes = new List<String>();

			foreach (Fun potentialTarget in this.targets) {
				// In this scope matches arguments with parameters of potential targets
				Scope argumentsTestingScope = new Scope(scope);
				// This flag means all matches is passed and current function is actual target
				Boolean itIsActualTarget = true;

				Int32 contextParametersOffset = 0;
				Int32 currentParameterIndex = 0;

				while (currentParameterIndex < potentialTarget.Parameters.Count) {
					// If count of arguments less then (count of parameters - count of contexts)
					if (currentParameterIndex - contextParametersOffset >= arguments.Length) {
						return Helper.MakePartial(this, arguments);
					}

					MatchResult match =
						potentialTarget.Parameters[currentParameterIndex].Match(arguments[currentParameterIndex - contextParametersOffset], argumentsTestingScope);

					if (match.IsDelayed) {
						// Just going to next parameter, but remember that there are delayed
						contextParametersOffset++;
						currentParameterIndex++;
						continue;
					}

					if (match.IsFail) {
						// Just specific notes for ContextPattern
						if (potentialTarget.Parameters[currentParameterIndex] is ContextPattern) {
							String[] strs = match.Note.Split('|');

							notes.Add(strs[0] +
								$".{Environment.NewLine}\t\t{(contextParametersOffset > 0 ? "This is most likely reason. " : "")}" + strs[1]);
						}
						else {
							notes.Add(match.Note);
						}

						// This target is not fit
						itIsActualTarget = false;
						break;
					}

					currentParameterIndex++;
				}

				// If there are delayed patterns
				while (contextParametersOffset > 0) {
					ContextPattern contextPattern =
						potentialTarget.Parameters[contextParametersOffset - 1] as ContextPattern;

					MatchResult match =
						contextPattern.Match(argumentsTestingScope[contextPattern.identifier], argumentsTestingScope);

					if (!match.IsSuccess) {
						String[] strs = match.Note.Split('|');

						notes.Add(strs[0] +
							$".{Environment.NewLine}\t\tThis is most likely reason. " + strs[1]);

						itIsActualTarget = false;
						break;
					}

					contextParametersOffset--;
				}

				if (!itIsActualTarget) {
					continue;
				}

				Value functionCallResult =
					potentialTarget.Call(scope, arguments);

				// Returning TailRecursion means tailrec call
				if (functionCallResult is TailRecursion tailRecursion) {
					arguments = tailRecursion.newArguments;
					goto tail_recursion_entry;
				}

				return functionCallResult;
			}

			IEnumerable<String> overloads =
				this.targets.Zip(notes,
					(i, j) => $"{Environment.NewLine}\tlet { this.Name } {String.Join(" ", i.Parameters)}{Environment.NewLine}{Environment.NewLine}\t\tDoes not fit owing to {j}.");

			throw new LumenException($"no function found that take such arguments") {
				Note = $"Aviable functions: {Environment.NewLine}{String.Join(Environment.NewLine, overloads)}"
			};
		}

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}

		public override String ToString() {
			return $"[Function #{this.GetHashCode()}]";
		}
	}
}
