using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal sealed class LogicalModule : Module {
		internal LogicalModule() {
			this.Name = "Logical";

			#region operators

			this.SetMember("compare", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Number)scope["self"].CompareTo(other);
			}) {
				Parameters = Const.SelfOther
			});

			#endregion

			this.SetMember("toText", new LambdaFun((e, args) => new Text(e["self"].ToString())) {
				Parameters = Const.Self
			});

			this.SetMember("toNumber", new LambdaFun((e, args) => (Number)(Converter.ToBoolean(e["self"]) ? 1 : 0)) {
				Parameters = Const.Self
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Parameters = Const.Self
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return Const.FALSE;
			}) {
				Parameters = new List<IPattern>()
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Ord);
			this.AppendImplementation(Prelude.Clone);
		}
	}
}
