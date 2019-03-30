using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    public class UserFun : Fun {
        public Expression Body;
        public List<IPattern> Arguments { get; set; } = new List<IPattern>();
        public String Name { get; set; }

        public IObject Type => Prelude.Function;

        public UserFun(List<IPattern> arguments, Expression Body) {
            this.Arguments = arguments;
            this.Body = Body;
        }

        public String ToString(Scope scope) {
            return this.Name ?? "anonymousFunction#" + this.GetHashCode();
        }

        public override String ToString() {
            return this.Name ?? "anonymousFunction#" + this.GetHashCode();
        }

        public Value Run(Scope e, params Value[] args) {
            if (this.Arguments.Count > args.Length) {
                return this.MakePartial(args);
            }

            Int32 counter = 0;

            foreach (IPattern i in this.Arguments) {
                if (!i.Match(args[counter], e)) {
                    throw new LumenException($"function with signature {String.Join(" ", this.Arguments)} can not be applied");
                }
                counter++;
            }

            try {
                return this.Body.Eval(e);
            } catch (Return rt) {
                return rt.Result;
            }
        }

        private Value MakePartial(Value[] vals) {
            return new PartialFun {
                InnerFunction = this,
                Args = vals,
                restArgs = this.Arguments.Count - vals.Length
            };
        }

        public Int32 CompareTo(Object obj) {
            return 0;
        }

        public Value Clone() {
            return this;
        }
    }
}
