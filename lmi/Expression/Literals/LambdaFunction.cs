using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    public class LambdaFunction : Expression {
        public List<IPattern> arguments;
        public Expression body;
 
        public LambdaFunction(List<IPattern> arguments, Expression body) {
            this.arguments = arguments;
            this.body = body;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new LambdaFunction(this.arguments.Select(i => i.Closure(visible, thread) as IPattern).ToList(), this.body.Closure(visible, thread));
        }

        public Value Eval(Scope e) {
            List<String> s = new List<String>() { "self", "_" };

            return new UserFun(this.arguments, this.body.Closure(s, e));
        }

        public override String ToString() {
            String result = "(";

            return (result.Length > 2 ? result.Substring(0, result.Length - 2) : result) + ") => " + this.body.ToString();
        }
    }
}