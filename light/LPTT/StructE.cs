using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using static Stereotype.FunctionDefineStatement;

namespace Stereotype {
	public class StructE : Expression {
		public String name;
		public List<ArgumentMetadataGenerator> fields;
		public Int32 line;
		public String file;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public StructE(String nameType, List<ArgumentMetadataGenerator> fields, Int32 line, String file) {
			this.name = nameType;
			this.fields = fields;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope e) {
			Record result = new Record {
				meta = new TypeMetadata {
					Name = this.name
				}
			};

			List<FunctionArgument> fields = this.fields.Select(i => i.EvalArgumnet(e)).ToList();

			result.meta.Fields = fields;

			result.Set("()", new LambdaFun((innerScope, args) => {
				Objectn obj = new Objectn(this.fields.Count, result);

				foreach (var i in result.meta.Fields) {
					obj.Set(i.name, innerScope[i.name], AccessModifiers.PRIVATE, innerScope);
				}

				return obj;
			}) { Arguments = fields });

			result.SetAttribute(Op.EQL, new LambdaFun((innerScope, args) => {
				Objectn obj = innerScope.This as Objectn;
				Objectn obj2 = innerScope.This as Objectn;

				return (Bool)obj.Equals(obj2);
			}));

			result.SetAttribute("to_s", new LambdaFun((innerScope, args) => {
				Objectn obj = innerScope.This as Objectn;
	
				return new KString($"({String.Join(", ", (IEnumerable<Value>)obj.value)})");
			}));

			e.Set(this.name, result);

			return Const.NULL;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			Dictionary<String, Expression> new_fields = new Dictionary<String, Expression>();

			return null;// new StructE(this.name, new_fields, this.self.Select(i => i.Closure(visible, thread)).ToList(), this.attrs.Select(i => i.Closure(visible, thread)).ToList(), this.interfaces.Select(i => i.Closure(visible, thread)).ToList(), this.closed);
		}

		/*[Serializable]
		public class Initialize : SystemFun {
			List<FunctionArgument> args;

			public Initialize(List<FunctionArgument> args) {
				this.args = args;
			}

			public override Value Run(Scope kt, params Value[] args) {
				IObject th = (IObject)kt.Get("this");

				if (kt.ExistsInThisScope
					("base") && kt.Get("base") is Fun method) {
					method.Run(new TypedScope(kt) { ["this"] = th }, args);
				}

				Int32 index = 0;
				foreach (FunctionArgument i in this.args) {
					if (index < args.Length) {
						th.Set(i.name, args[index], AccessModifiers.PRIVATE, kt);
					}
					else {
						th.Set(i.name, (Value)i.defaultValue, AccessModifiers.PRIVATE, kt);
					}

					index++;
				}
				return Const.NULL;
			}
		}

		[Serializable]
		public class Create : SystemFun {
			KType obj;

			public Create(KType obj) {
				this.obj = obj;
			}

			public override List<FunctionArgument> Arguments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

			public override Value Run(Scope e, params Value[] args) {
				IObject result = null;

				int size = obj.meta.Fields.Length;

				switch (size) {
					case 0:
						result = new Object0(obj);
						break;
					case 1:
						result = new Object1(obj);
						break;
					case 2:
						result = new Object2(obj);
						break;
					case 3:
						result = new Object3(obj);
						break;
					case 4:
						result = new Object4(obj);
						break;
					default:
						result = new Objectn(size, obj);
						break;
				}

				Scope ex = new TypedScope(e) { ["this"] = result, ["<CTOR>"] = (Bool)true };

				if (obj.meta.BaseType.AttributeExists("initialize")) {
					ex.Set("base", new LambdaFun((scope, arguments) => {
						scope.Set("this", result);
						return ((Fun)obj.meta.BaseType.GetAttribute("initialize", e)).Run(scope, arguments);
					}));
				}

				((Fun)obj.GetAttribute("initialize", e)).Run(ex, args);

				return result;
			}
		}*/
	}
}