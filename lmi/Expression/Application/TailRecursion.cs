using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class TailRecursion : Expression {
        private List<Expression> argumentsExpression;
        private String fileName;
        private Int32 lineNumber;

        public TailRecursion(List<Expression> argumentsExpression, String fileName, Int32 lineNumber) {
            this.argumentsExpression = argumentsExpression;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
        }

        public Value Eval(Scope e) {
            throw new Return(new Tailrec(this.argumentsExpression.Select(i => i.Eval(e)).ToArray()));
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new LumenException("evaluating tailrec in generator");
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasTailRecursion = true;
			return new TailRecursion(
				this.argumentsExpression.Select(i => i.Closure(manager)).ToList(),
				this.fileName, this.lineNumber);
		}

		public override String ToString() {
			return "tailrec " + Utils.ArgumentsToString(this.argumentsExpression);
		}

		public class Tailrec : Exception, Value {
			public Value[] newArguments;

			public Tailrec(Value[] result) {
				this.newArguments = result;
			}

			public IType Type => throw new NotImplementedException();

			public Int32 CompareTo(Object obj) {
				throw new NotImplementedException();
			}

			public String ToString(String format, IFormatProvider formatProvider) {
				throw new NotImplementedException();
			}
		}
	}
}