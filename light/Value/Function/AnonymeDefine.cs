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
		private readonly List<Expression> otherContacts;

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
			this.def = this.def;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			List<String> new_visible = new List<String>();

			foreach(String i in visible) {
				new_visible.Add(i);
			}

			foreach(ArgumentMetadataGenerator i in this.Args) {
				new_visible.Add(i.name);
			}

			return new AnonymeDefine(this.Args.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), this.Body.Closure(new_visible, thread), this.returnedType?.Closure(visible, thread));
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

			if (this.def != null) {
				expre.Add(this.def);
			}

			if (this.returnedType != null || expre.expressions.Count > 0) {
				expre.Add(this.Body);
			}

			if (this.returnedType != null || expre.expressions.Count > 0) {
				this.Body = expre;
			}

			UserFun v = new UserFun {
				Arguments = args,
				body = this.Body.Closure(s, e),
				condition = this.otherContacts != null ? (this.otherContacts.Count > 0 ? this.otherContacts[0] : null) : null
			};
			v.Attributes["name"] = (Str)"[lambda]";
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

			return (result.Length > 2 ? result.Substring(0, result.Length - 2) : result) + ") => " + this.Body.ToString();
		}
	}
}