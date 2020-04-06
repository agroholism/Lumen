using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class ArrayModule : Module {
        public ArrayModule() {
            this.Name = "Array";

			#region operators
			this.SetMember(Op.SETI, new LambdaFun((e, args) => {
				List<Value> exemplare = e.Get("array").ToList(e);

				List<Value> result = new List<Value>();

				Int32 index = Index((Int32)e.Get("index").ToDouble(e), exemplare.Count);

				if (index < 0 || index >= exemplare.Count) {
					throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
				}

				exemplare[index] = e.Get("value");

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("array"),
					new NamePattern("index"),
					new NamePattern("value")
				}
			});
			#endregion

			this.SetMember("getRefIndex", new LambdaFun((e, args) => {
				Int32 index = e.Get("index").ToInt(e);
				return new ArrayRef(e.Get("this") as Array, index);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this"),
					new NamePattern("index")
				}
			});

			this.SetMember("add", new LambdaFun((scope, args) => {
                List<Value> array = scope["a"].ToList(scope);

                array.Add(scope["e"]);

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("e"),
                    new NamePattern("a")
                }
            });

            this.SetMember("toText", new LambdaFun((e, args) => {
                Array v = e["this"].ToArray(e);
                return new Text(v.ToString());
            }));
            this.SetMember("sort", new LambdaFun((e, args) => {
                List<Value> value = e["this"].Clone().ToList(e);

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
            this.SetMember("contains", new LambdaFun((e, args) => {
				List<Value> value = e["this"].ToList(e);
				return (Bool)value.Contains(e.Get("elem"));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this"),
					new NamePattern("elem")
				}
			});
            this.SetMember("pop", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Value lastElement = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return lastElement;
            }));
            this.SetMember("peek", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Value lastElement = list[list.Count - 1];
                return lastElement;
            }));
            this.SetMember("shift", new LambdaFun((e, args) => {
                List<Value> list = e["this"].ToList(e);
                Value firstElement = list[0];
                list.RemoveAt(0);
                return firstElement;
            }));
            this.SetMember("unshift", new LambdaFun((e, args) => {
                List<Value> list = e["this"].ToList(e);
                list.Insert(0, args[0]);
                return Const.UNIT;
            }));
            this.SetMember("map_m", new LambdaFun((e, args) => {
                List<Value> v = e["this"].ToList(e);
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

            this.SetMember("index", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Int32 result = list.IndexOf(args[0]);
                if (result == -1) {
                    return Const.FALSE;
                }
                return (Number)result;
            }));
            this.SetMember("last_index", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                Int32 result = list.LastIndexOf(args[0]);
                if (result == -1) {
                    return Const.FALSE;
                }
                return (Number)result;
            }));
            this.SetMember("clear", new LambdaFun((e, args) => {
                List<Value> list = Converter.ToList(e.Get("this"), e);
                list.Clear();
                return Const.UNIT;
            }));
            this.SetMember("find_index", new LambdaFun((e, args) => {
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
            this.SetMember("find", new LambdaFun((e, args) => {
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
            this.SetMember("find_last", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return v.FindLast(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x)));
            }));
            this.SetMember("find_last_index", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return new Number(v.FindLastIndex(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x))));
            }));
            this.SetMember("insert", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                v.Insert((Int32)Converter.ToDouble(args[0], e), args[1]);
                return Const.UNIT;
            }));
            this.SetMember("last_index", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                return new Number(v.LastIndexOf(args[0]));
            }));
            this.SetMember("delete", new LambdaFun((e, args) => {
                List<Value> v = Converter.ToList(e.Get("this"), e);
                foreach (Value i in args) {
                    v.Remove(i);
                }

                return Const.UNIT;
            }));
            this.SetMember("reject", new LambdaFun((e, args) => {
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
            this.SetMember("reverse", new LambdaFun((e, args) => {
                List<Value> arv = Converter.ToList(e.Get("this"), e);
                List<Value> result = new List<Value>(arv.Count);
                foreach (Value i in arv) {
                    result.Add(i);
                }

                result.Reverse();
                return new Array(result);
            }));
            this.SetMember("reverse", new LambdaFun((e, args) => {
                List<Value> arv = Converter.ToList(e.Get("this"), e);
                arv.Reverse();
                return new Array(arv);
            }));
            //Set("flatten", new Flatten());
            this.SetMember("partition", new LambdaFun((e, args) => {
                List<Value> obj = e["this"].ToList(e);

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

			this.SetMember("toStream", new LambdaFun((e, args) => {
				return new Stream(e["array"].ToList(e));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("array")
				}
			});

			this.IncludeMixin(Prelude.Collection);
        }

        private static Int32 Index(Int32 index, Int32 count) {
            if (index < 0) {
                return count + index;
            }
            return index;
        }
    }
}
