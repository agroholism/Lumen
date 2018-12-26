using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class ExpandoType : KType {
		public static Expando BASE;

		public ExpandoType() {
			BASE = new BaseExpando();
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.Expando",
			};
			/*
						SetAttribute("taint", new LambdaFun(e => {
							HObject obj = (HObject)e.Get("this");
							if (obj.IsExists("tainted?") && KernelConverter.ToBoolean(obj.Get("tainted?")) == true) {
								return Const.NULL;
							}
							obj.Set("tainted?", Const.TRUE);
							return Const.TRUE;
						}));

						SetAttribute("to_l", new LambdaFun(e => {
							HObject obj = (HObject)e.Get("this");
							return new KList(KernelConverter.ToList((obj.Get("to_l") as Fun).Run(new Scope(e))));
						}));

						SetAttribute("to_i", new LambdaFun(e => {
							HObject obj = (HObject)e.Get("this");
							return new Iterator(KernelConverter.ToIterator((obj.Get("to_i") as Fun).Run(new Scope(e)), e));
						}));*/
		}

		private class BaseExpando : Expando {
			public BaseExpando() {
				Set("prototype", this, AccessModifiers.PUBLIC, null);

				this["set", null] = new LambdaFun((e, args) => {
					Expando obj = (Expando)e.Get("this");
					obj.Set(e["name"].ToString(e), e["value"], AccessModifiers.PUBLIC, e);
					return Const.TRUE;
				}) {
					Arguments = new List<FunctionArgument> {
								new FunctionArgument("name"),
								new FunctionArgument("value")
							}
				};
				this["taint", null] = new LambdaFun((e, args) => {
					Expando obj = (Expando)e.Get("this");
					if (obj.IsExists("tainted?") && Converter.ToBoolean(obj.Get("tainted?", AccessModifiers.PUBLIC, e)) == true) {
						return Const.FALSE;
					}
					obj.Set("tainted?", Const.TRUE, AccessModifiers.PUBLIC, e);
					return Const.TRUE;
				});
				this["freeze", null] = new LambdaFun((e, args) => {
					Expando obj = (Expando)e.Get("this");
					if (Converter.ToBoolean(obj.Get("frosen?", AccessModifiers.PUBLIC, e)) == true) {
						return Const.FALSE;
					}
					obj.Set("frosen?", Const.TRUE, AccessModifiers.PUBLIC, e);
					return Const.TRUE;
				});
				this["to_s", null] = new LambdaFun((e, args) => {
					return (KString)"";
				});
				this["missing", null] = new LambdaFun((e, args) => {
					throw new Exception("объект не содержит поля " + e["name"], stack: e);
				}) {
					Arguments = new List<FunctionArgument> {
								new FunctionArgument("name")
							}
				};
				this["tainted?", null] = new Bool(false);
				this["frosen?", null] = new Bool(false);
			}

			public override Boolean IsExists(String name) {
				return this.Fields.ContainsKey(name);
			}
		}
	}
}
