using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class StreamModule : Module {
		public StreamModule() {
			this.Name = "Stream";

			this.SetMember("toStream", new LambdaFun((e, args) => {
				return new Stream(e["this"].ToList(e));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this")
				}
			});

			this.AppendImplementation(Prelude.Collection);
		}
	}
}
