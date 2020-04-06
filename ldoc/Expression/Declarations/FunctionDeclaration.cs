using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections;

namespace ldoc {
    public class FunctionDeclaration : Expression {
        public String NameFunction;
        public List<IPattern> arguments;
        public Expression Body;
		public Boolean isLazy;
        public Int32 line;
        public String file;

        public FunctionDeclaration(String NameFunction, List<IPattern> Args, Expression Body, Boolean isLazy) {
            this.NameFunction = NameFunction;
            this.arguments = Args;
            this.Body = Body;
			this.isLazy = isLazy;
        }

        public FunctionDeclaration(String name, List<IPattern> arguments, Expression body, Boolean isLazy, Int32 line, String file) : this(name, arguments, body, isLazy) {
            this.line = line;
            this.file = file;
        }

        public override String ToString() {
            String result = "let " + this.NameFunction + "(" + String.Join(", ", this.arguments) + ")" + "{" + this.Body + "}";
            return result;
        }

        public Value Eval(Scope e) {
			ClosureManager manager = new ClosureManager(e);
			manager.Declare(new List<String> {"_"});

            foreach (IPattern i in this.arguments) {
				manager.Declare(i.GetDeclaredVariables());
            }

            Fun v = new UserFun(this.arguments.Select(i => 
			i.Closure(manager) as IPattern).ToList(), this.Body?.Closure(manager)) {
                Name = this.NameFunction,
           };

			Fun ff = v;

            if (e.ExistsInThisScope(this.NameFunction) && e[this.NameFunction] is Dispatcher dis) {
                dis.Append(v);
            } else if(e.ExistsInThisScope(this.NameFunction) && e[this.NameFunction] is Fun f) {
                Dispatcher disp = new Dispatcher();
                disp.Name = this.NameFunction;
                disp.Append(f);
                disp.Append(v);
                e.Bind(this.NameFunction, disp);
            }
            else {
                e.Bind(this.NameFunction, v);
            }

            return v;
        }
		// add partials
		public class Dispatcher : Fun {
            internal Dictionary<List<IPattern>, Fun> functions = new Dictionary<List<IPattern>, Fun>();
			public Boolean IsLazy { get; set; }
			public String Name { get; set; }

            public String ToString(Scope scope) {
                return "dis";
            }

            public Dictionary<String, Value> Attributes { get; } = new Dictionary<String, Value>();
            public List<IPattern> Arguments { get; set; }

            public IType Type => throw new NotImplementedException();

            public IType Parent { get; set; }

            public void Append(Fun f) {
                this.functions[f.Arguments] = f;
            }

            public Boolean IsParentOf(Value value) {
                if (value is IType parent) {
                    while (true) {
                        if (parent.TryGetMember("@prototype", out Value v)) {
                            parent = v as IType;
                            if (parent == this) {
                                return true;
                            }
                        } else {
                            break;
                        }
                    }
                }
                return false;
            }

            public Value Run(Scope e, params Value[] args) {
                foreach (KeyValuePair<List<IPattern>, Fun> i in this.functions) {
                    if(i.Key.Count != args.Length) {
                        continue;
                    }

                    Int32 counter = 0;

                    Scope s = new Scope(e);

                    Boolean ok = false;

                    foreach (IPattern j in i.Key) {
						var m = j.Match(args[counter], s);

						if (!m.Success) {
                            ok = true;
                            break;
                        }
                        counter++;
                    }

                    if (ok) {
                        continue;
                    }

                    Value result = null;
                    try {
                        result = i.Value.Run(e, args);
                    } catch (Return rt) {
                        result = rt.Result;
                    }

                    return result;
                }

                throw new LumenException($"can not to find overload") {
                    Note = $"Aviable overloads: {Environment.NewLine}{String.Join(Environment.NewLine, this.functions.Select(i => $"let { this.Name } {String.Join(" ", i.Key)} = ..."))}"
                };
            }

            public Int32 CompareTo(Object obj) {
                return 0;
            }

            public Value Clone() {
                return (Value)this.MemberwiseClone();
            }

            public Value GetField(String name, Scope e) {
                if (this.Attributes.TryGetValue(name, out Value result)) {
                    return result;
                }

                if (this.Parent != null) {
                    if (this.Parent.TryGetMember(name, out result)) {
                        return result;
                    }
                }

                throw new LumenException($"Module does not contains a field {name}");
            }

            public void SetField(String name, Value value, Scope e) {
                this.Attributes[name] = value;
            }

            public Boolean TryGetField(String name, out Value result) {
                result = null;

                if (this.Attributes.TryGetValue(name, out result)) {
                    return true;
                }

                if (this.Parent != null) {
                    if (this.Parent.TryGetMember(name, out result)) {
                        return true;
                    }
                }

                return false;
            }

			public String ToString(String format, IFormatProvider formatProvider) {
				return this.ToString(null);
			}
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.NameFunction);
            return new FunctionDeclaration(this.NameFunction, this.arguments.Select(i => i.Closure(manager) as IPattern).ToList(), this.Body.Closure(manager), this.isLazy, this.line, this.file);
        }
    }
}