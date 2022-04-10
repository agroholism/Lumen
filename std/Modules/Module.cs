using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class ModuleModule : Module {
		internal ModuleModule() {
			this.Name = "Module";

			this.SetMember("getImplementations", new LambdaFun((e, args) => {
				Module module = e["self"] as Module;
				return new List(module.Mixins);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"),
				},
			});

			this.SetMember("getNames", new LambdaFun((e, args) => {
				Module module = e["self"] as Module;
				return new List(module.Members.Keys.Select(x => new Text(x)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"),
				},
			});

		}
	}
}
