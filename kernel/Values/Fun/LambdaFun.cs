using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Std {
	public sealed class LambdaFun : Fun, IObject {
		public HFun value;
		public Dictionary<String, Value> Attributes { get; set; }

		public LambdaFun(HFun value) {
			this.value = value;
			this.Attributes = new Dictionary<string, Value>();
		}

		public LambdaFun(HFun value, String name) {
			this.value = value;
			this.Attributes = new Dictionary<string, Value>();
			this.Attributes["name"] = (KString)name;
		}

		public Value Run(Scope e, params Value[] args) {
go:
			Int32 counter = 0;

			foreach (FunctionArgument i in this.Arguments) {
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
						e.Set(i.name, args[counter]);
						counter++;
					}
					else {
						if (!e.ExistsInThisScope(i.name)) {
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
						counter++;
					}
				}
				else {
					if (!e.ExistsInThisScope(i.name)) {
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
			}


			Value result = null;
			try {
				result = value(e, args);
			}
			catch (Return rt) {
				result = rt.Result;
			}
			return result;
		}

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public KType Type => StandartModule.Function;

		public List<FunctionArgument> Arguments { get; set; } = new List<FunctionArgument>();

		public int CompareTo(object obj) {
			return 0;
		}

		public Value Clone() {
			return this;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			if (this.Attributes.TryGetValue(name, out Value result)) {
				return result;
			}

			if (this.Attributes.TryGetValue("get_" + name, out result)) {
				return result;
			}

			if (this.Type.AttributeExists("get_" + name) && this.Type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.Type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(string name, Value value, AccessModifiers mode, Scope e) {
			Attributes[name] = value;
		}

		public String ToString(Scope e) {
			if (this.Attributes.ContainsKey("name")) {
				return this.Attributes["name"].ToString(e);
			}

			return "";
		}
	}
}
