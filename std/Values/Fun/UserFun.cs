using System;
using System.Collections.Generic;
using System.Text;

using Lumen.Lang.Expressions;

namespace Lumen.Lang.Std {
	public class UserFun : Fun, IObject {
		public Expression body;
		public Expression condition;
		public Dictionary<String, Value> Attributes { get; set; }
		public Boolean isInline;

		public UserFun() {
			this.Attributes = new Dictionary<String, Value>();
		}

		public UserFun(List<FunctionArgument> fmtd, Expression Body) : this() {
			this.Arguments = fmtd;
			this.body = Body;
		}

		public UserFun(List<FunctionArgument> fmtd, Expression Body, Expression condition) : this(fmtd, Body) {
			this.condition = condition;
		}

		public Record Type => StandartModule.Function;

		public List<FunctionArgument> Arguments { get; set; }

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			StringBuilder result = new StringBuilder();
			result.Append($"fun {this.Attributes["name"]}(");
			foreach (FunctionArgument arg in this.Arguments) {
				result.Append(arg.name);
				if (arg.defaultValue != null) {
					result.Append("=" + arg.defaultValue.ToString());
				}
				result.Append(", ");
			}
			result.Remove(result.Length - 2, 2);
			String str = this.body.ToString();
			result.Append(")");
			result.Append(str.StartsWith("{") ? "" : " {\n\t");
			result.Append(str);
			result.Append(str.StartsWith("{") ? "" : "\n}");
			return result.ToString();
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public Value Run(Scope e, params Value[] args) {
tail_recursion:
			Int32 counter = 0;

			foreach (FunctionArgument i in this.Arguments) {
				if (e.ExistsInThisScope(i.name.Replace("*", ""))) {
					continue;
				}

				if (counter < args.Length || (args.Length == 0 && counter == args.Length)) {
					if (i.name.StartsWith("*")) {
						List<Value> v = new List<Value>();

						for (Int32 z = counter; z < args.Length; z++) {
							v.Add(args[z]);
							counter++;
						}

						e.Set(i.name.Substring(1, i.name.Length - 1), new Vec(v));
						continue;
					}

					if (args.Length != 0) {
						if (i.Attributes != null && i.Attributes.TryGetValue("type", out Value t)) {
							if (t != args[counter].Type) {
								throw new Exception("type error ");
							}
						}

						e.Set(i.name, args[counter]);
						counter++;
					}
					else {
						if (i.defaultValue != null) {
							if (i.defaultValue is Value val) {
								e.Set(i.name, val);
							}
							else if (i.defaultValue is Expression exp) {
								e.Set(i.name, exp.Eval(e));
							}
						}
						else {
							e.Set(i.name, Const.NULL);
						}

						counter++;
					}
				}
				else {
					if (i.defaultValue != null) {
						if (i.defaultValue is Value val) {
							e.Set(i.name, val);
						}
						else if (i.defaultValue is Expression exp) {
							e.Set(i.name, exp.Eval(e));
						}
					}
					else {
						e.Set(i.name, Const.NULL);
					}
				}
			}

			if (this.condition != null && !Converter.ToBoolean(this.condition.Eval(e))) {
				throw new Exception("contract has broken", stack: e);
			}

			Value result = null;
			try {
				result = this.body.Eval(e);
			}
			catch (Return rt) {
				result = rt.Result;
			}
			catch (GotoE ex) {
				args = ex.result;
				foreach (FunctionArgument i in this.Arguments) {
					e.Remove(i.name);
				}
				goto tail_recursion;
			}

			if(this.Attributes.TryGetValue("returned", out Value ret)) {
				if(!(result.Type == ret)) {
					throw new Exception("wrong ret type");
				}
			}

			return result;
		}

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			return this.Attributes[name];
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			this.Attributes[name] = value;
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}
	}
}
