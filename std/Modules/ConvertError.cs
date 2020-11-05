using System.Collections.Generic;
using Lumen.Lang.Expressions;

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
				Arguments = new List<IPattern> {
					new NamePattern("message")
				}
			});

			this.SetMember("message", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return self.GetField("message");
			}) {
				Arguments = Const.Self
			});
		}
	}

	public sealed class AssertError : ErrorModule {
		internal AssertError() : base() {
			this.Name = "AssertError";

			this.constructor = new ExceptionConstructor("AssertError", this);

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.MakeExceptionInstance();
			}));

			this.SetMember("message", new LambdaFun((e, args) => {
				return new Text(Exceptions.ASSERT_IS_BROKEN);
			}) {
				Arguments = Const.Self
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
				Arguments = new List<IPattern> {
					new NamePattern("fromType"),
					new NamePattern("targetType")
				}
			});

			this.SetMember("message", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return new Text(
					Exceptions.CONVERT_ERROR.F(self.GetField("fromType"), self.GetField("targetType")));
			}) {
				Arguments = Const.Self
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
				Arguments = new List<IPattern> {
					new NamePattern("typeObject"),
					new NamePattern("functionName")
				}
			});

			this.SetMember("message", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return new Text(Exceptions.FUNCTION_IS_NOT_IMPLEMENTED_FOR_TYPE.F(self.GetField("functionName"), self.GetField("typeObject")));
			}) {
				Arguments = Const.Self
			});
		}
	}
}
