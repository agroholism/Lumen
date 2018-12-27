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

			Record extendableRecord = null;

			foreach (ArgumentMetadataGenerator i in this.Args) {
				FunctionArgument arg = i.EvalArgumnet(e);

				// It's extension?
				if (arg.name == "this"
					&& arg.Attributes != null
					&& arg.Attributes.TryGetValue("type", out var value)
					&& value is Record record) {
					extendableRecord = record;
					continue;
				}

				args.Add(arg);
			}

			List<String> notClosurableVariables = new List<String> { "self", "_", "this", "base", "value", "kwargs", "args" };

			foreach (FunctionArgument i in args) {
				String mutname = i.name.Replace("*", "");

				if (mutname == "this") {
					throw new Lumen.Lang.Std.Exception("Параметр функции не может иметь имя this", stack: e);
				}

				notClosurableVariables.Add(mutname);
			}

			UserFun v = new UserFun {
				Arguments = args,
				condition = this.otherContacts.Count > 0 ? this.otherContacts[0] : null,
				body = this.Body?.Closure(notClosurableVariables, e)
			};


			v.Set("name", (KString)this.NameFunction, AccessModifiers.PRIVATE, e);

			if(this.returnedType != null) {
				v.Set("type", this.returnedType.Eval(e), AccessModifiers.PRIVATE, e);
			}


			if (extendableRecord != null) {
				extendableRecord.SetAttribute(this.NameFunction, v);
			}
			else {
				if (e.ExistsInThisScope(NameFunction) && e[NameFunction] is UserFun uf) {
					uf.body = v.body;
				}
				else {
					e.Set(this.NameFunction, v);
				}
			}

			return v;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(this.NameFunction);
			return new FunctionDefineStatement(this.NameFunction, this.Args.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), Body.Closure(visible, thread), returnedType?.Closure(visible, thread));
		}
	}
}