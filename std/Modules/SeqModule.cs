using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class SeqModule : Module {
		public SeqModule() {
			this.Name = "Seq";

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Seq(Enumerable.Empty<Value>());
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("toSeq", new LambdaFun((e, args) => {
				return new Seq(e["this"].ToList(e));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("this")
				}
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Collection);
		}
	}

	internal class RangeModule : Module {
		public RangeModule() {
			this.Name = "Range";

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new InfinityRange();
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("hasEnd", new LambdaFun((e, args) => {
				var self = e["self"];

				if(self is InfinityRange) {
					return new Logical(false);
				}

				if(self is NumberRange numrabge) {
					return new Logical(numrabge.HasEnd);
				}

				throw new LumenException("hasEnd");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("hasStart", new LambdaFun((e, args) => {
				var self = e["self"];

				if (self is InfinityRange) {
					return new Logical(false);
				}

				if (self is NumberRange numrabge) {
					return new Logical(numrabge.HasStart);
				}

				throw new LumenException("hasStart");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("getStep", new LambdaFun((e, args) => {
				var self = e["self"];

				if (self is InfinityRange infRange) {
					return new Number(infRange.Step);
				}

				if (self is NumberRange numrabge) {
					return new Number(numrabge.Step);
				}

				throw new LumenException("hasStart");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("toSeq", new LambdaFun((e, args) => {
				return new Seq(e["self"].ToSeq(e));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Collection);
		}
	}
}
