using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Collection : SystemClass {
		internal Collection() {
			this.Name = "Collection";

			this.SetMember("toSeq", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Collection.toSeq", scope["self"].Type);
				return Const.UNIT;
			}) {
				Name = "toSeq",
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember(Constants.GETI, new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IType typeParameter = self.Type;
				IEnumerable<IValue> values = self.ToSeq(scope);

				List<IValue> indices = scope["indices"].ToList(scope);

				if (indices.Count == 1) {
					IValue index = indices[0];

					if (index is Fun fun) {
						return Helper.FromSeq(typeParameter,
							values.Where(x => fun.Call(new Scope(scope), x).ToBoolean()), scope);
					}

					static (Int32, Int32?) NormalizeIndex(Int32 index, IEnumerable<IValue> values) {
						if (index < 0) {
							Int32 count = values.Count();
							return (Helper.Index(index, count), count);
						}

						return (index, null);
					}

					if (index is Number) {
						(Int32 intIndex, Int32? count) = NormalizeIndex(index.ToInt(scope), values);

						if (intIndex < 0 || (count != null && intIndex >= count)) {
							throw Helper.IndexOutOfRange();
						}

						try {
							return values.ElementAt(intIndex);
						}
						catch (ArgumentOutOfRangeException) {
							throw Helper.IndexOutOfRange();
						}
					}

					if(index is InfinityRange infinityRange) {
						if (infinityRange.Step == 1 || infinityRange.Step == 0) {
							return Helper.FromSeq(typeParameter, values.Select(i => i), scope);
						} else if(infinityRange.IsDownToUp) {
							return Helper.FromSeq(typeParameter, Step(values.Select(i => i), (Int32)infinityRange.Step), scope);
						} else {
							return Helper.FromSeq(typeParameter, 
								Step(values, Math.Abs((Int32)infinityRange.Step)).Reverse(), scope);
						}
					}

					if (index is NumberRange numberRange) {
						Int32 start = (Int32)numberRange.StartOr(0);
						Int32 step = (Int32)numberRange.Step;

						// start...
						if (!numberRange.HasEnd) {
							if (start < 0) {
								(start, _) = NormalizeIndex(start, values);
							}

							if (step == 1 || step == 0) {
								return Helper.FromSeq(typeParameter, values.Skip(start), scope);
							}
							else {
								return Helper.FromSeq(typeParameter, 
									Step(values.Skip(start).Reverse(), Math.Abs(step)), scope);
							}
						}
						else {
							// start..end, start...end
							Int32 end = (Int32)numberRange.End.Value;

							Int32? count = null;
							if (start < 0) {
								(start, count) = NormalizeIndex(start, values);
							}

							if (end < 0) {
								if (count.HasValue) {
									end = Helper.Index(end, count.Value);
								}
								else {
									(end, count) = NormalizeIndex(end, values);
								}
							}

							if(numberRange.IsInclusive && !numberRange.IsDownToUp && start <= end) {
								end += 2;
							}

							if (start < 0 || (count != null && start >= count)) {
								throw Helper.IndexOutOfRange();
							}

							if (end < 0 || (count != null && end >= count)) {
								throw Helper.IndexOutOfRange();
							}

							if (numberRange.IsInclusive && numberRange.IsDownToUp && start >= end) {
								end -= 2;
							}

							static IEnumerable<IValue> RangeIt(IEnumerable<IValue>
								source, Int32 start, Int32 end, Int32 step) {

								Int32 current = start;

								Boolean isDownToUp = start <= end;

								IValue currentValue = null;
								if (isDownToUp) {
									while (current < end) {
										try {
											currentValue= source.ElementAt(current);
										}
										catch (ArgumentOutOfRangeException) {
											throw Helper.IndexOutOfRange();
										}
										current += step;
										yield return currentValue;
									}
								}
								else {
									while (current > end) {
										try {
											currentValue = source.ElementAt(current);
										}
										catch (ArgumentOutOfRangeException) {
											throw Helper.IndexOutOfRange();
										}
										current -= step;
										yield return currentValue;
									}
								}
							}

							return Helper.FromSeq(typeParameter,
								RangeIt(values, start, end, Math.Abs(step)), scope);
						}

					}

					Int32 actualIndex = 0;
					Int32? counted = null;

					return Helper.FromSeq(typeParameter, index.ToSeq(scope).Select(i => {
						if (i is Number) {
							if (counted == null) {
								(actualIndex, counted) = NormalizeIndex(i.ToInt(scope), values);
							}
							else {
								actualIndex = Helper.Index(i.ToInt(scope), counted.Value);
							}

							if (actualIndex < 0 || (counted != null && actualIndex >= counted)) {
								throw Helper.IndexOutOfRange();
							}

							try {
								return values.ElementAt(actualIndex);
							}
							catch (ArgumentOutOfRangeException) {
								throw Helper.IndexOutOfRange();
							}
						}
						else {
							throw new LumenException(Exceptions.TYPE_ERROR.F(Prelude.Number, i.Type));
						}
					}), scope);
				}

				throw Helper.InvalidArgument("indices", "indexation support only one index");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("indices"),
					new ExactTypePattern("self", this),
				}
			});

			this.SetMember(Constants.USTAR, new LambdaFun((scope, args) => (Number)scope["values"].ToSeq(scope).Count()) {
				Parameters = new List<IPattern> { new NamePattern("values") }
			});

			this.SetMember(Constants.SLASH, new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IValue foldf = scope["foldf"];

				if (foldf is Number num) {
					if(self is NumberRange numberRange) {
						return numberRange.Clone(num.value);
					}

					if (self is InfinityRange infinityRange) {
						return new InfinityRange(num.value);
					}

					IEnumerable<IValue> value = self.ToSeq(scope);

					return new Flow(Step(value, foldf.ToInt(scope)));
				}

				IEnumerable<IValue> values = self.ToSeq(scope);

				Fun func = scope["foldf"].ToFunction(scope);

				return values.Aggregate((x, y) => func.Call(new Scope(scope), x, y));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("foldf")
				}
			});

			this.SetMember(Constants.STAR, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<IValue> value = scope["values"].ToSeq(scope);
				IValue other = scope["x"];

				if (other == Const.UNIT) {
					return (Number)value.Count();
				}

				return other switch
				{
					Text text => new Text(String.Join((String)text, value)),
					Fun fun => new Flow(value.Select(it => fun.Call(new Scope(scope), it))),
					_ => new Flow(this.Cycle(value, (Int32)other.ToDouble(scope))),
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("x")
				}
			});

			this.SetMember(Constants.MINUS, new LambdaFun((scope, args) => {
				IEnumerable<IValue> values = scope["values"].ToSeq(scope);
				IEnumerable<IValue> valuesx = scope["values'"].ToSeq(scope);

				return new Flow(values.Except(valuesx));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});

			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
				IEnumerable<IValue> values = scope["values"].ToSeq(scope);
				IEnumerable<IValue> valuesx = scope["values'"].ToSeq(scope);

				return new Flow(values.Concat(valuesx));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});


			static IValue Sum(IEnumerable<IValue> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.PLUS, out IValue plusUnchecked)
							&& plusUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Call(scope, x, y);
					}

					throw Helper.InvalidOperation("all the values in collection should support + operator");
				});
			}

			static IValue Min(IEnumerable<IValue> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.SHIP, out IValue comparatorUnchecked)
							&& comparatorUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Call(scope, x, y).ToInt(scope) < 0 ? x : y;
					}

					throw Helper.InvalidOperation("all the values in collection should implement Ord class");
				});
			}

			static IValue Max(IEnumerable<IValue> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.SHIP, out IValue plusUnchecked)
							&& plusUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Call(scope, x, y).ToInt(scope) > 0 ? x : y;
					}

					throw Helper.InvalidOperation("all the values in collection should implement Ord class");
				});
			}

			this.SetMember("average", new LambdaFun((scope, args) => {
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				try {
					IValue sum = Sum(values, scope);

					if (sum.Type.TryGetMember(Constants.SLASH, out IValue divUnchecked)
						&& divUnchecked.TryConvertToFunction(out Fun div)) {
						return div.Call(scope, sum, new Number(values.Count()));
					}

					throw Prelude.InvalidOperation.MakeExceptionInstance(
						new Text("all the values in collection should support / operator"));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Name = "Collection.average",
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("min", new LambdaFun((scope, args) => {
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				try {
					return Min(values, scope);
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("max", new LambdaFun((scope, args) => {
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				try {
					return Max(values, scope);
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});


			this.SetMember("fold", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				try {
					return values.Aggregate((x, y) => folder.Call(new Scope(scope), x, y));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("folder")
				}
			});

			this.SetMember("foldInit", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				IValue init = scope["init"];

				return values.Aggregate(init, (x, y) => folder.Call(new Scope(scope), x, y));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("init"),
					new NamePattern("folder")
				}
			});

			this.SetMember("foldr", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				try {
					return values.Reverse().Aggregate((x, y) => folder.Call(new Scope(scope), x, y));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("folder")
				}
			});

			this.SetMember("foldrInit", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				IValue init = scope["init"];

				return values.Reverse().Aggregate(init, (x, y) => folder.Call(new Scope(scope), x, y));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("init"),
					new NamePattern("folder")
				}
			});


			this.SetMember("first", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				IValue result = self.FirstOrDefault();

				return (result == null) ? Prelude.None : (IValue)Helper.CreateSome(result);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("last", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				IValue result = self.LastOrDefault();

				return (result == null) ? Prelude.None : (IValue)Helper.CreateSome(result);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});


			this.SetMember("count", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				return new Number(self.Count());
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("countOf", new LambdaFun((scope, args) => {
				IValue elem = scope["elem"];
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				return new Number(self.Count(i => elem.Equals(i)));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , 
					new NamePattern("elem")
				}
			});

			this.SetMember("countWhen", new LambdaFun((scope, args) => {
				IValue pred = scope["pred"];
				IEnumerable<IValue> stream = scope["self"].ToSeq(scope);

				Fun fun = pred.ToFunction(scope);
				return new Number(stream.Count(i => fun.Call(new Scope(scope), i).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("pred")
				}
			});


			this.SetMember("isAll", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				return new Logical(self.All(x => predicate.Call(new Scope(scope), x).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});

			this.SetMember("isAny", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				return new Logical(self.Any(x => predicate.Call(new Scope(scope), x).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});


			this.SetMember("filter", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToSeq(scope);

				return new Flow(values.Where(i => predicate.Call(new Scope(scope), i).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("predicate"),
					new ExactTypePattern("self", this),
				}
			});
			
			this.SetMember("map", new LambdaFun((scope, args) => {
				IValue fc = scope["fc"];
				IType typeParameter = fc.Type;
				Fun mapper = scope["fn"].ToFunction(scope);
				IEnumerable<IValue> values = fc.ToSeq(scope);

				return Helper.FromSeq(typeParameter,
					values.Select(i => mapper.Call(new Scope(scope), i)), scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new ExactTypePattern("fc", this),
				}
			});
			
			this.SetMember("mapi", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				Fun mapper = scope["mapper"].ToFunction(scope);
				IEnumerable<IValue> values = self.ToSeq(scope);
				IType typeParameter = self.Type;

				return Helper.FromSeq(typeParameter, values.Select((i, index) => mapper.Call(new Scope(scope), new Number(index), i)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("mapper")
				}
			});


			static IEnumerable<IValue> Flatten2(IEnumerable<IEnumerable<IValue>> list, Scope s) {
				foreach (IEnumerable<IValue> item in list) {
					foreach (IValue item2 in item) {
						yield return item2;
					}
				}
			}
			
			this.SetMember("lift", new LambdaFun((scope, args) => {
				IEnumerable<IValue> obj = scope["other"].ToSeq(scope);
				IValue self = scope["self"];
				IEnumerable<IValue> selfStream = self.ToSeq(scope);

				return Helper.FromSeq(
					scope["other"].Type,
					Flatten2(obj.Select(i =>
						selfStream.Select(j =>
							i.ToFunction(scope).Call(new Scope(), j))), scope),
					scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("other"),
				}
			});

			this.SetMember("bind", new LambdaFun((scope, args) => {
				IEnumerable<IValue> monad = scope["monad"].ToSeq(scope);
				Fun fn = scope["fn"].ToFunction(scope);

				return Helper.FromSeq(
					scope["monad"].Type,
					Flatten2(monad.Select(i => fn.Call(new Scope(), i).ToSeq(scope)), scope),
					scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("monad"),
				}
			});

			this.SetMember("iter", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				foreach (IValue i in self) {
					action.Call(new Scope(scope), i);
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("action")
				}
			});

			this.SetMember("iteri", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);

				Int32 index = 0;
				foreach (IValue i in self) {
					action.Call(new Scope(scope), new Number(index), i);
					index++;
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("action")
				}
			});


			this.SetMember("find", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);
				Fun predicate = scope["predicate"].ToFunction(scope);

				foreach (IValue i in self) {
					if (predicate.Call(new Scope(scope), i).ToBoolean()) {
						return Helper.CreateSome(i);
					}
				}

				return Prelude.None;
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});

			this.SetMember("findAll", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);
				Fun predicate = scope["predicate"].ToFunction(scope);

				List<IValue> result = new List<IValue>();
				foreach (IValue i in self) {
					if (predicate.Call(new Scope(scope), i).ToBoolean()) {
						result.Add(i);
					}
				}

				return new MutArray(result);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});


			this.SetMember("sort", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				return Helper.FromSeq(self.Type, value.OrderBy(x => x), scope);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("sortBy", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun mutator = scope["other"].ToFunction(scope);
				return Helper.FromSeq(self.Type, value.OrderBy(i => mutator.Call(new Scope(scope), i)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("other")
				}
			});

			this.SetMember("sortWith", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun comparator = scope["other"].ToFunction(scope);
				return Helper.FromSeq(self.Type, value.OrderBy(i => i, new CompareUtil(comparator, scope)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("other")
				}
			});

			this.SetMember("sortDescending", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				return Helper.FromSeq(self.Type, value.OrderByDescending(x => x), scope);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("sortDescendingBy", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun mutator = scope["other"].ToFunction(scope);
				return Helper.FromSeq(self.Type, value.OrderByDescending(i => mutator.Call(new Scope(scope), i)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("other")
				}
			});

			this.SetMember("sortDescendingWith", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun comparator = scope["other"].ToFunction(scope);
				return Helper.FromSeq(self.Type, value.OrderByDescending(i => i, new CompareUtil(comparator, scope)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("other")
				}
			});

			this.SetMember("take", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Int32 count = scope["count"].ToInt(scope);

				return Helper.FromSeq(self.Type, value.Take(count), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("count")
				}
			});

			this.SetMember("takeWhile", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);

				return Helper.FromSeq(self.Type, value.TakeWhile(i => predicate.Call(new Scope(scope), i).ToBoolean()), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});

			this.SetMember("skip", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Int32 count = scope["count"].ToInt(scope);

				return Helper.FromSeq(self.Type, value.Skip(count), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("count")
				}
			});

			this.SetMember("skipWhile", new LambdaFun((scope, args) => {
				IValue self = scope["self"];
				IEnumerable<IValue> value = self.ToSeq(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);

				return Helper.FromSeq(self.Type, value.SkipWhile(i => predicate.Call(new Scope(scope), i).ToBoolean()), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("predicate")
				}
			});


			this.SetMember("unique", new LambdaFun((scope, args) => {
				IValue self = scope["self"];

				return Helper.FromSeq(self.Type, self.ToSeq(scope).Distinct(), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
				}
			});

			this.SetMember("difference", new LambdaFun((scope, args) => {
				IValue self = scope["self"];

				return Helper.FromSeq(self.Type,
					self.ToSeq(scope).Except(scope["other"].ToSeq(scope)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this) , new NamePattern("other")
				}
			});

			this.SetMember("intersection", new LambdaFun((scope, args) => {
				IValue self = scope["self"];

				return Helper.FromSeq(self.Type,
					self.ToSeq(scope).Intersect(scope["other"].ToSeq(scope)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
						new ExactTypePattern("other", this),
				}
			});

			this.SetMember("union", new LambdaFun((scope, args) => {
				IValue self = scope["self"];

				return Helper.FromSeq(self.Type,
					self.ToSeq(scope).Union(scope["other"].ToSeq(scope)), scope);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("other"),
				}
			});
			// isSubset


			this.SetMember("zip", new LambdaFun((e, args) => {
				IEnumerable<IValue> v = Converter.ToSeq(e.Get("this"), e);
				if (args.Length == 1) {
					return new MutArray(v.Zip<IValue, IValue, IValue>(Converter.ToSeq(args[0], e), (x, y) => new MutArray(new List<IValue> { x, y })).ToList());
				}

				return new Flow(v.Zip(Converter.ToSeq(args[0], e), (x, y) => ((Fun)args[1]).Call(new Scope(e), x, y)));
			}));

			this.SetMember("join", new LambdaFun((e, args) => {
				IEnumerable<IValue> self = e["self"].ToSeq(e);
				String delim = e["delim"].ToString();

				return new Text(String.Join(delim, self));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("delim"),
					new ExactTypePattern("self", this),
				}
			});

			this.SetMember("step", new LambdaFun((e, args) => {
				IEnumerable<IValue> v = Converter.ToSeq(e.Get("self"), e);

				return new Flow(Step(v, e["count"].ToInt(e)));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this),
					new NamePattern("count")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);
				return new Logical(self.Contains(scope["elem"]));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("elem"),
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("exists?", new LambdaFun((e, args) => {
				IEnumerable<IValue> v = Converter.ToSeq(e.Get("this"), e);
				return new Logical(v.FirstOrDefault(i => Converter.ToBoolean(((Fun)args[0]).Call(new Scope(e), i))) != null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("toList", new LambdaFun((scope, args) => {
				IEnumerable<IValue> self = scope["self"].ToSeq(scope);
				return new List(LinkedList.Create(self));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("self", this)
				}
			});

			this.SetMember("fromList", new LambdaFun((scope, args) => {
				IEnumerable<IValue> result = scope["self"].ToLinkedList(scope);
				return new Flow(result);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
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
		}

		private static IEnumerable<IValue> Step(IEnumerable<IValue> val, Int32 by) {
			Int32 indexer = by;
			foreach (IValue i in val) {
				if (by == indexer) {
					yield return i;
					indexer = 1;
				}
				else {
					indexer++;
				}
			}
		}
		/*
		private IEnumerable<Value> CycleInf(IEnumerable<Value> val) {
			while (true) {
				foreach (Value i in val) {
					yield return i;
				}
			}
		}*/

		private IEnumerable<IValue> Cycle(IEnumerable<IValue> val, Int32 count) {
			Int32 currentIndex = 0;
			while (currentIndex < count) {
				foreach (IValue i in val) {
					yield return i;
				}
				currentIndex++;
			}
		}

		/*private IEnumerable<Value> EachSlice(IEnumerable<Value> val, Int32 count) {
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
		}*/

		internal class CompareUtil : IComparer<IValue> {
			Fun comparator;
			Scope scope;

			public CompareUtil(Fun comparator, Scope scope) {
				this.comparator = comparator;
				this.scope = scope;
			}

			public Int32 Compare(IValue x, IValue y) {
				return (Int32)Converter.ToDouble(this.comparator.Call(new Scope(this.scope), x, y), this.scope);
			}
		}
	}
}
