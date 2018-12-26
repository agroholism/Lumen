using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Lumen.Lang.Std {
	public sealed class StandartModule : Module {
		#region Fields
		public static KType Vector { get; } = new RVec();
		public static KType Function { get; } = new FunctionClass();
		public static KType Number { get; } = new NumberType();
		public static KType Expando { get; } = new ExpandoType();
		public static KType Map { get; } = new MapClass();
		public static KType Null { get; } = new NullClass();
		public static KType String { get; } = new StringClass();
		public static KType Enumerator { get; } = new EnumeratorType();
		public static KType File { get; } = new FileType();

		public static KType Exception { get; } = new ExceptionType();

		public static KType Boolean { get; } = new RBoolean();
		public static KType DateTime { get; } = new DateTimeClass();
		public static KType _Type { get; } = new TypeType();

		public static StandartModule __Kernel__ { get; } = new StandartModule();

		#endregion

		public static Dictionary<String, Dictionary<String, Value>> LoadedModules { get; } = new Dictionary<String, Dictionary<String, Value>>();

		private StandartModule() {
			Set("std", this);

			Set("vec", Vector);
			Set("seq", Enumerator);
			Set("num", Number);
			Set("bool", Boolean);
			Set("str", String);
			Set("fun", Function);
			Set("exc", Exception);

			Set("type", _Type);
			Set("map", Map);

			Set("true", Const.TRUE);
			Set("false", Const.FALSE);
			Set("null", Const.NULL);

			Set("PI", (Num)Math.PI);
			Set("E", (Num)Math.E);

			Set("NL", (KString)Environment.NewLine);
			Set("EL", (KString)System.String.Empty);

			Set("print", new LambdaFun((scope, args) => {
				String separator = scope["sep"].ToString(scope);
				String onend = scope["onend"].ToString(scope);

				var temp = scope["params"];

				String result = System.String.Join(separator, temp.ToIterator(scope).Select(i => 
					i.ToString(scope))) + (onend);

				Value file = scope["file"];

				if (file is Null) {
					Console.Write(result);
				}
				else if (file is KString) {
					Boolean isWriteMode = Converter.ToBoolean(scope["write?"]);

					// add encoding

					try {
						if (isWriteMode) {
							System.IO.File.WriteAllText(file.ToString(), result, Encoding.Default);
						}
						else {
							System.IO.File.AppendAllText(file.ToString(), result, Encoding.Default);
						}
					}
					catch { } // 
				}


				return Const.NULL;
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("*params"),
						new FunctionArgument("sep", (KString)" "),
						new FunctionArgument("onend", (KString)Environment.NewLine),
						new FunctionArgument("file", Const.NULL),
						new FunctionArgument("write?", Const.TRUE)
					}
			});
			Set("input", new LambdaFun((scope, args) => {
				String prompt = scope["message"].ToString(scope);
				if (prompt != "") {
					Console.Write(prompt);
				}
				return (KString)Console.ReadLine();
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("message", (KString)"") }
			});
		}

		public static Value Call(Value callable, Scope scope, params Value[] args) {
			if (callable is Fun f) {
				return f.Run(scope, args);
			}

			if (callable.Type.AttributeExists("()")) {
				Scope s = new Scope(scope);
				s.This = callable;
				return Call(callable.Type.GetAttribute("()", scope), s, args);
			}

			return null;
		}
	}
}
