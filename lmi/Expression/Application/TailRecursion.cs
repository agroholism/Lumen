using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class TailRecursion : Expression {
        private List<Expression> args;
        private System.String fileName;
        private System.Int32 line;

        public TailRecursion(List<Expression> args, System.String fileName, System.Int32 line) {
            this.args = args;
            this.fileName = fileName;
            this.line = line;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new GotoE(this.args.Select(i => i.Eval(e)).ToArray());
        }
    }
}