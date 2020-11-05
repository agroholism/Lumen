using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class RangeOperator : Expression {
		private Expression stepExpression;
		private Expression startExpression;
		private Expression endExpression;
		private Boolean isInclusive;
		private Int32 line;
		private String file;

		public RangeOperator(Expression startExpression, Expression endExpression, Boolean isInclusive, Int32 line, String file) {
			this.startExpression = startExpression;
			this.endExpression = endExpression;
			this.isInclusive = isInclusive;
			this.line = line;
			this.file = file;
		}

		public RangeOperator(Expression startExpression, Expression endExpression, Expression step, Boolean isInclusive, Int32 line, String file)
			: this(startExpression, endExpression, isInclusive, line, file) {
			this.stepExpression = step;
		}

		public Expression Closure(ClosureManager manager) {
			return new RangeOperator(this.startExpression?.Closure(manager),
				this.endExpression?.Closure(manager), this.stepExpression?.Closure(manager),
				this.isInclusive, this.line, this.file);
		}

		public Value Eval(Scope scope) {
			Value startValue = this.startExpression?.Eval(scope) ?? Const.UNIT;
			Value endValue = this.endExpression?.Eval(scope) ?? Const.UNIT;

			Value stepValue = this.stepExpression?.Eval(scope);

			if (startValue == Const.UNIT) {
				if (endValue == Const.UNIT) {
					if (stepValue != null) {
						return new InfinityRange(stepValue.ToDouble(scope));
					}

					return new InfinityRange();
				}

				Value result =
						new Applicate(new DotOperator(
						new ValueLiteral(endValue.Type), this.isInclusive ? Constants.RANGE_INCLUSIVE : Constants.RANGE_EXCLUSIVE,
						this.file, this.line), new List<Expression> { new ValueLiteral(startValue), new ValueLiteral(endValue) }, this.line, this.file).Eval(scope);

				if (stepValue != null) {
					result =
					   new Applicate(new DotOperator(
					   new ValueLiteral(result.Type), Constants.SLASH,
					   this.file, this.line), new List<Expression> { new ValueLiteral(result), new ValueLiteral(stepValue) }, this.line, this.file).Eval(scope);
				}

				return result;
			}

			Value _result = new Applicate(new DotOperator(
				new ValueLiteral(startValue.Type), this.isInclusive ? Constants.RANGE_INCLUSIVE : Constants.RANGE_EXCLUSIVE,
				this.file, this.line), new List<Expression> { new ValueLiteral(startValue), new ValueLiteral(endValue)
				}, this.line, this.file).Eval(scope);

			if (stepValue != null) {
				_result =
				   new Applicate(new DotOperator(
				   new ValueLiteral(_result.Type), Constants.SLASH,
				   this.file, this.line), new List<Expression> { new ValueLiteral(_result), new ValueLiteral(stepValue) }, this.line, this.file).Eval(scope);
			}

			return _result;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}

		public void AddStep(Expression stepExpression) {
			this.stepExpression = stepExpression;
		}
	}
}