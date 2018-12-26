using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Std {
	internal class RVec : KType {
		internal RVec() {
			this.meta = new TypeMetadata {
				Name = "vec"
			};

			Set("()", new LambdaFun((e, args) => {
				if (e.ExistsInThisScope("to")) {
					BigFloat from = (e.ExistsInThisScope("from") ? (Num)e["from"] : (Num)0).value;
					BigFloat step = e.ExistsInThisScope("step") ? ((Num)e["step"]).value : null;

					BigFloat to = ((Num)e["to"]).value;

					if (step is null) {
						if (e.ExistsInThisScope("len")) {
							step = (to - from) / ((Num)e["len"]).value;
						}
						else {
							step = 1;
						}
					}

					List<Value> res = new List<Value>();

					for (BigFloat i = from; i <= to; i += step) {
						res.Add(new Num(i));
					}

					return new Vec(res);
				}

				return new Vec(args);
			}));

			#region operators
			SetAttribute(Op.SLASH, new LambdaFun((scope, args) => {
				IEnumerable<Value> value = scope.This.ToIterator(scope);
				Fun func = scope["func"].ToFunction(scope);
				Value seed = scope["seed"];

				if (seed is Null) {
					return value.Aggregate((x, y) => func.Run(new Scope(scope), x, y));
				}
				else {
					return value.Aggregate(seed, (x, y) => func.Run(new Scope(scope), x, y));
				}
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("func"),
						new FunctionArgument("seed", Const.NULL)
					}
			});

			// vec + obj 
			SetAttribute(Op.PLUS, new LambdaFun((scope, args) => {
				List<Value> exemplare = scope.This.Clone().ToList(scope);
				exemplare.Add(scope["other"]);
				return new Vec(exemplare);
			}) { Arguments = Const.OTHER });
			// vec - obj
			SetAttribute(Op.MINUS, new LambdaFun((scope, args) => {
				List<Value> exemplare = scope.This.Clone().ToList(scope);
				IEnumerable<Value> other = scope["other"].ToIterator(scope);

				return new Vec(exemplare.Except(other).ToList());
			}) { Arguments = Const.OTHER });
			// *vec
			SetAttribute(Op.USTAR, new LambdaFun((scope, args) => {
				return (Num)scope.This.ToList(scope).Count;
			}));
			// vec == obj
			SetAttribute(Op.EQL, new LambdaFun((scope, args) => {
				return new Bool(scope.This.Equals(scope["other"].ToList(scope)));
			}) { Arguments = Const.OTHER });
			// vec != obj
			SetAttribute(Op.NOT_EQL, new LambdaFun((scope, args) => {
				return new Bool(!scope.This.Equals(scope["other"].ToList(scope)));
			}) { Arguments = Const.OTHER });
			// vec & (seq | vec)
			SetAttribute(Op.BAND, new LambdaFun((scope, args) => {
				List<Value> exemplare = Converter.ToList(scope.Get("this"), scope);
				List<Value> other = scope["other"].ToList(scope);

				return new Vec(exemplare.Union(other).ToList());
			}) { Arguments = Const.OTHER });
			// vec | (seq | vec)
			SetAttribute(Op.BOR, new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				List<Value> other = Converter.ToList(args[0], e);

				return new Vec(list.Intersect(other).ToList());
			}));
			// vec += obj
			SetAttribute(Op.APLUS, new LambdaFun((e, args) => {
				List<Value> list = e.This.ToList(e);

				// or clone?
				list.Add(e["other"]);

				return e.This;
			}) { Arguments = Const.OTHER });

			SetAttribute(Op.STAR, new LambdaFun((scope, args) => {
				List<Value> value = scope.This.ToList(scope);
				Value other = scope["other"];

				switch (other) {
					case KString _:
						return new KString(String.Join(other.ToString(), value));
					case Fun fun:
						return new Vec(value.Select((it, i) => fun.Run(new Scope(scope), it, (Num)i)).ToList());
					case BigFloat bf:
						return new Vec(Cycle(value, (Int32)(Double)bf).ToList());
					default:
						throw new Lumen.Lang.Std.Exception($"operator '*' can not get a value of type {other.Type}");
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
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("other")
					}
			});

			SetAttribute(Op.GETI, new LambdaFun((e, args) => {
				List<Value> exemplare = Converter.ToList(e.Get("this"), e);

				if (args.Length == 0) {
					return e.This.Clone();
				}
				else if (args.Length == 1) {
					List<Value> result = new List<Value>();

					if (args[0] is Fun f) {
						return new Vec(exemplare.Where(x => f.Run(new Scope(e), x).ToBoolean()).ToList());
					}

					if (args[0] is Num) {
						Int32 index = Index((Int32)(Double)Converter.ToBigFloat(args[0], e), exemplare.Count);

						if (index < 0 || index >= exemplare.Count) {
							throw new Exception("выход за пределы списка при срезе вида [i]", stack: e);
						}

						return exemplare[index];
					}

					foreach (Value inde in args[0].ToIterator(e)) {
						if (inde is Num num) {
							Int32 ind = Index((Int32)(Double)num.value, exemplare.Count);
							result.Add(exemplare[ind]);
						}
					}
					return new Vec(result);
				}
				else if (args.Length == 2) {
					List<Value> result = new List<Value>();

					if (args[0] is Enumerator it) {
						foreach (Value index in it) {
							if (index is Num num) {
								result.Add(exemplare[Index((Int32)(Double)num.value, exemplare.Count)]);
							}
						}
						return new Vec(result);
					}
					else {
						/*Int32 index = Index((Int32)Converter.ToBigFloat(args[0], e), exemplare.Count);
						Int32 length = (Int32)Converter.ToDouble(args[1], e);

						if (length <= 0) {
							return new List();
						}

						if (index < 0 || index >= exemplare.Count) {
							throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
						}

						if (length + index > exemplare.Count) {
							throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
						}

						Int32 current = 0;
						while (current < length) {
							result.Add(exemplare[index]);
							index++;
							current++;
						}

						return new List(result);*/
						return null;
					}
				}
				else if (args.Length == 3) {
					/*List<Value> result = new List<Value>();

					Int32 index = Index((Int32)Converter.ToDouble(args[0], e), exemplare.Count);
					Int32 length = (Int32)Converter.ToDouble(args[1], e);
					Int32 step = (Int32)Converter.ToDouble(args[2], e);

					Boolean stepIsNegative = step < 0;

					if (length <= 0) {
						return new List();
					}

					if (index < 0 || index >= exemplare.Count) {
						throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
					}

					if (length + (stepIsNegative ? -index : index) > exemplare.Count) {
						throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
					}

					Int32 current = 0;
					while (current < length) {
						result.Add(exemplare[index]);
						index += step;
						current += Math.Abs(step);
					}

					return new List(result);*/
				}

				return Const.NULL; //
			}));
			SetAttribute(Op.SETI, new LambdaFun((e, args) => {
				List<Value> exemplare = Converter.ToList(e.Get("this"), e);

				if (args.Length == 2) {
					List<Value> result = new List<Value>();

					Int32 index = Index((Int32)(double)Converter.ToBigFloat(args[0], e), exemplare.Count);

					if (index < 0 || index >= exemplare.Count) {
						throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
					}

					exemplare[index] = args[1];

					return e.This;
				}
				else if (args.Length == 3) {
					/*List<Value> result = new List<Value>();

					Int32 index = Index((Int32)Converter.ToDouble(args[0], e), exemplare.Count);
					Int32 length = (Int32)Converter.ToDouble(args[1], e);
					Int32 step = (Int32)Converter.ToDouble(args[2], e);

					Boolean stepIsNegative = step < 0;

					if (length <= 0) {
						return new List();
					}

					if (index < 0 || index >= exemplare.Count) {
						throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
					}

					if (length + (stepIsNegative ? -index : index) > exemplare.Count) {
						throw new Exception("выход за пределы списка при срезе вида [i, j]", stack: e);
					}

					Int32 current = 0;
					while (current < length) {
						result.Add(exemplare[index]);
						index += step;
						current += Math.Abs(step);
					}

					return new List(result);*/
				}

				return Const.NULL; //
			}));
			#endregion

			SetAttribute("get_size", new LambdaFun((e, args) => new Num(Converter.ToList(e.Get("this"), e).Count), "Kernel.List.get_size"));

			SetAttribute("seq", new LambdaFun((e, args) => {
				List<Value> v = e.This.ToList(e);

				if (args.Length > 0 && args[0].ToBigFloat(e) > 1) {
					Int32 index = 0;
					return new Enumerator(v.Select(i => new Vec(new List<Value> { i, new Num(index++) })));
				}

				return new Enumerator(v);
			}));
			SetAttribute("str", new LambdaFun((e, args) => {
				return (KString)e.Get("this").ToString(e);
			}));
			SetAttribute("sort", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				List<Value> result = v.Select(i => i).ToList();

				if (e.IsExsists("by")) {
					Value obj1 = e.Get("by");
					//Checker.CheckType(obj1, Kernel.Function, e);

					Fun fun = (Fun)obj1;

					result.Sort((i, j) => {
						Scope scope = new Scope(e);
						scope["self"] = fun;
						scope["args"] = new Vec(new List<Value> { i, j });
						scope["kwargs"] = new Map();
						return (Int32)(double)Converter.ToBigFloat(fun.Run(scope, i, j), e);
					});
				}
				else if (args.Length == 0) {
					result.Sort();
				}
				else if (args.Length >= 1) {
					//Checker.CheckType(args[0], Kernel.Function, e);
					Fun fun = (Fun)args[0];
					result.Sort((i, j) => {
						Scope scope = new Scope(e);
						scope["self"] = fun;
						scope["args"] = new Vec(new List<Value> { i, j });
						scope["kwargs"] = new Map();
						return (Int32)(Double)Converter.ToBigFloat(fun.Run(scope, i, j), e);
					});
				}

				return new Vec(result);
			}));
			/*
						SetAttribute("sort!", new LambdaFun((e, args) => {
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
			SetAttribute("pop", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				Value lastElement = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				return lastElement;
			}));
			SetAttribute("peek", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				Value lastElement = list[list.Count - 1];
				return lastElement;
			}));
			SetAttribute("shift!", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				Value firstElement = list[0];
				list.RemoveAt(0);
				return firstElement;
			}));
			SetAttribute("unshift!", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				list.Insert(0, args[0]);
				return Const.NULL;
			}));
			SetAttribute("each!", new LambdaFun((e, args) => {
				List<Value> v = null;
				UserFun f = null;

				if (args[0] is UserFun) {
					f = (UserFun)args[0];
					v = Converter.ToList(e.Get("this"), e);
				}
				else {
					throw new Exception("ожидался тип fun", stack: e);
				}

				if (f.Arguments.Count == 1) {
					for (int i = 0; i < v.Count; i++) {
						v[i] = f.Run(new Scope(e), v[i]);
					}
				}
				else {
					for (int i = 0; i < v.Count; i++) {
						v[i] = f.Run(new Scope(e), v[i], new Num(i));
					}
				}

				return Const.NULL;
			}));
			SetAttribute("add!", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"));

				for (Int32 i = 0; i < args.Length; i++) {
					if (args[i] is Vec lst && lst.value.Equals(list)) {
						list.Add(args[i].Clone());
					}
					else {
						list.Add(args[i]);
					}
				}

				return (Num)list.Count;
			}));
			SetAttribute("index", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				Int32 result = list.IndexOf(args[0]);
				if (result == -1) {
					return Const.FALSE;
				}
				return (Num)result;
			}));
			SetAttribute("last_index", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				Int32 result = list.LastIndexOf(args[0]);
				if (result == -1) {
					return Const.FALSE;
				}
				return (Num)result;
			}));
			SetAttribute("clear!", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);
				list.Clear();
				return Const.NULL;
			}));
			SetAttribute("find_index", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);

				Value obj = args[0];
				//Checker.CheckType(obj, Kernel.Function, e);

				Int32 result = list.FindIndex(i => {
					Scope scope = new Scope(e);
					scope["args"] = new Vec(new List<Value> { i });
					scope["kwargs"] = new Map();
					return Converter.ToBoolean((obj as Fun).Run(scope, i));
				});

				if (result == -1) {
					return Const.FALSE;
				}

				return (Num)result;
			}));
			SetAttribute("find", new LambdaFun((e, args) => {
				List<Value> list = Converter.ToList(e.Get("this"), e);

				Value obj = args[0];
				//Checker.CheckType(obj, Kernel.Function, e);

				Value result = list.Find(i => {
					Scope scope = new Scope(e);
					scope["args"] = new Vec(new List<Value> { i });
					scope["kwargs"] = new Map();
					return Converter.ToBoolean((obj as Fun).Run(scope, i));
				});

				if (result == null) {
					return Const.FALSE;
				}

				return result;
			}));
			SetAttribute("find_last", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				return v.FindLast(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x)));
			}));
			SetAttribute("find_last_index", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				return new Num(v.FindLastIndex(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x))));
			}, "Kernel.List.find_last_index"));
			SetAttribute("insert!", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				v.Insert((int)(Double)Converter.ToBigFloat(args[0], e), args[1]);
				return Const.NULL;
			}));
			SetAttribute("last_index", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				return new Num(v.LastIndexOf(args[0]));
			}));
			SetAttribute("delete!", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				foreach (Value i in args) {
					v.Remove(i);
				}

				return Const.NULL;
			}));
			SetAttribute("reject!", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				v.RemoveAll(x => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), x)));
				return Const.NULL;
			}));
			/*SetAttribute("remove_at!", new LambdaFun((e, args) => {
				List<Value> v = Converter.ToList(e.Get("this"), e);
				foreach (Value i in args) {
					v.RemoveAt((int)Converter.ToBigFloat(i, e));
				}

				return Const.NULL;
			}, "Kernel.List.remove_at!"));*/
			SetAttribute("reverse", new LambdaFun((e, args) => {
				List<Value> arv = Converter.ToList(e.Get("this"), e);
				List<Value> result = new List<Value>(arv.Count);
				foreach (Value i in arv) {
					result.Add(i);
				}

				result.Reverse();
				return new Vec(result);
			}));
			SetAttribute("reverse!", new LambdaFun((e, args) => {
				List<Value> arv = Converter.ToList(e.Get("this"), e);
				arv.Reverse();
				return new Vec(arv);
			}, "Kernel.List.reverse!"));
			//SetAttribute("flatten", new Flatten());
			SetAttribute("partition", new LambdaFun((e, args) => {
				List<Value> obj = e.This.ToList(e);

				List<Value> first = new List<Value>();
				List<Value> second = new List<Value>();

				Fun predicate = args[0].ToFunction(e);
				foreach (Value i in obj) {
					if (predicate.Run(new Scope(e), i).ToBoolean()) {
						first.Add(i);
					}
					else {
						second.Add(i);
					}
				}

				return new Vec(new List<Value> { new Vec(first), new Vec(second) });
			}, "Kernel.List.partition"));
		}

		private static Int32 Index(Int32 index, Int32 count) {
			if (index < 0) {
				return count + index;
			}
			return index;
		}

		/*class Flatten : SystemFun {
			public override Value Run(Scope e, params Value[] Objects) {
				List<Value> v = KernelConverter.ToList(e.Get("this"), e);

				List<Value> res = new List<Value>();

				foreach (Value i in v) {
					if (i is KList) {
						Scope x = new Scope();
						x.Set("this", i);
						foreach (Value j in KernelConverter.ToList(Run(x, i), e))
							res.Add(j);
					}
					else {
						res.Add(i);
					}
				}

				return new KList(res);
			}
		}*/
	}
}
