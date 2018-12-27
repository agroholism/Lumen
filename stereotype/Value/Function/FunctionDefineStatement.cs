using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class FunctionDefineStatement : Expression {
		public String NameFunction;
		public List<ArgumentMetadataGenerator> Args;
		public Expression Body;
		public Expression returnedType;
		public String doc;
		public List<Expression> otherContacts;
		public Int32 line;
		public String file;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public FunctionDefineStatement(String NameFunction, List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType) {
			this.NameFunction = NameFunction;
			this.Args = Args;
			this.Body = Body;
			this.returnedType = returnedType;
		}

		public FunctionDefineStatement(string NameFunction, List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType, List<Expression> otherContacts, Int32 line, String file) : this(NameFunction, Args, Body, returnedType) {
			this.otherContacts = otherContacts;
			this.line = line;
			this.file = file;
		}

		public override String ToString() {
			String result = "fun " + this.NameFunction + "(" + String.Join(", ", this.Args.Select(i => i.name)) + ")" + "{" + this.Body + "}";
			return result;
		}

		public Value Eval(Scope e) {
			List<FunctionArgument> args = new List<FunctionArgument>();

			Record exte = null;

			foreach (ArgumentMetadataGenerator i in this.Args) {
				var arg = i.EvalArgumnet(e);
				/*	if (arg.name == "this" && arg.type != null && arg.type is StandartLibrary.KType) {
						exte = (StandartLibrary.KType)arg.type;
						continue;
					}*/
				args.Add(arg);
			}

			List<string> s = new List<string>() { "self", "_", "this", "base", "value", "kwargs", "args" };

			BlockE expre = new BlockE();

			foreach (var i in args) {
				string mutname = i.name.Replace("*", "");
				if (mutname == "this")
					throw new Lumen.Lang.Std.Exception("Параметр функции не может иметь имя this", stack: e);
				s.Add(mutname);
			}

			/*	if (this.otherContacts != null) {
					foreach (Expression i in this.otherContacts) {
						expre.Add(new ConditionE(i, new UnknownExpression(), new RaiseE(null, new ValueE("контракт " + i.ToString() + " не выполнен"), "", -1)));
					}
				}*/

			if (this.returnedType != null || expre.expressions.Count > 0) {
				expre.Add(Body);
			}

			if (returnedType != null || expre.expressions.Count > 0)
				Body = expre;

			List<String> visible = new List<String> { "this", "self", "null", "true", "false", "args", "kwargs", "_" };
			visible.AddRange(this.Args.Select(i => i.name.Replace("*", "")));

			UserFun v = new UserFun {
				Arguments = args,
				condition = otherContacts.Count > 0 ? otherContacts[0] : null,
				body = this.Body?.Closure(visible, e)
			};
			v.Set("name", (KString)NameFunction, AccessModifiers.PRIVATE, e);

			if (exte != null) {
				exte.SetAttribute(NameFunction, v);
			}
			else {
				if (this.Body == null) {
					e.Set(this.NameFunction, v);
				}
				else if (e.ExistsInThisScope(NameFunction) && e[NameFunction] is UserFun uf) {
					uf.body = v.body;
				}
				else
					e.Set(this.NameFunction, v);
			}

			return v;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(NameFunction);
			return new FunctionDefineStatement(NameFunction, Args.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), Body.Closure(visible, thread), returnedType?.Closure(visible, thread));
		}
	}
}