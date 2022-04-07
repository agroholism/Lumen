﻿using System;
using System.Collections;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class CustomFlow : IEnumerable<Value> {
		private Expression generatorBody;
		private Scope associatedScope;

		public CustomFlow(Expression generatorBody, Scope associatedScope) {
			this.generatorBody = generatorBody;
			this.associatedScope = associatedScope;
		}

		private IEnumerable<Value> Run(Scope scope) {
			IEnumerator<Value> enumerator = this.generatorBody.EvalWithYield(scope).GetEnumerator();

			while (true) {
				Value exitValue = null;

				try {
					Boolean canMove = enumerator.MoveNext();

					if (!canMove) {
						yield break;
					}
				}
				catch (Return ret) {
					exitValue = ret.Result;
				}

				if (exitValue != null) {
					scope.Bind(Constants.YIELD_RESULT_SPECIAL_NAME, exitValue);
					yield break;
				}

				if(enumerator.Current is GeneratorExpressionTerminalResult terminalResult) {
					scope.Bind(Constants.YIELD_RESULT_SPECIAL_NAME, terminalResult);
					yield break;
				}

				yield return enumerator.Current;
			}
		}

		public IEnumerator<Value> GetEnumerator() {
			Scope scope = new Scope(this.associatedScope);
			return new FlowAutomat(this.Run(scope).GetEnumerator(), scope);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}