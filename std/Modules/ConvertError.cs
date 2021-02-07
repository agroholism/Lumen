using System;
using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	interface IExceptionConstructor {
		public LumenException MakeExceptionInstance(params Value[] values);
	}

	public abstract class ErrorModule : Type, IExceptionConstructor {
		public ExceptionConstructor constructor;

		public ErrorModule(String name) : base(name) {
			this.AppendImplementation(Prelude.Exception);
		}

		public LumenException MakeExceptionInstance(params Value[] values) {
			return this.constructor.MakeExceptionInstance(values);
		}
	}

	public sealed class Error : ErrorModule {
		internal Error() : base("Error") {
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
		internal InvalidOperation() : base("InvalidOperation") {
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
		internal CollectionIsEmpty() : base("CollectionIsEmpty") {
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

	public sealed class AssertFailed : ErrorModule {
		internal AssertFailed() : base("AssertFailed") {
			this.constructor = new ExceptionConstructor("AssertFailed", this, "message");

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

		public LumenException CreateAssertFailed(String message) {
			return this.constructor.MakeExceptionInstance(new Text(message));
		}
	}

	public sealed class ConvertError : ErrorModule {
		internal ConvertError() : base("ConvertError") {
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
		internal InvalidArgument() : base("InvalidArgument") {
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

	public sealed class NotImplemented : Type {
		public ExceptionConstructor Requirement { get; private set; }
		public ExceptionConstructor Todo { get; private set; }

		internal NotImplemented() : base("NotImplemented") {
			this.Requirement =
			   new ExceptionConstructor("Requirement", this, "typeObject", "functionName");
			this.Todo =
			   new ExceptionConstructor("Todo", this, "reason");

			this.SetMember("getMessage", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				if (this.Requirement.IsParentOf(self)) {

					return new Text(Exceptions.FUNCTION_IS_NOT_IMPLEMENTED_FOR_TYPE.F(self.GetField("functionName"), self.GetField("typeObject")));
				}

				return self.GetField("reason");
			}) {
				Parameters = Const.Self
			});
		}
	}

	public sealed class IndexOutOfRange : ErrorModule {
		internal IndexOutOfRange() : base("IndexOutOfRange") {
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
}
