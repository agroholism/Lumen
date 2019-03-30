using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    public sealed class LambdaFun : Fun {
        public LumenFunc _lambda;

        public String Name { get; set; }

        public List<IPattern> Arguments { get; set; } = new List<IPattern>();

        public IObject Type { get => Prelude.Function; }

        public LambdaFun(LumenFunc lambda) {
            this._lambda = lambda;
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
               return this._lambda(e, args);
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

        public String ToString(Scope scope) {
            return this.Name ?? "anonymousFunction#" + this.GetHashCode();
        }

        public override String ToString() {
            return this.Name ?? "anonymousFunction#" + this.GetHashCode();
        }
    }

    public class PartialFun : Fun {
        public Fun InnerFunction { get; set; }
        public Value[] Args { get; set; }
        public Int32 restArgs;

        public String Name { get; set; }
        public EntityAttribute Attribute { get; set; }
        public List<IPattern> Arguments { get; set; }
        public IObject Parent { get; set; }

        public IObject Type => Prelude.Function;

        public Value Clone() {
            return this;
        }

        public Int32 CompareTo(Object obj) {
            throw new NotImplementedException();
        }

        public Value GetField(String name, Scope scope) {
            throw new NotImplementedException();
        }

        public Boolean IsParentOf(Value value) {
            throw new NotImplementedException();
        }

        public Value Run(Scope e, params Value[] args) {
            if (this.restArgs > args.Length) {
                return new PartialFun {
                    InnerFunction = this,
                    Args = args,
                    restArgs = this.restArgs - args.Length
                };
            }

            List<Value> vals = new List<Value>();
            vals.AddRange(args);

            for (var i = 0; i < this.Args.Length; i++) {
                vals.Insert(i, this.Args[i]);
            }

            return this.InnerFunction.Run(e, vals.ToArray());
        }

        public void SetField(String name, Value value, Scope scope) {
            throw new NotImplementedException();
        }

        public String ToString(Scope e) {
            return "partial";
        }

        public Boolean TryGetField(String name, out Value result) {
            throw new NotImplementedException();
        }
    }
}
