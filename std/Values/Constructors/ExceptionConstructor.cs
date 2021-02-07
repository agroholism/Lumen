using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class ExceptionConstructor : Constructor, IExceptionConstructor, Fun {
		public ExceptionConstructor(String name, Type parent, Dictionary<String, List<IType>> fields)
			: base(name, parent, fields) {
		}

		public ExceptionConstructor(String name, Type parent, params String[] fields)
			:base(name, parent, fields) {
		}

		public LumenException MakeExceptionInstance(params Value[] values) {
			Value instance = this.MakeInstance(values);

			String message = null;

			if(this.Parameters.Count == 0) {
				message = "exception raised";
			}
			else if (this.TryGetMember("getMessage", out Value messageGenerator)
				&& messageGenerator.TryConvertToFunction(out Fun reallyMessageGenerator)) {
				message = reallyMessageGenerator.Call(new Scope(), instance).ToString();
			}

			LumenException result = new LumenException(this, message);

			if (this.TryGetMember("getNote", out Value noteGenerator)
				&& noteGenerator.TryConvertToFunction(out Fun reallyNoteGenerator)) {
				result.Note = reallyNoteGenerator.Call(new Scope(), result).ToString();
			}

			if (this.TryGetMember("getHelpLink", out Value helpLinkGenerator)
				&& helpLinkGenerator.TryConvertToFunction(out Fun reallyHelpLinkGenerator)) {
				result.HelpLink = reallyHelpLinkGenerator.Call(new Scope(), result).ToString();
			}

			return result;
		}

		public override Value Call(Scope e, params Value[] arguments) {
			if (this.Parameters.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			return this.MakeExceptionInstance(arguments);
		}
	}
}
