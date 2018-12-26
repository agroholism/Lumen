using System;
using System.Collections.Generic;
using System.Linq;

using StandartLibrary.Expressions;

namespace StandartLibrary {
	internal class Enumerable : Module {
		private IEnumerable<Value> ToEnumerator(Value obj, Scope e) {
			Fun function = (Fun)obj.Type.GetAttribute("next", e);
			while (true) {
				Value current = null;
				try {
					Scope s = new Scope(e);
					s.This = obj;
					current = function.Run(s);
				}
				catch (Break) {
					break;
				}
				yield return current;
			}
		}

		internal Enumerable() {
			this.name = "Kernel.Enumerable";
		/*	this.require = new Dictionary<String, FunctionMetadata> {
				["next"] = new FunctionMetadata(new List<FunctionArgument>())
			};*/

			this.scope = new Scope(null);

			// *Kernel.Iterable => Kernel.Number
			this.scope.Set("@*", new LambdaFun((scope, args) => (Num)scope.This.ToIterator(1, scope).Count()));

			// Kernel.Enumerable, Kernel.Function, Kernel.Object? => Kernel.Object
			this.scope.Set("/", new LambdaFun((scope, args) => {
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

			// Kernel.Enumerable, Krenel.Function | Kernel.String | Kernel.Number => Kernel.String | Kernel.Enumerator
		/*	this.scope.Set("*", new LambdaFun((scope, args) => {
				IEnumerable<Value> value = scope.This.ToIterator(scope);
				Value other = scope["other"];

				switch (other) {
					case KString _:
						return new KString(String.Join(other.ToString(), value));
					case Fun fun:
						return new Enumerator(value.Select((it, i) => fun.Run(new Scope(scope), it, (Num)i)));
					default:
						return new Enumerator(Cycle(value, (Int32)other.ToDouble(scope)));
				}

			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("other")
					}
			});
			*/
			// *Kernel.Iterable => Kernel.Number.Fix
			this.scope.Set("get_size", this.scope["@*"]);

			// Kernel.Function (predicate) | Any => Kernel.Number.Fix
			this.scope.Set("count", new LambdaFun((e, args) => {
				if (args[0] is Fun fun) {
					Scope s = new Scope(e);
					s.This = this;
					return new Num(Converter.ToIterator(e.Get("this"), 1, e).Count(i => Converter.ToBoolean(fun.Run(s, i))));
				}
				return new Num(Converter.ToIterator(e.Get("this"), 1, e).Count(i => args[0] == i));
			}));

			// [Kernel.Function (predicate)] => Kernel.Boolean
			this.scope.Set("all?", new LambdaFun((e, args) => {
				if (args.Length > 0 && args[0] is Fun fun) {
					return new Bool(Converter.ToIterator(e.Get("this"), 1, e).All(x => Converter.ToBoolean(fun.Run(new Scope(e), x))));
				}
				return new Bool(Converter.ToIterator(e.Get("this"), 1, e).All(x => Converter.ToBoolean(x) == true));
			}));
			// [Kernel.Function (predicate)] => Kernel.Boolean
			this.scope.Set("any?", new LambdaFun((e, args) => {
				if (args.Length > 0 && args[0] is Fun fun) {
					return new Bool(Converter.ToIterator(e.Get("this"), 1, e).All(x => Converter.ToBoolean(fun.Run(new Scope(e), x))));
				}
				return new Bool(Converter.ToIterator(e.Get("this"), 1, e).Any(x => Converter.ToBoolean(x) == true));
			}));

			// Kernel.Function (predicate) => Kernel.Iterator
			this.scope.Set("select", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), 1, e);
				Fun f = (Fun)args[0];
				return new Enumerator(v.Where(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}));
			// Kernel.Function => Kernel.Iterator
			this.scope.Set("each", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Enumerator(v.Select(x => f.Run(new Scope(e), x)));
			}));
			// Kernel.Function => Kernel.Iterator
			/*this.scope.Set("each_slice", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(EachSlice(v, (int)Converter.ToDouble(args[0], e)));
			}));*/
			// Kernel.Function => Kernel.List
			this.scope.Set("collect", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Vec(v.Select(x => f.Run(new Scope(e), x)).ToList());
			}));
			// Kernel.Function => Kernel.List
			this.scope.Set("action", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				foreach (Value i in v) {
					f.Run(new Scope(e), i);
				}
				return Const.NULL;
			}));

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
			this.scope.Set("find", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				foreach (Value i in v) {
					if (Converter.ToBoolean(f.Run(new Scope(e), i))) {
						return i;
					}
				}
				if (e.IsExsists("ifnone")) {
					return e["none"];
				}
				else {
					return Const.NULL;
				}
			}));
			// Возвращает массив, содержащий все элементы из перечисления для которых переданный блок возвращает значение true
			this.scope.Set("find_all", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				List<Value> result = new List<Value>();
				Fun f = (Fun)args[0];
				foreach (Value i in v) {
					if (Converter.ToBoolean(f.Run(new Scope(e), i))) {
						result.Add(i);
					}
				}
				return new Vec(result);
			}));
			this.scope.Set("reduce", this.scope["/"]);
			this.scope.Set("first", new LambdaFun((e, args) => {
				IEnumerator<Value> v = Converter.ToIterator(e.Get("this"), e).GetEnumerator();
				if (v.MoveNext()) {
					return v.Current;
				}
				throw new Break(1);
			}));
			this.scope.Set("last", new LambdaFun((e, args) => {
				IEnumerator<Value> v = Converter.ToIterator(e.Get("this"), e).GetEnumerator();
				Value current = null;
				while (v.MoveNext()) {
					current = v.Current;
				}

				return current ?? throw new Break(1);
			}));
			this.scope.Set("min", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return v.Aggregate((x, y) => {
					Scope s = new Scope(e);
					s.This = x;
					return Converter.ToBoolean(x.Type.GetAttribute("<", e).Run(s, y)) ? x : y;
				});
			}));
			this.scope.Set("max", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return v.Aggregate((x, y) => {
					Scope s = new Scope(e);
					s.This = x;
					return Converter.ToBoolean(x.Type.GetAttribute(">", e).Run(s, y)) ? x : y;
				});
			}));

			/*this.scope.Set("take", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Take((int)Converter.ToDouble(args[0], e)));
			}));*/
			this.scope.Set("take_while", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Enumerator(v.TakeWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}));
			/*this.scope.Set("skip", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Skip((int)Converter.ToDouble(args[0], e)));
			}));*/
			this.scope.Set("skip_while", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				return new Enumerator(v.SkipWhile(x => Converter.ToBoolean(f.Run(new Scope(e), x))));
			}));

			this.scope.Set("sort", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				IOrderedEnumerable<Value> res = v.OrderBy(x => x);
				return new Enumerator(res);
			}));

			this.scope.Set("orderby", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				Fun f = (Fun)args[0];
				var res = v.OrderBy(x => f.Run(new Scope(e), x));
				return new Enumerator(res);
			}));

			this.scope.Set("unique", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Distinct());
			}));

			this.scope.Set("except", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Except(Converter.ToIterator(args[0], e)));
			}));

			this.scope.Set("intersect", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Intersect(Converter.ToIterator(args[0], e)));
			}));

			this.scope.Set("union", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Enumerator(v.Union(Converter.ToIterator(args[0], e)));
			}));

			this.scope.Set("zip", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				if (args.Length == 1)
					return new Vec(v.Zip<Value, Value, Value>(Converter.ToIterator(args[0], e), (x, y) => new Vec(new List<Value> { x, y })).ToList());
				return new Enumerator(v.Zip(Converter.ToIterator(args[0], e), (x, y) => ((Fun)args[1]).Run(new Scope(e), x, y)));
			}));

			this.scope.Set("join", new LambdaFun((e, args) => {
				if (args.Length > 0) {
					return new KString(System.String.Join(args[0].ToString(), Converter.ToIterator(e.Get("this"), e)));
				}
				else {
					return new KString(System.String.Join("", Converter.ToIterator(e.Get("this"), e)));
				}
			}));

			this.scope.Set("to_l", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Vec(v.ToList());
			}));

			/*this.scope.Set("step", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);

				return new Enumerator(Step(v, (int)Converter.ToDouble(args[0], e)));
			}));*/

			this.scope.Set("contains?", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Bool(v.Contains(args[0]));
			}));

			this.scope.Set("exists?", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToIterator(e.Get("this"), e);
				return new Bool(v.FirstOrDefault(i => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), i))) != null);
			}));

			Fill(StandartModule.String);
			Fill(StandartModule.Vector);
			Fill(StandartModule.Enumerator);
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
					yield return new Enumerator(Many(args.ToArray()));
					index = 0;
					args.Clear();
				}
				args.Add(i);
				index++;
			}

			if (args.Count > 0) {
				yield return new Enumerator(Many(args.ToArray()));
			}
		}

		private IEnumerable<Value> Many(params Value[] args) {
			foreach (Value i in args) {
				yield return i;
			}
		}

		public override Boolean TypeImplemented(KType s) {
			if (s.attributes.ContainsKey("to_i")) {
				Fill(s);
				return true;
			}
			else if (s.attributes.ContainsKey("next")) {
				s.SetAttribute("to_i", new LambdaFun((e, args) => {
					return new Enumerator(ToEnumerator(e.Get("this"), e));
				}));
				Fill(s);
				return true;
			}
			return false;
		}

		private void Fill(KType type) {
			foreach (KeyValuePair<String, Value> i in this.scope.variables) {
				if (i.Value is Fun f) {
					if (!type.attributes.ContainsKey(i.Key)) {
						type.SetAttribute(i.Key, f);
					}
				}
			}
			if (!type.includedModules.Contains(this))
				type.includedModules.Add(this);
		}
	}
}
