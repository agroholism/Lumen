using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Light {
    internal class Applicate : Expression {
        public Expression callable;
        public List<Expression> argumentsExpression;
        public Int32 line;
        public String fileName;

        public Applicate(Expression callable, List<Expression> argumentsExpression, Int32 line, String fileName) {
            this.callable = callable;
            this.argumentsExpression = argumentsExpression;
            this.line = line;
            this.fileName = fileName;
        }

        public Value Eval(Scope e) {
            Value callable = this.callable.Eval(e);

            switch (callable) {
                case Fun _:
                    return this.EvalFun((Fun)callable, e);
				case SingletonConstructor _:
					return callable;
				case Module m:
					return this.EvalFun((Fun)m.GetField("defaultConstructor", e), e);
				default:
                    throw new LumenException($"can not call a value of type {callable.Type}") {
                        line = line,
                        file = fileName
                    };
            }
        }

        internal Value EvalFun(Fun function, Scope e) {
            Scope innerScope = new Scope(e) {
                ["self"] = function
            };

            try {
                Boolean isPartial = this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_");

                if (isPartial) {
                    return this.MakePartial(function, e);
                }

                Value[] arguments = this.argumentsExpression.Select(i => i.Eval(e)).ToArray();

                return this.ProcessCall(e, innerScope, function, arguments);
            } catch (GotoE ex) {
TAIL_RECURSION:
                try {
                    return this.ProcessCall(e, innerScope, function, ex.result);
                } catch (GotoE gt) {
                    ex = gt;
                    goto TAIL_RECURSION;
                }
            }
        }

        private class PartialFun : Fun {
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
                List<Value> vals = new List<Value>();
                vals.AddRange(args);

                for (Int32 i = 0; i < this.Args.Length; i++) {
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

        private Value ProcessCall(Scope parent, Scope scope, Fun function, Value[] args) {
            if (function is FunctionDeclaration.Dispatcher dis) {
                if (dis.functions.First().Key.Count > args.Length) {
                    return new PartialFun {
                        InnerFunction = function,
                        Args = args,
                        restArgs = dis.functions.First().Key.Count - args.Length
                    };
                }
            }

            scope.Bind("self", function);

            try {
                return function.Run(scope, args);
            } catch (LumenException lex) {
                if (lex.file == null) {
                    lex.file = this.fileName;
                }

                if (lex.line == -1) {
                    lex.line = this.line;
                }

                if (lex.functionName == null) {
                    if(parent.ExistsInThisScope("self")) {
                        lex.functionName = (parent["self"] as Fun).Name;
                    }
                } else {
                    if (parent.ExistsInThisScope("self")) {
                        Fun ff = parent["self"] as Fun;

                        lex.AddToCallStack(ff.Name, this.fileName, this.line);
                    } else {
                        lex.AddToCallStack(null, this.fileName, this.line);
                    }
                }

                throw;
            }
        }

        private Value MakePartial(Fun function, Scope parent) {
            List<IPattern> a = new List<IPattern>();

            List<Expression> nexps = new List<Expression>();
            Int32 x = 0;
            foreach (Expression exp in this.argumentsExpression) {
                if (exp is IdExpression ide && ide.id == "_") {
                    a.Add(new NamePattern("#x" + x));
                    nexps.Add(new IdExpression("#x" + x, ide.line, ide.file));
                    x++;
                } else {
                    nexps.Add(new ValueE(exp.Eval(parent)));
                }
            }

            UserFun f = new UserFun(a, new Applicate(new ValueE(function), nexps, this.line, this.fileName)) {
                Name = function.Name + "'"
            };

            return f;
        }

        public override String ToString() {
            return "(" + this.callable.ToString() + " " + String.Join(" ", this.argumentsExpression) + ")";
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new Applicate(this.callable.Closure(visible, thread), this.argumentsExpression.Select(i => i.Closure(visible, thread)).ToList(), this.line, this.fileName);
        }
    }
}