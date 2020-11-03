using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public sealed class Error : Module {
		public IConstructor constructor;

		internal Error() {
			this.Name = "Error";

			this.constructor = Helper.CreateConstructor("Error", this, "message");

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.constructor.MakeInstance(e["message"]);
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

			this.AppendImplementation(Prelude.Exception);
		}
	}

	public sealed class AssertError : Module {
		public IConstructor constructor;

		internal AssertError() {
			this.Name = "AssertError";

			this.constructor = Helper.CreateConstructor("AssertError", this);

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.constructor;
			}));

			this.SetMember("message", new LambdaFun((e, args) => {
				Instance self = e["self"] as Instance;

				return new Text(Exceptions.ASSERT_IS_BROKEN);
			}) {
				Arguments = Const.Self
			});

			this.AppendImplementation(Prelude.Exception);
		}
	}

	public sealed class ConvertError : Module {
		public IConstructor constructor;

		internal ConvertError() {
			this.Name = "ConvertError";

			this.constructor = Helper.CreateConstructor("ConvertError", this, new[] { "fromType", "targetType" });

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.constructor.MakeInstance(e["fromType"], e["targetType"]);
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

			this.AppendImplementation(Prelude.Exception);
		}
	}

	public sealed class FunctionIsNotImplemented : Module {
		public IConstructor constructor;

		internal FunctionIsNotImplemented() {
			this.Name = "FunctionIsNotImplemented";

			this.constructor =
			   Helper.CreateConstructor("FunctionIsNotImplemented", this, new[] { "typeObject", "functionName" });

			this.SetMember("<init>", new LambdaFun((e, args) => {
				return this.constructor.MakeInstance(e["typeObject"], e["functionName"]);
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

			this.AppendImplementation(Prelude.Exception);
		}
	}
}
