using System;
using System.Collections.Generic;
using System.Text;

using Lumen.Lang.Expressions;

namespace Lumen.Lang.Std {
	public class UserFun : Fun, IObject {
		public Expression body;
		public Expression condition;
		public Dictionary<String, Value> Attributes { get; set; }

		public IObject returned;

		public IObject Prototype {
			get {
				this.Attributes.TryGetValue("@prototype", out Value result);
				return result as IObject;
			}
			set => this.Attributes["@prototype"] = value;
		}

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

		public IObject Type => this.Prototype;

		public List<FunctionArgument> Arguments { get; set; }

		public String ToString(Scope scope) {
			if (this.TryGet("@name", out Value result)) {
				return result.ToString(scope);
			}

			return "";
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
							if (t is IObject iobject && !iobject.IsParentOf(args[counter])) {
								throw new Exception($"type error: wait {iobject} given ?");
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
							e.Set(i.name, Const.VOID);
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
						e.Set(i.name, Const.VOID);
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

			if(this.returned != null) {
				if(!this.returned.IsParentOf(result)) {
					throw new Exception("wrong ret type");
				}
			}

			return result;
		}

		public Boolean IsParentOf(Value value) {
			if (value is IObject parent) {
				while (true) {
					if (parent.TryGet("@prototype", out var v)) {
						parent = v as IObject;
						if (parent == this) {
							return true;
						}
					}
					else {
						break;
					}
				}
			}
			return false;
		}

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}

		public Value Get(String name, Scope e) {
			if (this.Attributes.TryGetValue(name, out var result)) {
				return result;
			}

			if (this.Prototype != null) {
				if (this.Prototype.TryGet(name, out result)) {
					return result;
				}
			}

			throw new Exception($"record does not contains a field {name}", stack: e);
		}

		public void Set(String name, Value value, Scope e) {
			this.Attributes[name] = value;
		}

		public Boolean TryGet(String name, out Value result) {
			result = null;

			if (this.Attributes.TryGetValue(name, out result)) {
				return true;
			}

			if (this.Prototype != null) {
				if (this.Prototype.TryGet(name, out result)) {
					return true;
				}
			}

			return false;
		}
	}
}
