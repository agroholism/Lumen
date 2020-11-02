namespace Lumen.Lang {
	internal class ExceptionClass : Module {
		internal ExceptionClass() {
			this.Name = "Exception";

			this.SetMember("message", new LambdaFun((scope, args) => {
				return new Text($"exception raised");
			}) {
				Arguments = Const.Self
			});
		}
	}
}
