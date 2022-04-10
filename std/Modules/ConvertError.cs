using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	interface IExceptionConstructor {
		public LumenException MakeExceptionInstance(params Value[] values);
	}

	public abstract class ErrorModule : Module, IExceptionConstructor {
		public ExceptionConstructor constructor;

		public ErrorModule() {
			this.AppendImplementation(Prelude.Exception);
		}

		public LumenException MakeExceptionInstance(params Value[] values) {
			return this.constructor.MakeExceptionInstance(values);
		}
	}

	public sealed class Error : ErrorModule {
		internal Error() : base() {
			this.Name = "Error";

			this.constructor = new ExceptionConstructor("Error", this, "message");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance(e["message"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("message")
				}
			});

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return self.GetField("message");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class InvalidOperation : ErrorModule {
		internal InvalidOperation() : base() {
			this.Name = "InvalidOperation";

			this.constructor = new ExceptionConstructor("InvalidOperation", this, "message");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance(e["message"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("message")
				}
			});

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return self.GetField("message");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class CollectionIsEmpty : ErrorModule {
		internal CollectionIsEmpty() : base() {
			this.Name = "CollectionIsEmpty";

			this.constructor = new ExceptionConstructor("CollectionIsEmpty", this);

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance();
			}) {
				Parameters = new List<IPattern>()
			});

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				return new Text("required not empty collection");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class AssertError : ErrorModule {
		internal AssertError() : base() {
			this.Name = "AssertError";

			this.constructor = new ExceptionConstructor("AssertError", this, "message");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance();
			}));

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return self.GetField("message");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class ConvertError : ErrorModule {
		internal ConvertError() : base() {
			this.Name = "ConvertError";

			this.constructor = new ExceptionConstructor("ConvertError", this, "fromType", "targetType");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance(e["fromType"], e["targetType"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fromType"),
					new NamePattern("targetType")
				}
			});

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return new Text(
					Exceptions.CONVERT_ERROR.F(self.GetField("fromType"), self.GetField("targetType")));
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class InvalidArgument : ErrorModule {
		internal InvalidArgument() : base() {
			this.Name = "InvalidArgument";

			this.constructor = new ExceptionConstructor("InvalidArgument", this, "name", "message");

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return this.MakeExceptionInstance(scope["name"], scope["message"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("name"),
					new NamePattern("message")
				}
			});

			this.SetMember("getMessage", new LambdaFun((scope, args) => {
				Instance self = scope["self"] as Instance;

				return new Text($"[{self.GetField("name")}] {self.GetField("message")}");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class FunctionIsNotImplemented : ErrorModule {
		internal FunctionIsNotImplemented() : base() {
			this.Name = "FunctionIsNotImplemented";

			this.constructor =
			   new ExceptionConstructor("FunctionIsNotImplemented", this, "typeObject", "functionName");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance(e["typeObject"], e["functionName"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("typeObject"),
					new NamePattern("functionName")
				}
			});

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return new Text(Exceptions.FUNCTION_IS_NOT_IMPLEMENTED_FOR_TYPE.F(self.GetField("functionName"), self.GetField("typeObject")));
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class IndexOutOfRange : ErrorModule {
		internal IndexOutOfRange() : base() {
			this.Name = "IndexOutOfRange";

			this.constructor = new ExceptionConstructor("IndexOutOfRange", this);

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return this.MakeExceptionInstance();
			}) {
				Parameters = new List<IPattern> {

				}
			});

			this.SetMember("getMessage", new LambdaFun((scope, args) => {
				return new Text(Exceptions.INDEX_OUT_OF_RANGE);
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class PrivacyError : ErrorModule {
		internal PrivacyError() : base() {
			this.Name = "PrivacyError";

			this.constructor = new ExceptionConstructor("PrivacyError", this, "message");

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return this.MakeExceptionInstance(scope["message"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("message"),
				}
			});

			this.SetMember("getMessage", new LambdaFun((scope, args) => {
				return new Text((scope["self"] as Instance).GetField("message").ToString());
			}) {
				Parameters = Const.Self
			});
		}
	}
}
