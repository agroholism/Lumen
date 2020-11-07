namespace Lumen.Lang {
	internal class ExceptionClass : Module {
		internal ExceptionClass() {
			this.Name = "Exception";

			this.SetMember("message", new LambdaFun((scope, args) => {
				return new Text($"exception raised");
			}) {
				Parameters = Const.Self
			});

			this.SetMember("cause", new LambdaFun((scope, args) => {
				LumenException exception = scope["self"].ToException();

				return exception.Cause == null ? Prelude.None : (Value)Helper.CreateSome(exception.Cause);
			}) {
				Parameters = Const.Self
			});
		}
	}
}
