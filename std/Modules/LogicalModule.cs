namespace Lumen.Lang {
	internal sealed class LogicalModule : Module {
		internal LogicalModule() {
			this.Name = "Logical";

			#region operators

			this.SetMember("compare", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Number)scope["self"].CompareTo(other);
			}) {
				Arguments = Const.SelfOther
			});

			#endregion

			this.SetMember("toText", new LambdaFun((e, args) => new Text(e["self"].ToString())) {
				Arguments = Const.Self
			});

			this.SetMember("toNumber", new LambdaFun((e, args) => (Number)(Converter.ToBoolean(e["self"]) ? 1 : 0)) {
				Arguments = Const.Self
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Arguments = Const.Self
			});

			this.AppendImplementation(Prelude.Ord);
			this.AppendImplementation(Prelude.Cloneable);
		}
	}
}
