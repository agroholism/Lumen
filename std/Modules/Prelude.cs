using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public sealed class Prelude : Module {
		#region Fields
		public static Class Default { get; } = new Default();
		public static Class Clone { get; } = new Clone();
		public static Class Exception { get; } = new ExceptionClass();
		public static Class Monoid { get; } = new Monoid();
		public static Class Functor { get; } = new Functor();
		public static Class Applicative { get; } = new Applicative();
		public static Class Monad { get; } = new Monad();
		public static Class Collection { get; } = new Collection();
		public static Class Container { get; } = new Container();
		public static Class Ord { get; } = new OrdModule();
		public static Class Format { get; } = new Format();
		public static Class Context { get; } = new Context();

		public static ErrorModule IndexOutOfRange { get; } = new IndexOutOfRange();
		public static ErrorModule FunctionIsNotImplemented { get; } = new FunctionIsNotImplemented();
		public static ErrorModule AssertError { get; } = new AssertError();
		public static ErrorModule ConvertError { get; } = new ConvertError();
		public static ErrorModule CollectionIsEmpty { get; } = new CollectionIsEmpty();
		public static ErrorModule InvalidOperation { get; } = new InvalidOperation();
		public static ErrorModule InvalidArgument { get; } = new InvalidArgument();
		public static ErrorModule Error { get; } = new Error();

		public static Module Any { get; } = new AnyModule();

		public static Module Unit { get; } = new UnitModule();

		public static Module Iterator { get; } = new IteratorModule();
		public static Module Ref { get; } = new RefModule();
		public static Module Seq { get; } = new SeqModule();
		public static Module Range { get; } = new RangeModule();
		public static Module MutArray { get; } = new MutArrayModule();
		public static Module Function { get; } = new FunctionModule();
		public static Module Number { get; } = new NumberModule();
		public static Module MutMap { get; } = new MutMapModule();
		public static Module Pair { get; } = new MutMapModule.PairModule();
		public static Module Text { get; } = new TextModule();
		public static Module List { get; } = new ListModule();
		public static Module Future { get; } = new FutureModule();

		public static Module Logical { get; } = new LogicalModule();

		public static Module Option { get; } = new OptionModule();
		public static IType None { get; } = (Option as OptionModule).None;
		public static Constructor Some { get; } = (Option as OptionModule).Some;

		public static Module Result { get; } = new ResultModule();
		public static Constructor Success { get; } = (Result as ResultModule).Success;
		public static Constructor Failed { get; } = (Result as ResultModule).Failed;

		public static Prelude Instance { get; } = new Prelude();

		#endregion

		public static Dictionary<String, Module> GlobalImportCache { get; } = new Dictionary<String, Module>();

		private Prelude() {
			this.SetMember("Prelude", this);

			this.SetMember("Any", Any);

			this.SetMember("Ord", Ord);
			this.SetMember("Format", Format);
			this.SetMember("Monoid", Monoid);
			this.SetMember("Functor", Functor);
			this.SetMember("Applicative", Applicative);
			this.SetMember("Monad", Monad);
			this.SetMember("Context", Context);
			this.SetMember("Clone", Clone);
			this.SetMember("Default", Default);
			this.SetMember("Exception", Exception);

			this.SetMember("IndexOutOfRange", IndexOutOfRange);
			this.SetMember("CollectionIsEmpty", CollectionIsEmpty);
			this.SetMember("InvalidOperation", InvalidOperation);
			this.SetMember("InvalidArgument", InvalidArgument);
			this.SetMember("FunctionIsNotImplemented", FunctionIsNotImplemented);
			this.SetMember("AssertError", AssertError);
			this.SetMember("ConvertError", ConvertError);
			this.SetMember("Error", Error);

			this.SetMember("Unit", Unit);

			this.SetMember("Option", Option);
			this.SetMember("Some", Some);
			this.SetMember("None", None);

			this.SetMember("Result", Result);
			this.SetMember("Success", Success);
			this.SetMember("Failed", Failed);

			this.SetMember("Range", Range);
			this.SetMember("Iterator", Iterator);
			this.SetMember("Ref", Ref);
			this.SetMember("ref", Ref);
			this.SetMember("List", List);
			this.SetMember("Seq", Seq);
			this.SetMember("MutArray", MutArray);
			this.SetMember("Number", Number);
			this.SetMember("Logical", Logical);
			this.SetMember("Text", Text);
			this.SetMember("Function", Function);
			this.SetMember("Collection", Collection);

			this.SetMember("Future", Future);

			this.SetMember("MutMap", MutMap);

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
				Parameters = new List<IPattern> {
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
				Parameters = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("readFileAsync", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();

				StreamReader stream = File.OpenText(fileName);
				return new Future(stream.ReadToEndAsync().ContinueWith(task => {
					stream.Dispose();
					return (Value)new Text(task.Result);
				}));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("readFileLines", new LambdaFun((scope, args) => {
				String fileName = scope["fileName"].ToString();

				if (File.Exists(fileName)) {
					try {
						return new Seq(File.ReadLines(fileName).Select(i => new Text(i)));
					}
					catch {
						return Lang.Seq.Empty;
					}
				}

				return Lang.Seq.Empty;
			}) {
				Parameters = new List<IPattern> {
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
				Parameters = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			static Module GetModule(IType obj) {
				if (obj is Module m) {
					return m;
				}

				if (obj is Instance instance) {
					return (instance.Type as Constructor).Parent as Module;
				}

				if (obj is Constructor constructor) {
					return constructor.Parent;
				}

				if (obj is SingletonConstructor singleton) {
					return singleton.Parent;
				}

				return GetModule(obj.Type);
			}

			this.SetMember("typeof", new LambdaFun((scope, args) => {
				Value value = scope["value"];

				return GetModule(value.Type);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("value")
				}
			});

			// 23/04
			this.SetMember("println", new LambdaFun((scope, args) => {
				Value x = scope["x"];

				Console.WriteLine(x.ToString());

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("print", new LambdaFun((scope, args) => {
				Value x = scope["x"];

				Console.Write(x.ToString());

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
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
				Parameters = new List<IPattern> {
				new NamePattern("prompt")
			}
			});

			this.SetMember("functionIsNotImplementedForType", new LambdaFun((scope, args) => {
				FunctionIsNotImplementedForType(scope["fName"].ToString(), scope["t"]);
				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("t"),
					new NamePattern("fName")
				}
			});

			this.SetMember("parsen", new LambdaFun((scope, args) => {
				String str = scope["inputStr"].ToString();

				if (Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
					return Helper.CreateSome(new Number(result));
				}

				return Prelude.None;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("inputStr")
				}
			});

			//122520

			this.SetMember("listOf", new LambdaFun((scope, args) => {
				Value init = scope["init"];

				if(init == Const.UNIT) {
					return new List();
				}

				return new List(init.ToSeq(scope));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("seqOf", new LambdaFun((scope, args) => {
				Value init = scope["init"];

				if (init == Const.UNIT) {
					return new Seq(Enumerable.Empty<Value>());
				}

				return new Seq(init.ToSeq(scope));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("mutArrayOf", new LambdaFun((scope, args) => {
				Value init = scope["init"];

				if (init == Const.UNIT) {
					return new MutArray();
				}

				return new MutArray(init.ToSeq(scope));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("mutMapOf", new LambdaFun((scope, args) => {
				Value value = scope["init"];
				MutMap result = new MutMap();

				if (value == Const.UNIT) {
					return result;
				}

				foreach (Value i in value.ToSeq(scope)) {
					List<Value> stream = i.ToSeq(scope).Take(2).ToList();
					result.InternalValue[stream[0]] = stream[1];
				}

				return result;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});
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
				throw AssertError.constructor.ToException();
			}
		}

		public static void FunctionIsNotImplementedForType(String functionName, Value typeName) {
			throw FunctionIsNotImplemented.constructor.MakeExceptionInstance(typeName, new Text(functionName)).ToException();

		}
	}
}
