using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Collection : Module {
		static Int32 Index(Int32 index, Int32 count) {
			if (index < 0) {
				return count + index;
			}
			return index;
		}

		internal Collection() {
			this.Name = "Collection";

			this.IncludeMixin(Prelude.Functor);
			this.IncludeMixin(Prelude.Applicative);

			this.SetMember("toStream", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Collection.toStream", scope["x"].Type.ToString());
				return Const.UNIT;
			}) {
				Name = "toStream",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("fromStream", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Collection.fromStream", scope["x"].Type.ToString());
				return Const.UNIT;
			}) {
				Name = "fromStream",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember(Op.GETI, new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				Value index = scope["indices"];

				if (index is Fun fun) {
					return new Stream(values.Where(x => fun.Run(new Scope(scope), x).ToBoolean()));
				}

				Int32 count = values.Count();

				if (index is Number) {
					Int32 intIndex = Index(index.ToInt(scope), count);

					if (intIndex < 0 || intIndex >= count) {
						throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
					}

					return values.ElementAt(intIndex);
				}

				return new Stream(index.ToStream(scope).Select(i => {
					if (i is Number) {
						Int32 index = Index(i.ToInt(scope), count);

						if (index < 0 || index >= count) {
							throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
						}

						return values.ElementAt(index);
					}
					else {
						throw new LumenException(Exceptions.TYPE_ERROR.F(Prelude.Number, i.Type));
					}
				}));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("indices"),
					new NamePattern("self")
				}
			});

			this.SetMember(Op.USTAR, new LambdaFun((scope, args) => (Number)scope["values"].ToStream(scope).Count()) {
				Arguments = new List<IPattern> { new NamePattern("values") }
			});

			this.SetMember(Op.SLASH, new LambdaFun((scope, args) => {
				IEnumerable<Value> value = scope["values"].ToStream(scope);
				Value foldf = scope["foldf"];
				if(foldf is Number num) {
					return new Stream(this.Step(value, foldf.ToInt(scope)));
				}

				Fun func = scope["foldf"].ToFunction(scope);

				return value.Aggregate((x, y) => func.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("foldf")
				}
			});

			this.SetMember(Op.STAR, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<Value> value = scope["values"].ToStream(scope);
				Value other = scope["x"];

				if (other == Const.UNIT) {
					return (Number)value.Count();
				}

				switch (other) {
					case Text text:
						return new Text(String.Join(text.value, value));
					case Fun fun:
						return new Stream(value.Select(it => fun.Run(new Scope(scope), it)));
					default:
						return new Stream(this.Cycle(value, (Int32)other.ToDouble(scope)));
				}
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("x")
				}
			});

			this.SetMember(Op.MINUS, new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["values"].ToStream(scope);
				IEnumerable<Value> valuesx = scope["values'"].ToStream(scope);

				return new Stream(values.Except(valuesx));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});

			this.SetMember(Op.PLUS, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<Value> values = scope["values"].ToStream(scope);
				IEnumerable<Value> valuesx = scope["values'"].ToStream(scope);

				return new Stream(values.Concat(valuesx));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});

			// Collection 'T => xs: 'T -> 'U
			this.SetMember("average", new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["values"].ToStream(scope);

				Value sum = values.Aggregate((x, y) => {
					return (x.Type.GetMember(Op.PLUS, scope) as Fun).Run(scope, x, y);
				});

				return (sum.Type.GetMember(Op.SLASH, scope) as Fun).Run(scope, sum, new Number(values.Count()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values")
				}
			});

			this.SetMember("fold", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"] as Fun;
				IEnumerable<Value> values = scope["values"].ToStream(scope);

				return values.Aggregate((x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("folder"),
					new NamePattern("values")
				}
			});
			this.SetMember("foldInit", new LambdaFun((scope, args) => {
				Fun folder;
				IEnumerable<Value> values;

				Value init = scope["init"];
				folder = scope["folder"] as Fun;
				values = scope["values"].ToStream(scope);

				return values.Aggregate(init, (x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("folder"),
					new NamePattern("values")
				}
			});
			this.SetMember("foldr", new LambdaFun((scope, args) => {
				Fun folder;
				IEnumerable<Value> values;

				folder = scope["folder"] as Fun;
				values = scope["values"].ToStream(scope);

				return values.Reverse().Aggregate((x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("folder"),
					new NamePattern("values")
				}
			});
			this.SetMember("foldrInit", new LambdaFun((scope, args) => {
				Fun folder;
				IEnumerable<Value> values;

				Value init = scope["init"];
				folder = scope["folder"] as Fun;
				values = scope["values"].ToStream(scope);

				return values.Reverse().Aggregate(init, (x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("folder"),
					new NamePattern("values")
				}
			});

			this.SetMember("first", new LambdaFun((scope, args) => {
				IEnumerable<Value> s1 = scope["s"].ToStream(scope);

				Value res = s1.FirstOrDefault();

				if (res == null) {
					return Prelude.None;
				}

				return Helper.CreateSome(res);
			}) {
				Arguments = new List<IPattern> { new NamePattern("s") }
			});

			this.SetMember("last", new LambdaFun((scope, args) => {
				IEnumerable<Value> s1 = scope["s"].ToStream(scope);

				Value res = s1.LastOrDefault();

				if (res == null) {
					return Prelude.None;
				}

				return Helper.CreateSome(res);
			}) {
				Arguments = new List<IPattern> { new NamePattern("s") }
			});

			this.SetMember("count", new LambdaFun((scope, args) => {
				IEnumerable<Value> stream = scope["values"].ToStream(scope);

				return new Number(stream.Count());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values")
				}
			});

			this.SetMember("countOf", new LambdaFun((scope, args) => {
				Value elem = scope["elem"];
				IEnumerable<Value> stream = scope["values"].ToStream(scope);

				return new Number(stream.Count(i => elem.Equals(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elem"),
					new NamePattern("values")
				}
			});

			this.SetMember("countWhen", new LambdaFun((scope, args) => {
				Value pred = scope["pred"];
				IEnumerable<Value> stream = scope["values"].ToStream(scope);

				Fun fun = pred.ToFunction(scope);
				return new Number(stream.Count(i => fun.Run(new Scope(scope), i).ToBoolean()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("pred"),
					new NamePattern("values")
				}
			});

			// [Kernel.Function (predicate)] => Kernel.Boolean
			this.SetMember("isAll", new LambdaFun((e, args) => {
				Fun f = e["f"] as Fun;
				IEnumerable<Value> s = e["s"].ToStream(e);

				return new Bool(s.All(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("f")
				}
			});

			// [Kernel.Function (predicate)] => Kernel.Boolean
			this.SetMember("isAny", new LambdaFun((e, args) => {
				Fun f = e["f"] as Fun;
				IEnumerable<Value> s = e["s"].ToStream(e);

				return new Bool(s.Any(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("f")
				}
			});

			// Kernel.Function (predicate) => Kernel.Iterator
			this.SetMember("filter", new LambdaFun((e, args) => {
				IType typeParameter = e["values"].Type;
				Fun mapper;
				IEnumerable<Value> values;

				if (e["fn"] is Fun) {
					mapper = e["fn"] as Fun;
					values = e["fc"].ToStream(e);
				}
				else {
					values = e["fn"].ToStream(e);
					mapper = e["fc"] as Fun;
				}

				return Helper.FromStream(typeParameter, values.Where(i => mapper.Run(new Scope(e), i).ToBoolean()), e);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			static IEnumerable<Value> Flatten(IEnumerable<IEnumerable<Value>> list, Scope s) {
				foreach (IEnumerable<Value> item in list) {
					foreach (Value item2 in item) {
						yield return item2;
					}
				}
			}

			Value Map(Scope scope, Value fc, Value fn) {
				IType typeParameter = fc.Type;
				Fun mapper = fn.ToFunction(scope);
				IEnumerable<Value> values = fc.ToStream(scope);

				return Helper.FromStream(typeParameter, 
					values.Select(i => mapper.Run(new Scope(scope), i)), scope);
			}

			// Collection -> Function -> Stream
			this.SetMember("map", new LambdaFun((scope, args) => {
				Fun mapper = scope["fn"].ToFunction(scope);
				IEnumerable<Value> values = scope["fc"].ToStream(scope);

				return new Stream(values.Select(i => mapper.Run(new Scope(scope), i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc")
				}
			});
			
			// Functor realization
			// Collection 'T -> Function -> Collection 'T
			this.SetMember("fmap", new LambdaFun((scope, args) => {
				return Map(scope, scope["fc"], scope["fn"]);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			LambdaFun mapi = new LambdaFun((e, args) => {
				IType typeParameter = e["values"].Type;
				Fun mapper;
				IEnumerable<Value> values;

				if (e["fn"] is Fun) {
					mapper = e["fn"] as Fun;
					values = e["fc"].ToStream(e);
				}
				else {
					values = e["fn"].ToStream(e);
					mapper = e["fc"] as Fun;
				}

				return Helper.FromStream(typeParameter, values.Select((i, index) => mapper.Run(new Scope(e), new Number(index), i)), e);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			};

			this.SetMember("mapi", mapi);

			// Applicative
			this.SetMember("liftA", new LambdaFun((scope, args) => {
				IEnumerable<Value> obj = scope["f"].ToStream(scope);

				return Helper.FromStream(
					scope["f"].Type,
					Flatten(obj.Select(i => 
						scope["m"].ToStream(scope).Select(j => 
							i.ToFunction(scope).Run(new Scope(), j))), scope), 
					scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});


			// Kernel.Function => Kernel.List
			this.SetMember("collect", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Array(v.Select(x => f.Run(new Scope(e), x)).ToList());
			}));

			this.SetMember("iter", new LambdaFun((e, args) => {
				Fun action;
				IEnumerable<Value> values;

				if (e["action"] is Fun) {
					action = e["action"] as Fun;
					values = e["list"].ToStream(e);
				}
				else {
					values = e["action"].ToStream(e);
					action = e["list"] as Fun;
				}

				foreach (Value i in values) {
					action.Run(new Scope(e), i);
				}
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("list"),
					new NamePattern("action")
				}
			});

			this.SetMember("iteri", new LambdaFun((e, args) => {
				Fun action;
				IEnumerable<Value> values;

				if (e["action"] is Fun) {
					action = e["action"] as Fun;
					values = e["list"].ToStream(e);
				}
				else {
					values = e["action"].ToStream(e);
					action = e["list"] as Fun;
				}

				Int32 index = 0;
				foreach (Value i in values) {
					action.Run(new Scope(e), new Number(index), i);
					index++;
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("list"),
					new NamePattern("action")
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
			this.SetMember("find", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e["s"], e);
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
			this.SetMember("findAll", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e["s"], e);
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
			this.SetMember("min", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("s"), e);
				return v.Aggregate((x, y) => {
					Scope s = new Scope(e);
					return x.CompareTo(y) < 0 ? x : y;
				});
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s")
				}
			});
			this.SetMember("max", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("s"), e);
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
			this.SetMember("take_while", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Stream(v.TakeWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
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
			this.SetMember("skip_while", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Stream(v.SkipWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("sort", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("s"), e);
				IOrderedEnumerable<Value> res = v.OrderBy(x => x);
				return new Stream(res);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s")
				}
			});

			this.SetMember("orderby", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				Fun f = (Fun)args[0];
				IOrderedEnumerable<Value> res = v.OrderBy(x => f.Run(new Scope(e), x));
				return new Stream(res);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("unique", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Stream(v.Distinct());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("except", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Stream(v.Except(Converter.ToStream(args[0], e)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("intersect", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Stream(v.Intersect(Converter.ToStream(args[0], e)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("union", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Stream(v.Union(Converter.ToStream(args[0], e)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("zip", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				if (args.Length == 1) {
					return new Array(v.Zip<Value, Value, Value>(Converter.ToStream(args[0], e), (x, y) => new Array(new List<Value> { x, y })).ToList());
				}

				return new Stream(v.Zip(Converter.ToStream(args[0], e), (x, y) => ((Fun)args[1]).Run(new Scope(e), x, y)));
			}));

			this.SetMember("join", new LambdaFun((e, args) => {
				IEnumerable<Value> values = e["values"].ToStream(e);
				String delim = e["delim"].ToString();

				return new Text(String.Join(delim, values));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("delim")
				}
			});

			this.SetMember("Array", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Array(v.ToList());
			}) {
				Arguments = Const.Self
			});

			this.SetMember("Text", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Text("[seq]");
			}) {
				Arguments = Const.Self
			});

			this.SetMember("step", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);

				return new Stream(this.Step(v, e["count"].ToInt(e)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this"),
					new NamePattern("count")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);
				return new Bool(self.Contains(scope["elem"]));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elem"),
					new NamePattern("self")
				}
			});

			this.SetMember("exists?", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Bool(v.FirstOrDefault(i => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), i))) != null);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("toList", new LambdaFun((scope, args) => {
				IEnumerable<Value> s = scope["x"].ToStream(scope);
				return new List(LinkedList.Create(s));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("fromList", new LambdaFun((scope, args) => {
				IEnumerable<Value> s = scope["x"].ToLinkedList(scope);
				return new Stream(s);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});
		}

		private IEnumerable<Value> Step(IEnumerable<Value> val, Int32 by) {
			Int32 indexer = by;
			foreach (Value i in val) {
				if (by == indexer) {
					yield return i;
					indexer = 1;
				}
				else {
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
					yield return new Stream(this.Many(args.ToArray()));
					index = 0;
					args.Clear();
				}
				args.Add(i);
				index++;
			}

			if (args.Count > 0) {
				yield return new Stream(this.Many(args.ToArray()));
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
