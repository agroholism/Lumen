using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public sealed class Prelude : Module {
		#region Fields
		public static Module Cloneable { get; } = new Cloneable();
		public static Module Exception { get; } = new ExceptionClass();
		public static Module Functor { get; } = new Functor();
		public static Module Applicative { get; } = new Applicative();
		public static Module Collection { get; } = new Collection();
		public static Module Ord { get; } = new OrdModule();
		public static Module Format { get; } = new Format();
		public static Module Context { get; } = new Context();

		public static FunctionIsNotImplemented FunctionIsNotImplemented { get; } = new FunctionIsNotImplemented();
		public static AssertError AssertError { get; } = new AssertError();
		public static ConvertError ConvertError { get; } = new ConvertError();
		public static Error Error { get; } = new Error();

		public static Module Any { get; } = new AnyModule();

		public static Module Unit { get; } = new UnitModule();

		public static Module Iterator { get; } = new IteratorModule();
		public static Module Ref { get; } = new RefModule();
		public static Module Stream { get; } = new StreamModule();
		public static Module Array { get; } = new ArrayModule();
		public static Module Function { get; } = new FunctionModule();
		public static Module Number { get; } = new NumberModule();
		public static Module Map { get; } = new MapModule();
		public static Module Pair { get; } = new MapModule.PairModule();
		public static Module Text { get; } = new TextModule();
		public static Module List { get; } = new ListModule();

		public static Module Logical { get; } = new LogicalModule();

		public static IType Fail { get; private set; }
		public static Module Option { get; } = new OptionModule();
		public static IType None { get; } = (Option as OptionModule).None;
		public static Constructor Some { get; } = (Option as OptionModule).Some;

		public static Prelude Instance { get; } = new Prelude();

		#endregion

		public static Dictionary<String, Module> GlobalImportCache { get; } = new Dictionary<String, Module>();

		private Prelude() {
			ConstructFail();

			this.SetMember("Prelude", this);

			this.SetMember("Ord", Ord);
			this.SetMember("Format", Format);
			this.SetMember("Functor", Functor);
			this.SetMember("Applicative", Applicative);
			this.SetMember("Context", Context);
			this.SetMember("Cloneable", Cloneable);
			this.SetMember("Exception", Exception);

			this.SetMember("FunctionIsNotImplemented", FunctionIsNotImplemented);
			this.SetMember("AssertError", AssertError);
			this.SetMember("ConvertError", ConvertError);
			this.SetMember("Error", Error);

			this.SetMember("Fail", Fail);

			this.SetMember("Pair", MapModule.PairModule.ctor);

			this.SetMember("Unit", Unit);

			this.SetMember("Option", Option);
			this.SetMember("Some", (Option as OptionModule).Some);
			this.SetMember("None", (Option as OptionModule).None);

			this.SetMember("Iterator", Iterator);
			this.SetMember("Ref", Ref);
			this.SetMember("ref", Ref);
			this.SetMember("List", List);
			this.SetMember("Stream", Stream);
			this.SetMember("Array", Array);
			this.SetMember("Number", Number);
			this.SetMember("Logical", Logical);
			this.SetMember("Text", Text);
			this.SetMember("Function", Function);
			this.SetMember("Collection", Collection);

			this.SetMember("Map", Map);

			this.SetMember("true", Const.TRUE);
			this.SetMember("false", Const.FALSE);

			this.SetMember("inf", new Number(Double.PositiveInfinity));
			this.SetMember("nan", new Number(Double.NaN));

			this.SetMember("nl", new Text(Environment.NewLine));

			this.SetMember("pi", (Number)Math.PI);
			this.SetMember("e", (Number)Math.E);

			this.SetMember("writeFile", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();
				String text = scope["text"].ToString();

				try {
					File.WriteAllText(fileName, text);
					return Helper.CreateSome(new Text(fileName));
				}
				catch {
					return Prelude.None;
				}
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("text"),
					new NamePattern("fileName")
				}
			});

			this.SetMember("readFile", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();

				if (File.Exists(fileName)) {
					try {
						return Helper.CreateSome(new Text(File.ReadAllText(fileName)));
					}
					catch {
						return Prelude.None;
					}
				}

				return Prelude.None;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("readLines", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();

				if (File.Exists(fileName)) {
					try {
						return new List(LinkedList.Create(File.ReadAllLines(fileName).Select(i => new Text(i))));
					}
					catch {
						return new List(LinkedList.Empty);
					}
				}

				return new List(LinkedList.Empty);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("readArray", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();

				if (File.Exists(fileName)) {
					try {
						return new Array(File.ReadAllLines(fileName).Select(i => new Text(i) as Value).ToList());
					}
					catch {
						return new Array();
					}
				}

				return new Array();
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("createFile", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();
				try {
					File.Create(fileName).Close();
					return Helper.CreateSome(new Text(fileName));
				}
				catch {
					return Prelude.None;
				}
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			// 23/04
			this.SetMember("println", new LambdaFun((scope, args) => {
				Value x = scope["x"];

				Console.WriteLine(x.ToString());

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("print", new LambdaFun((scope, args) => {
				Value x = scope["x"];

				Console.Write(x.ToString());

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("read", new LambdaFun((scope, args) => {
				return new Text(Console.ReadLine());
			}));

			this.SetMember("readWith", new LambdaFun((scope, args) => {
				Console.Write(scope["prompt"]);
				return new Text(Console.ReadLine());
			}) {
				Arguments = new List<IPattern> {
				new NamePattern("prompt")
			}
			});

			this.SetMember("assert", new LambdaFun((scope, args) => {
				Assert(scope["condition"].ToBoolean(), scope);

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("condition")
				}
			});

			this.SetMember("functionIsNotImplementedForType", new LambdaFun((scope, args) => {
				FunctionIsNotImplementedForType(scope["fName"].ToString(), scope["t"], scope);
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fName"),
					new NamePattern("t")
				}
			});

			this.SetMember("parsen", new LambdaFun((scope, args) => {
				String str = scope["inputStr"].ToString();

				if (Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
					return Helper.CreateSome(new Number(result));
				}

				return Prelude.None;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("inputStr")
				}
			});
		}

		private static void ConstructFail() {
			Module FailModule = new Module("_") {
			};

			FailModule.SetMember("String", new LambdaFun((scope, args) => {
				IType obj = scope["this"] as IType;
				if (obj.TryGetMember("message", out Value result)) {
					return new Text($"Failed with message '{result}'");
				}

				throw new LumenException("failed in fail.tos");
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this")
				}
			}, null);

			Fail = Helper.CreateConstructor("prelude.Fail", FailModule, new List<String> { "message" });
		}

		public static Value DeconstructSome(Value some) {
			if (Some.IsParentOf(some)) {
				Instance someInstance = some as Instance;
				return someInstance.GetField("x");
			}

			return some;
		}

		public static void Assert(Boolean condition, Scope scope) {
			if (!condition) {
				throw AssertError.constructor.ToException(scope);
			}
		}

		public static void FunctionIsNotImplementedForType(String functionName, Value typeName, Scope scope) {
			throw FunctionIsNotImplemented.constructor.MakeInstance(typeName, new Text(functionName)).ToException(scope);

		}
	}
}
