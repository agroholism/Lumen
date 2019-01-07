using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class FunctionDefineDotStatement : Expression {
		internal String name;
		private List<ArgumentMetadataGenerator> arguments;
		private Expression body;
		private Expression returnedType;
		private List<Expression> contracts;
		private Int32 line;
		private String fileName;
		internal List<String> helper;

		public FunctionDefineDotStatement(String name, List<ArgumentMetadataGenerator> arguments, Expression body, Expression returnedType, List<Expression> contracts, Int32 line, String fileName, List<String> helper) {
			this.name = name;
			this.arguments = arguments;
			this.body = body;
			this.returnedType = returnedType;
			this.contracts = contracts;
			this.line = line;
			this.fileName = fileName;
			this.helper = helper;
		}

		public override String ToString() {
			String result = "let " + this.name + "(" + String.Join(", ", this.arguments.Select(i => i.name)) + ")" + "{" + this.body + "}";
			return result;
		}

		public Value Eval(Scope e) {
			List<FunctionArgument> args = new List<FunctionArgument>();

			foreach (ArgumentMetadataGenerator i in this.arguments) {
				FunctionArgument arg = i.EvalArgumnet(e);

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
				condition = this.contracts.Count > 0 ? this.contracts[0] : null,
				body = this.body?.Closure(notClosurableVariables, e)
			};


			v.Set("@name", (Str)(String.Join(".", this.helper) + "." + this.name), e);

			if (this.returnedType != null) {
				v.Set("type", this.returnedType.Eval(e), e);
			}

			Value x = e.Get(this.helper[0]);
			for (Int32 i = 1; i < this.helper.Count; i++) {
				if(x is IObject io) {
					x = io.Get(this.helper[i], e);
				}
			}

			if(x is IObject f) {
				f.Set(this.name, v, e);
			}

			return v;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(this.name);
			return new FunctionDefineDotStatement(this.name, this.arguments.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), this.body.Closure(visible, thread), this.returnedType?.Closure(visible, thread), this.contracts, this.line, this.fileName, this.helper);
		}

		public Expression Optimize(Scope scope) {
			throw new NotImplementedException();
		}
	}
}