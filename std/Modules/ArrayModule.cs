using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal class ArrayModule : Module {
        public ArrayModule() {
            this.name = "prelude.Array";

            #region operators

            // *vec<?> := num
            this.SetField(Op.USTAR, new LambdaFun((scope, args) => {
                return (Number)scope["a"].ToList(scope).Count;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("a")
                }
            });

            // vec<?> + ? := vec<?> { can be slow - makes copy } 
            this.SetField(Op.PLUS, new LambdaFun((scope, args) => {
                List<Value> value = scope["other"].Clone().ToList(scope);
                foreach (Value i in scope["this"].ToSequence(scope)) {
                    value.Add(i);
                }

                return new Array(value);
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> - (vec<?> | seq<?>) := vec<?>
            this.SetField(Op.MINUS, new LambdaFun((scope, args) => {
                List<Value> value = scope["other"].ToList(scope);
                IEnumerable<Value> other = scope["this"].ToSequence(scope);

                return new Array(value.Except(other).ToList());
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> * (String | fun<?> | num) := String | vec<?> | vec<?>
            this.SetField(Op.STAR, new LambdaFun((scope, args) => {
                List<Value> value = scope["other"].ToList(scope);
                Value other = scope["this"];

                switch (other) {
                    case Text _:
                        return new Text(String.Join(other.ToString(scope), value));
                    case Fun fun:
                        return new Array(value.Select((it, i) => fun.Run(new Scope(scope), it, (Number)i)).ToList());
                    case Number num:
                        return new Array(Cycle(value, (Int32)num.ToDouble(scope)).ToList());
                    default:
                        throw new LumenException($"operator '*' can not get a value of type {other.Type}");
                }

                IEnumerable<Value> Cycle(IEnumerable<Value> val, Int32 count) {
                    Int32 currentIndex = 0;
                    while (currentIndex < count) {
                        foreach (Value i in val) {
                            yield return i;
                        }
                        currentIndex++;
                    }
                }

            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> = ? := bool
            this.SetField(Op.EQUALS, new LambdaFun((scope, args) => {
                return new Bool(scope["other"].Equals(scope["this"]));
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> <> ? := bool
            this.SetField(Op.NOT_EQL, new LambdaFun((scope, args) => {
                return new Bool(!scope.This.Equals(scope["other"]));
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> & (seq<?> | vec<?>) := vec<?>
            this.SetField(Op.BAND, new LambdaFun((scope, args) => {
                List<Value> value = scope.This.ToList(scope);
                IEnumerable<Value> other = scope["other"].ToSequence(scope);

                return new Array(value.Union(other).ToList());
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?> | (seq<?> | vec<?>) := vec<?>
            this.SetField(Op.BOR, new LambdaFun((scope, args) => {
                List<Value> value = scope.This.ToList(scope);
                IEnumerable<Value> other = scope["other"].ToSequence(scope);

                return new Array(value.Intersect(other).ToList());
            }) {
                Arguments = Const.ThisOther
            });

            // vec<?>[...args]
            this.SetField(Op.GETI, new LambdaFun((scope, args) => {
                List<Value> exemplare = scope["a"].ToList(scope);
                Value i = scope["i"];

                if (i is Fun f) {
                    return new Array(exemplare.Where(x => f.Run(new Scope(scope), x).ToBoolean()).ToList());
                }

                if (i is Number) {
                    Int32 index = Index(i.ToInt(scope), exemplare.Count);

                    if (index < 0 || index >= exemplare.Count) {
                        throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
                    }

                    return exemplare[index];
                }

                List<Value> result = new List<Value>();

                foreach (Value subIndex in i.ToSequence(scope)) {
                    if (subIndex is Number) {
                        Int32 ind = Index(subIndex.ToInt(scope), exemplare.Count);

                        if (ind < 0 || ind >= exemplare.Count) {
                            throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
                        }

                        result.Add(exemplare[ind]);
                    }
                }

                return new Array(result);

            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("i"),
                    new NamePattern("a")
                }
            });

            this.SetField(Op.SETI, new LambdaFun((e, args) => {
                List<Value> exemplare = Converter.ToList(args[2], e);

                List<Value> result = new List<Value>();

                Int32 index = Index((Int32)args[0].ToDouble(e), exemplare.Count);

                if (index < 0 || index >= exemplare.Count) {
                    throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
                }
                
                exemplare[index] = args[1];

                return Const.UNIT;
            }));
            #endregion

            this.SetField("add", new LambdaFun((scope, args) => {
                List<Value> array = scope["a"].ToList(scope);

                array.Add(scope["e"]);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("e"),
                    new NamePattern("a")
                }
            });

            this.SetField("map", new LambdaFun((e, args) => {
                List<Value> v = e["a"].Clone().ToList(e);
                UserFun f = (UserFun)e["f"];

                if (f.Arguments.Count == 1) {
                    for (Int32 i = 0; i < v.Count; i++) {
                        v[i] = f.Run(new Scope(e), v[i]);
                    }
                } else {
                    for (Int32 i = 0; i < v.Count; i++) {
                        v[i] = f.Run(new Scope(e), v[i], new Number(i));
                    }
                }

                return new Array(v);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("a")
                }
            });

            // Converts vec to std.seq, can get
            this.SetField("Sequence", new LambdaFun((e, args) => {
                List<Value> v = e.This.ToList(e);

                if (args.Length > 0 && args[0].ToDouble(e) > 1) {
                    Int32 index = 0;
                    return new Enumerator(v.Select(i => new Array(new List<Value> { i, new Number(index++) })));
                }

                return new Enumerator(v);
            }));
            this.SetField("String", new LambdaFun((e, args) => {
                Array v = e.This.ToVec(e);
                return new Text(v.ToString(e));
            }));
            this.SetField("sort", new LambdaFun((e, args) => {
                List<Value> value = e.This.Clone().ToList(e);

                Value by = e["by"];
                Value descending = e["descending"];

                if (by != Const.UNIT) {
                    Fun fun = (Fun)by;

                    if (descending.ToBoolean()) {
                        value.Sort((i, j) => {
                            Scope scope = new Scope(e);
                            scope["self"] = fun;
                            return -(Int32)Converter.ToDouble(fun.Run(scope, i, j), e);
                        });
                    } else {
                        value.Sort((i, j) => {
                            Scope scope = new Scope(e);
                            scope["self"] = fun;
                            return (Int32)Converter.ToDouble(fun.Run(scope, i, j), e);
                        });
                    }
                } else if (descending.ToBoolean()) {
                    value.Sort((i, j) => j.CompareTo(i));
                } else {
                    value.Sort();
                }

                return new Array(value);
            }) {

            });
            /*
						Set("sort!", new LambdaFun((e, args) => {
							List<Value> list = Converter.ToList(e.Get("this"), e);

							if (e.IsExsists("by")) {
								Value obj1 = e.Get("by");
								//Checker.CheckType(obj1, Kernel.Function, e);

								Fun fun = (Fun)obj1;

								list.Sort((i, j) => {
									Scope scope = new Scope(e);
									scope["self"] = fun;
									scope["args"] = new List(new List<Value> { i, j });
									scope["kwargs"] = new Map();
									return (Int32)Converter.ToDouble(fun.Run(scope, i, j), e);
								});
							}
							else if (args.Length == 0) {
								list.Sort();
							}
							else if (args.Length >= 1) {
								//Checker.CheckType(args[0], Kernel.Function, e);
								Fun fun = (Fun)args[0];
								list.Sort((i, j) => {
									Scope scope = new Scope(e);
									scope["self"] = fun;
									scope["args"] = new List(new List<Value> { i, j });
									scope["kwargs"] = new Map();
									return (Int32)Converter.ToDouble(fun.Run(scope, i, j), e);
								});
							}

							return new List(list);
						}, "Kernel.List.sort!"));
						*/
            this.SetField("contains", new LambdaFun((e, args) => {
                List<Value> value = e.This.ToList(e);
                Value obj = args[0];
                return (Bool)value.Contains(obj);
            }));
            this.SetField("pop", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Value lastElement = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return lastElement;
            }));
            this.SetField("peek", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Value lastElement = list[list.Count - 1];
                return lastElement;
            }));
            this.SetField("shift", new LambdaFun((e, args) => {
                List<Value> list = e.This.ToList(e);
                Value firstElement = list[0];
                list.RemoveAt(0);
                return firstElement;
            }));
            this.SetField("unshift", new LambdaFun((e, args) => {
                List<Value> list = e.This.ToList(e);
                list.Insert(0, args[0]);
                return Const.UNIT;
            }));
            this.SetField("map_m", new LambdaFun((e, args) => {
                List<Value> v = e.This.ToList(e);
                UserFun f = (UserFun)args[0];

                if (f.Arguments.Count == 1) {
                    for (Int32 i = 0; i < v.Count; i++) {
                        v[i] = f.Run(new Scope(e), v[i]);
                    }
                } else {
                    for (Int32 i = 0; i < v.Count; i++) {
                        v[i] = f.Run(new Scope(e), v[i], new Number(i));
                    }
                }

                return Const.UNIT;
            }));

            this.SetField("index", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Int32 result = list.IndexOf(args[0]);
                if (result == -1) {
                    return Const.FALSE;
                }
                return (Number)result;
            }));
            this.SetField("last_index", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Int32 result = list.LastIndexOf(args[0]);
                if (result == -1) {
                    return Const.FALSE;
                }
                return (Number)result;
            }));
            this.SetField("clear", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                list.Clear();
                return Const.UNIT;
            }));
            this.SetField("find_index", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);

                Value obj = args[0];
                //Checker.CheckType(obj, Kernel.Function, e);

                Int32 result = list.FindIndex(i => {
                    Scope scope = new Scope(e);
                    scope["args"] = new Array(new List<Value> { i });
                    scope["kwargs"] = new Map();
                    return Converter.ToBoolean((obj as Fun).Run(scope, i));
                });

                if (result == -1) {
                    return Const.FALSE;
                }

                return (Number)result;
            }));
            this.SetField("find", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);

                Value obj = args[0];
                //Checker.CheckType(obj, Kernel.Function, e);

                Value result = list.Find(i => {
                    Scope scope = new Scope(e);
                    scope["args"] = new Array(new List<Value> { i });
                    scope["kwargs"] = new Map();
                    return Converter.ToBoolean((obj as Fun).Run(scope, i));
                });

                if (result == null) {
                    return Const.FALSE;
                }

                return result;
            }));
            this.SetField("find_last", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return v.FindLast(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x)));
            }));
            this.SetField("find_last_index", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return new Number(v.FindLastIndex(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x))));
            }));
            this.SetField("insert", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                v.Insert((Int32)Converter.ToDouble(args[0], e), args[1]);
                return Const.UNIT;
            }));
            this.SetField("last_index", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return new Number(v.LastIndexOf(args[0]));
            }));
            this.SetField("delete", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                foreach (Value i in args) {
                    v.Remove(i);
                }

                return Const.UNIT;
            }));
            this.SetField("reject", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                v.RemoveAll(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x)));
                return Const.UNIT;
            }));
            /*Set("remove_at!", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				foreach (Value i in args) {
					v.RemoveAt((int)Converter.ToBigFloat(i, e));
				}

				return Const.NULL;
			}, "Kernel.List.remove_at!"));*/
            this.SetField("reverse", new LambdaFun((e, args) => {
                List<Value> arv = Converter.ToList(e.Get("this"), e);
                List<Value> result = new List<Value>(arv.Count);
                foreach (Value i in arv) {
                    result.Add(i);
                }

                result.Reverse();
                return new Array(result);
            }));
            this.SetField("reverse", new LambdaFun((e, args) => {
                List<Value> arv = Converter.ToList(e.Get("this"), e);
                arv.Reverse();
                return new Array(arv);
            }));
            //Set("flatten", new Flatten());
            this.SetField("partition", new LambdaFun((e, args) => {
                List<Value> obj = e.This.ToList(e);

                List<Value> first = new List<Value>();
                List<Value> second = new List<Value>();

                Fun predicate = args[0].ToFunction(e);
                foreach (Value i in obj) {
                    if (predicate.Run(new Scope(e), i).ToBoolean()) {
                        first.Add(i);
                    } else {
                        second.Add(i);
                    }
                }

                return new Array(new List<Value> { new Array(first), new Array(second) });
            }));

            /*    this.Set("String", new LambdaFun((e, args) => {
                    Array v = e.This.ToVec(e);
                    return (String)v.ToString(e);
                }));*/

            this.Derive(Prelude.Sequence);
        }

        private static Int32 Index(Int32 index, Int32 count) {
            if (index < 0) {
                return count + index;
            }
            return index;
        }
    }
}
