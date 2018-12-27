using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class Applicate : Expression {
		public Expression callable;
		public List<Expression> argse;
		public Int32 Line;

		public Applicate(Expression callable, List<Expression> args, Int32 line) {
			this.callable = callable;
			this.argse = args;
			this.Line = line;
		}

		internal Value ExecuteCasual(Fun function, Scope e) {
			// Спиок аргументов функции.
			List<Value> arguments = new List<Value>();
			// Область видимости, в которой выполняется функция.
			Scope innerScope = new Scope(e) { ["self"] = function };

			Boolean isPartial = false;

			List<Int32> placeholders = new List<int>();

			Dictionary<Value, Value> kwargs = new Dictionary<Value, Value>();
			List<Value> args = new List<Value>();

			Int32 pos = 0;
			// Вычисление аргументов функции.
			foreach (Expression i in this.argse) {
				if (i is Assigment ass_e) {
					if (function.Arguments.Exists(arg => arg.name == ass_e.id)) {
						Value argument = ass_e.exp.Eval(e);
						innerScope.Set(ass_e.id, argument);
						args.Add(argument);
						pos++;
					}
					else {
						kwargs[new KString(ass_e.id)] = ass_e.exp.Eval(e);
					}

				}
				else if (i is IdExpression id && id.id == "_") {
					isPartial = true;
					placeholders.Add(pos);
				}
				else if (i is SpreadE spread) {
					Value container = i.Eval(e);
					if (container is Map map) {
						UserFun userFunction = function as UserFun;
						foreach (KeyValuePair<Value, Value> item in map.value) {
							if (item.Key is KString str && (userFunction != null && userFunction.Arguments.Exists(arg => arg.name == str.ToString()))) {
								innerScope.Set(str.ToString(e), item.Value);
								args.Add(item.Value);
								pos++;
							}
							else if (userFunction != null) {
								kwargs[item.Key] = item.Value;
							}
							else {
								kwargs[item.Key] = item.Value;
								innerScope.Set(item.Key.ToString(e), item.Value);
							}
						}
					}
					else {
						foreach (Value argument in DeSpread(container, spread.i, e)) {
							arguments.Add(argument);
							args.Add(argument);
							pos++;
						}
					}
				}
				else {
					Value arg = i.Eval(e);
					arguments.Add(arg);
					args.Add(arg);
				}
				pos++;
			}

			if (isPartial) {
				var a = new List<FunctionArgument>();
				foreach (var i in placeholders) {
					a.Add(new FunctionArgument("x" + i));
				}
				var fmtd = a;

				var nexps = new List<Expression>();
				var x = 0;
				foreach (var exp in this.argse) {
					if (exp is IdExpression ide && ide.id == "_") {
						nexps.Add(new IdExpression("x" + placeholders[x], ide.line, ide.file));
						x++;
					}
					else {
						nexps.Add(exp);
					}
				}

				UserFun f = new UserFun(fmtd, new Applicate(new ValueE(function), nexps, this.Line));
				f.Attributes["name"] = (KString)(function.Attributes["name"] + "'");
				return f;
			}

			innerScope["kwargs"] = new Map(kwargs);
			innerScope["args"] = new Vec(args);

			if (!Provider.isRelease) {
				Value name = (KString)String.Empty;
				if (function is UserFun sfn) {
					if (sfn.Attributes.ContainsKey("name")) {
						name = sfn.Attributes["name"];
					}

					if (name.ToString() == "") {
						name = (KString)this.callable.ToString();
					}
				}
				else if (function is LambdaFun lfun) {
					if (lfun.Attributes.ContainsKey("name")) {
						name = lfun.Attributes["name"];
					}
					if (name.ToString() == "") {
						name = (KString)this.callable.ToString();
					}
				}
				else {
					name = (KString)this.callable.ToString();
				}

				Stopwatch s = new Stopwatch();
				Int32 before = Provider.profileResults.Count;
				s.Start();
				Value result = null;
				try {
					result = function.Run(innerScope, arguments.ToArray());
				} catch(Lumen.Lang.Std.Exception ex) {
					ex.line = this.Line;
					throw;
				}
				s.Stop();
				Int32 after = Provider.profileResults.Count;
				Provider.profileResults.Add(new ProfileResult {
					div = after - before,
					incl_time = s.ElapsedMilliseconds,
					excl_time = s.ElapsedMilliseconds,
					name = name.ToString()
				});
				return result;
			}
			else {
				Value result = function.Run(innerScope, arguments.ToArray());
				return result;
			}
		}

		private static IEnumerable<Value> DeSpread(Value obj, Int32 deep, Scope e) {
			foreach (Value element in Converter.ToIterator(obj, e)) {
				if (deep > 0) {
					foreach (Value subElement in DeSpread(element, deep - 1, e)) {
						yield return subElement;
					}
				}
				else {
					yield return element;
				}
			}
		}

		public override string ToString() {
			return callable.ToString() + "(" + String.Join(", ", argse) + ")";
		}

		public Value Eval(Scope e) {
			Value callable = this.callable.Eval(e);

			switch (callable) {
				case Module m:
					return ExecuteCasual((Fun)m.Get("()"), e);
				case Fun _:
					return ExecuteCasual((Fun)callable, e);
				case Expando h:
					return ExecuteCasual((Fun)h.Get("call", AccessModifiers.PUBLIC, e), new Scope(e) { ["this"] = h });
				case IObject y:
					return ExecuteCasual((Fun)y.Type.GetAttribute("call", e), new Scope(e) { ["this"] = y });
				case Record x:
					return this.ExecuteCasual((Fun)x.Get("()", e), e);
			}

			return Const.NULL;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new Applicate(this.callable.Closure(visible, thread), this.argse.Select(i => i.Closure(visible, thread)).ToList(), this.Line);
		}

		public Expression Optimize(Scope scope) {
			return new Applicate(this.callable, this.argse.Select(i => i.Optimize(scope)).ToList(), this.Line);
		}
	}
}