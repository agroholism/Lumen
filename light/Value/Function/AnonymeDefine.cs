using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class AnonymeDefine : Expression {
		public List<ArgumentMetadataGenerator> Args;
		public Expression Body;
		public Expression def;
		public Expression returnedType;
		private List<Expression> otherContacts;

		public AnonymeDefine(List<ArgumentMetadataGenerator> Args, Expression Body) {
			this.Args = Args;
			this.Body = Body;
			this.returnedType = null;
		}

		public AnonymeDefine(List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType) {
			this.Args = Args;
			this.Body = Body;
			this.returnedType = returnedType;
		}

		public AnonymeDefine(List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType, List<Expression> otherContacts) : this(Args, Body, returnedType) {
			this.otherContacts = otherContacts;
			this.def = def;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			List<String> new_visible = new List<string>();

			foreach(String i in visible) {
				new_visible.Add(i);
			}

			foreach(ArgumentMetadataGenerator i in Args) {
				new_visible.Add(i.name);
			}

			return new AnonymeDefine(Args.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), Body.Closure(new_visible, thread), returnedType?.Closure(visible, thread));
		}

		public Value Eval(Scope e) {
			List<FunctionArgument> args = new List<FunctionArgument>();

			foreach (ArgumentMetadataGenerator i in this.Args) {
				args.Add(i.EvalArgumnet(e));
			}

			List<String> s = new List<String>() { "self", "_", "this", "value" };

			BlockE expre = new BlockE();

			foreach (FunctionArgument i in args) {
				String mutname = i.name.Replace("*", "");
				s.Add(mutname);
			}

			if (def != null)
				expre.Add(def);

			if (this.returnedType != null || expre.expressions.Count > 0) {
				expre.Add(this.Body);
			}

			if (returnedType != null || expre.expressions.Count > 0)
				Body = expre;

			UserFun v = new UserFun {
				Arguments = args,
				body = Body.Closure(s, e),
				condition = otherContacts != null ? (otherContacts.Count > 0 ? otherContacts[0] : null) : null
			};
			v.Attributes["name"] = (KString)"[lambda]";
			return v;
		}

		public Expression Optimize(Scope scope) {
			return new AnonymeDefine(this.Args, this.Body.Optimize(scope), this.returnedType, this.otherContacts);
		}

		public override String ToString() {
			String result = "(";

			foreach (ArgumentMetadataGenerator i in this.Args) {
				result += i.name + (i.type == null ? "" : ": " + i.type) + (i.defaultValue == null ? "" : " = " + i.defaultValue) + ", ";
			}

			return (result.Length > 2 ? result.Substring(0, result.Length - 2) : result) + ") => " + Body.ToString();
		}
	}
}