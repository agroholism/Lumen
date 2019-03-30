using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace Lumen.Light {
    public class FunctionDeclaration : Expression {
        public String NameFunction;
        public List<IPattern> arguments;
        public Expression Body;
        public Int32 line;
        public String file;

        public FunctionDeclaration(String NameFunction, List<IPattern> Args, Expression Body) {
            this.NameFunction = NameFunction;
            this.arguments = Args;
            this.Body = Body;
        }

        public FunctionDeclaration(String name, List<IPattern> arguments, Expression body, Int32 line, String file) : this(name, arguments, body) {
            this.line = line;
            this.file = file;
        }

        public override String ToString() {
            String result = "let " + this.NameFunction + "(" + String.Join(", ", this.arguments) + ")" + "{" + this.Body + "}";
            return result;
        }

        public Value Eval(Scope e) {
            List<String> notClosurableVariables = new List<String> {
                "self",
                "_",
                "this",
                "args"
            };

            foreach (IPattern i in this.arguments) {
                notClosurableVariables.AddRange(i.GetDeclaredVariables());
            }

            UserFun v = new UserFun(this.arguments, this.Body?.Closure(notClosurableVariables, e));

            v.Name = this.NameFunction;

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

        public class Dispatcher : Fun {
            internal Dictionary<List<IPattern>, Fun> functions = new Dictionary<List<IPattern>, Fun>();

            public String Name { get; set; }

            public String ToString(Scope scope) {
                return "dis";
            }

            public EntityAttribute Attribute { get; set; }
            public Dictionary<String, Value> Attributes { get; } = new Dictionary<String, Value>();
            public List<IPattern> Arguments { get; set; }

            public IObject Type => throw new NotImplementedException();

            public IObject Parent { get; set; }

            public void Append(Fun f) {
                this.functions.Add(f.Arguments, f);
            }

            public Boolean IsParentOf(Value value) {
                if (value is IObject parent) {
                    while (true) {
                        if (parent.TryGetField("@prototype", out var v)) {
                            parent = v as IObject;
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
                        if (!j.Match(args[counter], s)) {
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
                if (this.Attributes.TryGetValue(name, out var result)) {
                    return result;
                }

                if (this.Parent != null) {
                    if (this.Parent.TryGetField(name, out result)) {
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
                    if (this.Parent.TryGetField(name, out result)) {
                        return true;
                    }
                }

                return false;
            }
        }

        public Expression Closure(List<String> visible, Scope thread) {
            visible.Add(this.NameFunction);
            return new FunctionDeclaration(this.NameFunction, this.arguments.Select(i => i.Closure(visible, thread) as IPattern).ToList(), this.Body.Closure(visible, thread));
        }
    }
}