using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Collection : Module {
		internal Collection() {
			this.Name = "Collection";

			this.AppendImplementation(Prelude.Functor);
			this.AppendImplementation(Prelude.Applicative);

			this.SetMember("toStream", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Collection.toStream", scope["self"].Type, scope);
				return Const.UNIT;
			}) {
				Name = "toStream",
				Arguments = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("fromStream", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Name = "fromStream",
				Arguments = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember(Constants.GETI, new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IType typeParameter = self.Type;
				IEnumerable<Value> values = self.ToStream(scope);

				List<Value> indices = scope["indices"].ToList(scope);

				if (indices.Count == 1) {
					Value index = indices[0];

					if (index is Fun fun) {
						return Helper.FromStream(typeParameter,
							values.Where(x => fun.Run(new Scope(scope), x).ToBoolean()), scope);
					}

					static (Int32, Int32?) NormalizeIndex(Int32 index, IEnumerable<Value> values) {
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
							return Helper.FromStream(typeParameter, values.Select(i => i), scope);
						} else if(infinityRange.IsDownToUp) {
							return Helper.FromStream(typeParameter, Step(values.Select(i => i), (Int32)infinityRange.Step), scope);
						} else {
							return Helper.FromStream(typeParameter, 
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
								return Helper.FromStream(typeParameter, values.Skip(start), scope);
							}
							else {
								return Helper.FromStream(typeParameter, 
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

							static IEnumerable<Value> RangeIt(IEnumerable<Value>
								source, Int32 start, Int32 end, Int32 step) {

								Int32 current = start;

								Boolean isDownToUp = start <= end;

								Value currentValue = null;
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

							return Helper.FromStream(typeParameter,
								RangeIt(values, start, end, Math.Abs(step)), scope);
						}

					}

					Int32 actualIndex = 0;
					Int32? counted = null;

					return Helper.FromStream(typeParameter, index.ToStream(scope).Select(i => {
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
				Arguments = new List<IPattern> {
					new NamePattern("indices"),
					new TypePattern("self", this)
				}
			});

			this.SetMember(Constants.USTAR, new LambdaFun((scope, args) => (Number)scope["values"].ToStream(scope).Count()) {
				Arguments = new List<IPattern> { new NamePattern("values") }
			});

			this.SetMember(Constants.SLASH, new LambdaFun((scope, args) => {
				Value self = scope["self"];
				Value foldf = scope["foldf"];

				if (foldf is Number num) {
					if(self is NumberRange numberRange) {
						return numberRange.Clone(num.value);
					}

					if (self is InfinityRange infinityRange) {
						return new InfinityRange(num.value);
					}

					IEnumerable<Value> value = self.ToStream(scope);

					return new Stream(Step(value, foldf.ToInt(scope)));
				}

				IEnumerable<Value> values = self.ToStream(scope);

				Fun func = scope["foldf"].ToFunction(scope);

				return values.Aggregate((x, y) => func.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("foldf")
				}
			});

			this.SetMember(Constants.STAR, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<Value> value = scope["values"].ToStream(scope);
				Value other = scope["x"];

				if (other == Const.UNIT) {
					return (Number)value.Count();
				}

				return other switch
				{
					Text text => new Text(String.Join((String)text, value)),
					Fun fun => new Stream(value.Select(it => fun.Run(new Scope(scope), it))),
					_ => new Stream(this.Cycle(value, (Int32)other.ToDouble(scope))),
				};
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("x")
				}
			});

			this.SetMember(Constants.MINUS, new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["values"].ToStream(scope);
				IEnumerable<Value> valuesx = scope["values'"].ToStream(scope);

				return new Stream(values.Except(valuesx));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});

			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
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


			static Value Sum(IEnumerable<Value> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.PLUS, out Value plusUnchecked)
							&& plusUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Run(scope, x, y);
					}

					throw Helper.InvalidOperation("all the values in collection should support + operator");
				});
			}

			static Value Min(IEnumerable<Value> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.SHIP, out Value comparatorUnchecked)
							&& comparatorUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Run(scope, x, y).ToInt(scope) < 0 ? x : y;
					}

					throw Helper.InvalidOperation("all the values in collection should implement Ord class");
				});
			}

			static Value Max(IEnumerable<Value> values, Scope scope) {
				return values.Aggregate((x, y) => {
					if (x.Type.TryGetMember(Constants.SHIP, out Value plusUnchecked)
							&& plusUnchecked.TryConvertToFunction(out Fun plus)) {
						return plus.Run(scope, x, y).ToInt(scope) > 0 ? x : y;
					}

					throw Helper.InvalidOperation("all the values in collection should implement Ord class");
				});
			}

			this.SetMember("average", new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				try {
					Value sum = Sum(values, scope);

					if (sum.Type.TryGetMember(Constants.SLASH, out Value divUnchecked)
						&& divUnchecked.TryConvertToFunction(out Fun div)) {
						return div.Run(scope, sum, new Number(values.Count()));
					}

					throw Prelude.InvalidOperation.MakeExceptionInstance(
						new Text("all the values in collection should support / operator"));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});

			this.SetMember("min", new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				try {
					return Min(values, scope);
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});

			this.SetMember("max", new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				try {
					return Max(values, scope);
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});


			this.SetMember("fold", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				try {
					return values.Aggregate((x, y) => folder.Run(new Scope(scope), x, y));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("folder"),
					new TypePattern("self", this)
				}
			});

			this.SetMember("foldInit", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				Value init = scope["init"];

				return values.Aggregate(init, (x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("folder"),
					new TypePattern("self", this)
				}
			});

			this.SetMember("foldr", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				try {
					return values.Reverse().Aggregate((x, y) => folder.Run(new Scope(scope), x, y));
				}
				catch (InvalidOperationException) {
					throw Prelude.CollectionIsEmpty.MakeExceptionInstance();
				}
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("folder"),
					new TypePattern("self", this)
				}
			});

			this.SetMember("foldrInit", new LambdaFun((scope, args) => {
				Fun folder = scope["folder"].ToFunction(scope);
				IEnumerable<Value> values = scope["self"].ToStream(scope);

				Value init = scope["init"];

				return values.Reverse().Aggregate(init, (x, y) => folder.Run(new Scope(scope), x, y));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("folder"),
					new TypePattern("self", this)
				}
			});


			this.SetMember("first", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				Value result = self.FirstOrDefault();

				return (result == null) ? Prelude.None : (Value)Helper.CreateSome(result);
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});

			this.SetMember("last", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				Value result = self.LastOrDefault();

				return (result == null) ? Prelude.None : (Value)Helper.CreateSome(result);
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});


			this.SetMember("count", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				return new Number(self.Count());
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});

			this.SetMember("countOf", new LambdaFun((scope, args) => {
				Value elem = scope["elem"];
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				return new Number(self.Count(i => elem.Equals(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elem"),
					new TypePattern("self", this)
				}
			});

			this.SetMember("countWhen", new LambdaFun((scope, args) => {
				Value pred = scope["pred"];
				IEnumerable<Value> stream = scope["self"].ToStream(scope);

				Fun fun = pred.ToFunction(scope);
				return new Number(stream.Count(i => fun.Run(new Scope(scope), i).ToBoolean()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("pred"),
				new TypePattern("self", this),
				}
			});


			this.SetMember("isAll", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				return new Logical(self.All(x => predicate.Run(new Scope(scope), x).ToBoolean()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("isAny", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				return new Logical(self.Any(x => predicate.Run(new Scope(scope), x).ToBoolean()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
					new TypePattern("self", this),
				}
			});


			this.SetMember("filter", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<Value> values = self.ToStream(scope);
				IType typeParameter = self.Type;

				return Helper.FromStream(typeParameter, values.Where(i => predicate.Run(new Scope(scope), i).ToBoolean()), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
					new TypePattern("self", this),
				}
			});

			LambdaFun map = new LambdaFun((scope, args) => {
				Value fc = scope["fc"];
				IType typeParameter = fc.Type;
				Fun mapper = scope["fn"].ToFunction(scope);
				IEnumerable<Value> values = fc.ToStream(scope);

				return Helper.FromStream(typeParameter,
					values.Select(i => mapper.Run(new Scope(scope), i)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new TypePattern("fc", this),
				}
			};

			this.SetMember("fmap", map);

			this.SetMember("map", map);

			this.SetMember("mapi", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				Fun mapper = scope["mapper"].ToFunction(scope);
				IEnumerable<Value> values = self.ToStream(scope);
				IType typeParameter = self.Type;

				return Helper.FromStream(typeParameter, values.Select((i, index) => mapper.Run(new Scope(scope), new Number(index), i)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("mapper"),
					new TypePattern("self", this),
				}
			});


			static IEnumerable<Value> Flatten2(IEnumerable<IEnumerable<Value>> list, Scope s) {
				foreach (IEnumerable<Value> item in list) {
					foreach (Value item2 in item) {
						yield return item2;
					}
				}
			}

			this.SetMember("liftA", new LambdaFun((scope, args) => {
				IEnumerable<Value> obj = scope["other"].ToStream(scope);
				Value self = scope["self"];
				IEnumerable<Value> selfStream = self.ToStream(scope);

				return Helper.FromStream(
					scope["other"].Type,
					Flatten2(obj.Select(i =>
						selfStream.Select(j =>
							i.ToFunction(scope).Run(new Scope(), j))), scope),
					scope);
			}) {
				Arguments = new List<IPattern> {
			new TypePattern("self", this),
					new NamePattern("other"),
				}
			});


			this.SetMember("iter", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				foreach (Value i in self) {
					action.Run(new Scope(scope), i);
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("action"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("iteri", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);
				IEnumerable<Value> self = scope["self"].ToStream(scope);

				Int32 index = 0;
				foreach (Value i in self) {
					action.Run(new Scope(scope), new Number(index), i);
					index++;
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("action"),
					new TypePattern("self", this),
				}
			});


			this.SetMember("find", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);
				Fun predicate = scope["predicate"].ToFunction(scope);

				foreach (Value i in self) {
					if (predicate.Run(new Scope(scope), i).ToBoolean()) {
						return Helper.CreateSome(i);
					}
				}

				return Prelude.None;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("findAll", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);
				Fun predicate = scope["predicate"].ToFunction(scope);

				List<Value> result = new List<Value>();
				foreach (Value i in self) {
					if (predicate.Run(new Scope(scope), i).ToBoolean()) {
						result.Add(i);
					}
				}

				return new Array(result);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
						new TypePattern("self", this),
				}
			});


			this.SetMember("sort", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				return Helper.FromStream(self.Type, value.OrderBy(x => x), scope);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("sortBy", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun mutator = scope["other"].ToFunction(scope);
				return Helper.FromStream(self.Type, value.OrderBy(i => mutator.Run(new Scope(scope), i)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
						new TypePattern("self", this),
				}
			});

			this.SetMember("sortWith", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun comparator = scope["other"].ToFunction(scope);
				return Helper.FromStream(self.Type, value.OrderBy(i => i, new CompareUtil(comparator, scope)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
						new TypePattern("self", this),
				}
			});

			this.SetMember("sortDescending", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				return Helper.FromStream(self.Type, value.OrderByDescending(x => x), scope);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("sortDescendingBy", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun mutator = scope["other"].ToFunction(scope);
				return Helper.FromStream(self.Type, value.OrderByDescending(i => mutator.Run(new Scope(scope), i)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
						new TypePattern("self", this),
				}
			});

			this.SetMember("sortDescendingWith", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun comparator = scope["other"].ToFunction(scope);
				return Helper.FromStream(self.Type, value.OrderByDescending(i => i, new CompareUtil(comparator, scope)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
						new TypePattern("self", this),
				}
			});


			this.SetMember("take", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Int32 count = scope["count"].ToInt(scope);

				return Helper.FromStream(self.Type, value.Take(count), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("count"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("takeWhile", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);

				return Helper.FromStream(self.Type, value.TakeWhile(i => predicate.Run(new Scope(scope), i).ToBoolean()), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
						new TypePattern("self", this),
				}
			});

			this.SetMember("skip", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Int32 count = scope["count"].ToInt(scope);

				return Helper.FromStream(self.Type, value.Skip(count), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("count"),
						new TypePattern("self", this),
				}
			});

			this.SetMember("skipWhile", new LambdaFun((scope, args) => {
				Value self = scope["self"];
				IEnumerable<Value> value = self.ToStream(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);

				return Helper.FromStream(self.Type, value.SkipWhile(i => predicate.Run(new Scope(scope), i).ToBoolean()), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
						new TypePattern("self", this),
				}
			});


			this.SetMember("unique", new LambdaFun((scope, args) => {
				Value self = scope["self"];

				return Helper.FromStream(self.Type, self.ToStream(scope).Distinct(), scope);
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this),
				}
			});

			this.SetMember("difference", new LambdaFun((scope, args) => {
				Value self = scope["self"];

				return Helper.FromStream(self.Type,
					self.ToStream(scope).Except(scope["other"].ToStream(scope)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("intersection", new LambdaFun((scope, args) => {
				Value self = scope["self"];

				return Helper.FromStream(self.Type,
					self.ToStream(scope).Intersect(scope["other"].ToStream(scope)), scope);
			}) {
				Arguments = new List<IPattern> {
						new TypePattern("other", this),
						new TypePattern("self", this),
				}
			});

			this.SetMember("union", new LambdaFun((scope, args) => {
				Value self = scope["self"];

				return Helper.FromStream(self.Type,
					self.ToStream(scope).Union(scope["other"].ToStream(scope)), scope);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new TypePattern("self", this),
				}
			});
			// isSubset


			this.SetMember("zip", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				if (args.Length == 1) {
					return new Array(v.Zip<Value, Value, Value>(Converter.ToStream(args[0], e), (x, y) => new Array(new List<Value> { x, y })).ToList());
				}

				return new Stream(v.Zip(Converter.ToStream(args[0], e), (x, y) => ((Fun)args[1]).Run(new Scope(e), x, y)));
			}));

			this.SetMember("join", new LambdaFun((e, args) => {
				IEnumerable<Value> self = e["self"].ToStream(e);
				String delim = e["delim"].ToString();

				return new Text(String.Join(delim, self));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("delim"),
					new TypePattern("self", this),
				}
			});

			this.SetMember("step", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("self"), e);

				return new Stream(Step(v, e["count"].ToInt(e)));
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this),
					new NamePattern("count")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);
				return new Logical(self.Contains(scope["elem"]));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elem"),
					new TypePattern("self", this)
				}
			});

			this.SetMember("exists?", new LambdaFun((e, args) => {
				IEnumerable<Value> v = Converter.ToStream(e.Get("this"), e);
				return new Logical(v.FirstOrDefault(i => Converter.ToBoolean(((Fun)args[0]).Run(new Scope(e), i))) != null);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("s"),
					new NamePattern("act")
				}
			});

			this.SetMember("toList", new LambdaFun((scope, args) => {
				IEnumerable<Value> self = scope["self"].ToStream(scope);
				return new List(LinkedList.Create(self));
			}) {
				Arguments = new List<IPattern> {
					new TypePattern("self", this)
				}
			});

			this.SetMember("fromList", new LambdaFun((scope, args) => {
				IEnumerable<Value> result = scope["self"].ToLinkedList(scope);
				return new Stream(result);
			}) {
				Arguments = new List<IPattern> {
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

		private static IEnumerable<Value> Step(IEnumerable<Value> val, Int32 by) {
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
		/*
		private IEnumerable<Value> CycleInf(IEnumerable<Value> val) {
			while (true) {
				foreach (Value i in val) {
					yield return i;
				}
			}
		}*/

		private IEnumerable<Value> Cycle(IEnumerable<Value> val, Int32 count) {
			Int32 currentIndex = 0;
			while (currentIndex < count) {
				foreach (Value i in val) {
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

		internal class CompareUtil : IComparer<Value> {
			Fun comparator;
			Scope scope;

			public CompareUtil(Fun comparator, Scope scope) {
				this.comparator = comparator;
				this.scope = scope;
			}

			public Int32 Compare(Value x, Value y) {
				return (Int32)Converter.ToDouble(this.comparator.Run(new Scope(this.scope), x, y), this.scope);
			}
		}
	}
}
