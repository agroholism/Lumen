using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal class StreamModule : Module {
        public StreamModule() {
            this.Name = "prelude.Stream";

			this.SetMember("toStream", new LambdaFun((e, args) => {
				return new Stream(e["this"].ToList(e));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this")
				}
			});

			this.IncludeMixin(Prelude.Collection);
        }
    }
}
