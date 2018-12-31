using System;
using System.Linq;

namespace Lumen.Lang.Std {
	internal class ObjectClass : Record {
		internal ObjectClass() {
			this.meta = new TypeMetadata {
				Name = "Kernel.Object",
			};

			SetAttribute("get_type", new LambdaFun((e, args) => e.Get("this").Type, "Kernel.Object.get_type"));
			SetAttribute("==", new LambdaFun((e, args) => new Bool(ReferenceEquals(e.Get("this"), args[0])), "Kernel.Object.=="));
			SetAttribute("eql?", new LambdaFun((e, args) => new Bool(ReferenceEquals(e.Get("this"), args[0])), "Kernel.Object.eql?"));
			SetAttribute("get_hash", new LambdaFun((e, args) => new Num(e.Get("this").GetHashCode()), "Kernel.Object.get_hash"));
			SetAttribute("clone", new LambdaFun((e, args) => e.Get("this"), "Kernel.Object.clone"));
			SetAttribute("respond?", new LambdaFun((e, args) => new Bool(e.Get("this").Type.AttributeExists(args[0].ToString())), "Kernel.Object.respond?"));
			SetAttribute("get_null?", new LambdaFun((e, args) => new Bool(e.Get("this") is Null), "Kernel.Object.get_null?"));
			SetAttribute("send", new LambdaFun((e, args) => {
				Value val = e.Get("this");
				Record type = val.Type;

				Value[] _args = new Value[args.Length - 1];
				Array.Copy(args, 1, _args, 0, args.Length - 1);
				Scope s = new Scope(e) {
					This = val
				};
				return ((Fun)type.GetAttribute(args[0].ToString(), e)).Run(s, _args);
			}, "Kernel.Object.send"));
			SetAttribute("ref_get_attr", new LambdaFun((e, args) => {
				Value val = e.Get("this");
				String name = args[0].ToString(e);
				if (val is IObject pbj) {
					return pbj.Get(name, AccessModifiers.PRIVATE, e);
				}
				return Const.NULL;
			}, "Kernel.Object.ref_get_attr"));
			SetAttribute("ref_set_attr", new LambdaFun((e, args) => {
				Value val = e.Get("this");
				String name = args[0].ToString(e);
				if (val is IObject pbj) {
					pbj.Set(name, args[1], AccessModifiers.PRIVATE, e);
				}
				return Const.NULL;
			}, "Kernel.Object.ref_set_attr"));
			SetAttribute("to_s", new LambdaFun((e, args) => {
				Value val = e.Get("this");
				return (KString)$"<object:{val.Type.meta.Name}>";
			}, "Kernel.Object.to_s"));

		/*	Set("new", new LambdaFun((e, args) => {
				return new Object0(this);
			}));
			*/
			Set("include?", new LambdaFun((e, args) => {
				Record type = e.Get("this") as Record;
				return (Bool)type.includedModules.Contains(args[0]);
			}, "Kernel.Object.include?"));
			Set("get_included", new LambdaFun((e, args) => {
				Record type = e.Get("this") as Record;
				return new Vec(type.includedModules.Cast<Value>().ToList());
			}, "Kernel.Object.get_included"));
			/*Set("get_methods", new LambdaFun((e, args) => {
				KType one = (KType)e.Get("this");
				return new List(one.Attributes.Select(i => i.Value).ToList());
			}));
			Set("get_type_methods", new LambdaFun((e, args) => {
				KType one = (KType)e.Get("this");
				return new List(one.TypeAttributes.Where(i => i.Value is Fun).Select(i => i.Value).ToList());
			}, "Kernel.Object.get_type_methods"));*/
			Set("get_type", new LambdaFun((e, args) => e.Get("this").Type, "Kernel.Object.get_type"));
			Set("==", new LambdaFun((e, args) => (Bool)ReferenceEquals(e.Get("this"), args[0]), "Kernel.Object.=="));
			Set("eql?", new LambdaFun((e, args) => (Bool)ReferenceEquals(e.Get("this"), args[0]), "Kernel.Object.eql?"));
			Set("get_hash", new LambdaFun((e, args) => (Num)e.Get("this").GetHashCode(), "Kernel.Object.get_hash"));
		}
	}
}
