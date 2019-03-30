using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal class SequenceModule : TypeClass {
        private IEnumerable<Value> ToEnumerator(Value obj, Scope e) {
            Fun function = (Fun)obj.Type.GetField("next", e);
            while (true) {
                Value current = null;
                try {
                    Scope s = new Scope(e) {
                        This = obj
                    };
                    current = function.Run(s);
                } catch (Break) {
                    break;
                }
                yield return current;
            }
        }

        internal SequenceModule() {
            this.name = "prelude.Sequence";

            this.Requirements = new List<Fun> {

            };

            this.SetField(Op.USTAR, new LambdaFun((scope, args) => (Number)scope["s"].ToSequence(scope).Count()) {
                Arguments = new List<IPattern> { new NamePattern("s") }
            });

            this.SetField(Op.SLASH, new LambdaFun((scope, args) => {
                IEnumerable<Value> value = scope["s"].ToSequence(scope);
                Fun func = scope["f"].ToFunction(scope);

                return value.Aggregate((x, y) =>
                    func.Run(new Scope(scope), x, y));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            this.SetField(Op.STAR, new LambdaFun((scope, args) => {
                IEnumerable<Value> value = scope["s"].ToSequence(scope);
                Value other = scope["x"];

                switch (other) {
                    case Text text:
                        return new Text(String.Join(other.ToString(), text));
                    case Fun fun:
                        return new Enumerator(value.Select((it, i) => fun.Run(new Scope(scope), it, (Number)i)));
                    default:
                        return new Enumerator(this.Cycle(value, (Int32)other.ToDouble(scope)));
                }
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("x")
                }
            });

            this.SetField(Op.MINUS, new LambdaFun((scope, args) => {
                IEnumerable<Value> value = scope["s1"].ToSequence(scope);
                IEnumerable<Value> s2 = scope["s2"].ToSequence(scope);

                return new Enumerator(value.Except(s2));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s1"),
                    new NamePattern("s2")
                }
            });

            this.SetField(Op.PLUS, new LambdaFun((scope, args) => {
                IEnumerable<Value> s1 = scope["s1"].ToSequence(scope);
                IEnumerable<Value> s2 = scope["s2"].ToSequence(scope);

                return new Enumerator(s1.Concat(s2));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s1"),
                    new NamePattern("s2")
                }
            });

            this.SetField("reduce", new LambdaFun((scope, args) => {
                IEnumerable<Value> value = scope["s"].ToSequence(scope);
                Fun func = scope["f"].ToFunction(scope);

                return value.Aggregate((x, y) =>
                    func.Run(new Scope(scope), x, y));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            this.SetField("first", new LambdaFun((scope, args) => {
                IEnumerable<Value> s1 = scope["s"].ToSequence(scope);

                if (s1.Count() == 0) {
                    return Prelude.None;
                }

                return Helper.CreateSome(s1.First());
            }) {
                Arguments = new List<IPattern> { new NamePattern("s") }
            });

            this.SetField("last", new LambdaFun((scope, args) => {
                IEnumerable<Value> s1 = scope["s"].ToSequence(scope);

                if (s1.Count() == 0) {
                    return Prelude.None;
                }

                return Helper.CreateSome(s1.Last());
            }) {
                Arguments = new List<IPattern> { new NamePattern("s") }
            });

            this.SetField("count", new LambdaFun((e, args) => {
                Value x = e["x"];
                IEnumerable<Value> s = e["s"].ToSequence(e);

                if (x is Fun fun) {
                    return new Number(s.Count(i => fun.Run(new Scope(e), i).ToBoolean()));
                }

                return new Number(s.Count(i => x == i));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("x")
                }
            });

            // [Kernel.Function (predicate)] => Kernel.Boolean
            this.SetField("isAll", new LambdaFun((e, args) => {
                Fun f = e["f"] as Fun;
                IEnumerable<Value> s = e["s"].ToSequence(e);

                return new Bool(s.All(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            // [Kernel.Function (predicate)] => Kernel.Boolean
            this.SetField("isAny", new LambdaFun((e, args) => {
                Fun f = e["f"] as Fun;
                IEnumerable<Value> s = e["s"].ToSequence(e);

                return new Bool(s.Any(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            // Kernel.Function (predicate) => Kernel.Iterator
            this.SetField("filter", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), 1, e);
                Fun f = (Fun)args[0];
                return new Enumerator(v.Where(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
            }));
            // Kernel.Function => Kernel.Iterator
            this.SetField("map", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                Fun f = (Fun)args[0];
                return new Enumerator(v.Select(x => f.Run(new Scope(e), x)));
            }));
            // Kernel.Function => Kernel.List
            this.SetField("collect", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                Fun f = (Fun)args[0];
                return new Array(v.Select(x => f.Run(new Scope(e), x)).ToList());
            }));
            // Kernel.Function => Kernel.List
            this.SetField("action", new LambdaFun((e, args) => {
                IEnumerable<Value> v = e["s"].ToSequence(e);
                Fun f = (Fun)e["act"];
                foreach (Value i in v) {
                    f.Run(new Scope(e), i);
                }
                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            // Kernel.Function => Kernel.List
            /*this.scope.Set("cycle", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				if (args.Length == 0) {
					return new Enumerator(CycleInf(v));
				}
				return new Enumerator(Cycle(v, (Int32)Converter.ToDouble(args[0], e)));
			}));*/
            // Помещает каждую запись массива в блок. 
            // Возвращает первое вхождение для которого блок не false.
            // Если ни один объект не подошел вызывается переменная ifnone, если она не задана возвращается null.
            this.SetField("find", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e["s"], e);
                Fun f = (Fun)e["f"];

                foreach (Value i in v) {
                    if (Converter.ToBoolean(f.Run(new Scope(e), i))) {
                        return i;
                    }
                }

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            // Возвращает массив, содержащий все элементы из перечисления для которых переданный блок возвращает значение true
            this.SetField("findAll", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e["s"], e);
                Fun f = (Fun)e["f"];

                List<Value> result = new List<Value>();
                foreach (Value i in v) {
                    if (Converter.ToBoolean(f.Run(new Scope(e), i))) {
                        result.Add(i);
                    }
                }
                return new Array(result);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("f")
                }
            });

            //this.SetField("reduce", this.scope["/"]);
            this.SetField("min", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("s"), e);
                return v.Aggregate((x, y) => {
                    Scope s = new Scope(e);
                    return x.CompareTo(y) < 0 ? x : y;
                });
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s")
                }
            });
            this.SetField("max", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("s"), e);
                return v.Aggregate((x, y) => {
                    Scope s = new Scope(e);
                    return x.CompareTo(y) > 0 ? x : y;
                });
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s")
                }
            });

            /*this.scope.Set("take", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Take((int)Converter.ToDouble(args[0], e)));
			}));*/
            this.SetField("take_while", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                Fun f = (Fun)args[0];
                return new Enumerator(v.TakeWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });
            /*this.scope.Set("skip", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Skip((int)Converter.ToDouble(args[0], e)));
			}));*/
            this.SetField("skip_while", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                Fun f = (Fun)args[0];
                return new Enumerator(v.SkipWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("sort", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("s"), e);
                IOrderedEnumerable<Value> res = v.OrderBy(x => x);
                return new Enumerator(res);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s")
                }
            });

            this.SetField("orderby", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                Fun f = (Fun)args[0];
                IOrderedEnumerable<Value> res = v.OrderBy(x => f.Run(new Scope(e), x));
                return new Enumerator(res);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("unique", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Enumerator(v.Distinct());
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("except", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Enumerator(v.Except(Converter.ToSequence(args[0], e)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("intersect", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Enumerator(v.Intersect(Converter.ToSequence(args[0], e)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("union", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Enumerator(v.Union(Converter.ToSequence(args[0], e)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("zip", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                if (args.Length == 1) {
                    return new Array(v.Zip<Value, Value, Value>(Converter.ToSequence(args[0], e), (x, y) => new Array(new List<Value> { x, y })).ToList());
                }

                return new Enumerator(v.Zip(Converter.ToSequence(args[0], e), (x, y) => ((Fun)args[1]).Run(new Scope(e), x, y)));
            }));

            this.SetField("join", new LambdaFun((e, args) => {
                if (args.Length > 0) {
                    return new Text(System.String.Join(args[0].ToString(), Converter.ToSequence(e.Get("this"), e)));
                } else {
                    return new Text(System.String.Join("", Converter.ToSequence(e.Get("this"), e)));
                }
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("Array", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Array(v.ToList());
            }) {
                Arguments = Const.This
            });

            this.SetField("Text", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Text("[seq]");
            }) {
                Arguments = Const.This
            });

            /*this.scope.Set("step", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);

				return new Enumerator(Step(v, (int)Converter.ToDouble(args[0], e)));
			}));*/

            this.SetField("contains", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e["s"], e);
                return new Bool(v.Contains(e["e"]));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("e")
                }
            });

            this.SetField("exists?", new LambdaFun((e, args) => {
                IEnumerable<Value> v = Converter.ToSequence(e.Get("this"), e);
                return new Bool(v.FirstOrDefault(i => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), i))) != null);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s"),
                    new NamePattern("act")
                }
            });

            this.SetField("List", new LambdaFun((scope, args) => {
                IEnumerable<Value> s = scope["s"].ToSequence(scope);
                return new List(LinkedList.Create(s));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("s")
                }
            });
        }

        private IEnumerable<Value> Step(IEnumerable<Value> val, Int32 by) {
            Int32 indexer = by;
            foreach (Value i in val) {
                if (by == indexer) {
                    yield return i;
                    indexer = 1;
                } else {
                    indexer++;
                }
            }
        }

        private IEnumerable<Value> CycleInf(IEnumerable<Value> val) {
            while (true) {
                foreach (Value i in val) {
                    yield return i;
                }
            }
        }

        private IEnumerable<Value> Cycle(IEnumerable<Value> val, Int32 count) {
            Int32 currentIndex = 0;
            while (currentIndex < count) {
                foreach (Value i in val) {
                    yield return i;
                }
                currentIndex++;
            }
        }

        private IEnumerable<Value> EachSlice(IEnumerable<Value> val, Int32 count) {
            Int32 index = 0;
            List<Value> args = new List<Value>(count);
            foreach (Value i in val) {
                if (index == count) {
                    yield return new Enumerator(this.Many(args.ToArray()));
                    index = 0;
                    args.Clear();
                }
                args.Add(i);
                index++;
            }

            if (args.Count > 0) {
                yield return new Enumerator(this.Many(args.ToArray()));
            }
        }

        private IEnumerable<Value> Many(params Value[] args) {
            foreach (Value i in args) {
                yield return i;
            }
        }

        /* public override Boolean TypeImplemented(Module s) {
             if (s.attributes.ContainsKey("to_i")) {
                 this.Fill(s);
                 return true;
             } else if (s.attributes.ContainsKey("next")) {
                 s.Set("to_i", new LambdaFun((e, args) => {
                     return new Enumerator(this.ToEnumerator(e.Get("this"), e));
                 }));
                 this.Fill(s);
                 return true;
             }
             return false;
         }*/

        /*private void Fill(Module type) {
            foreach (KeyValuePair<String, Value> i in this.scope.variables) {
                if (i.Value is Fun f) {
                    if (!type.attributes.ContainsKey(i.Key)) {
                        type.Set(i.Key, f);
                    }
                }
            }
            if (!type.includedModules.Contains(this)) {
                type.includedModules.Add(this);
            }
        }*/
    }
}
