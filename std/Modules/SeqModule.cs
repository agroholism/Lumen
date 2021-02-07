using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class SeqModule : Type {
		public SeqModule() : base("Seq") {
			this.SetMember("empty", new LambdaFun((scope, args) => {
				return new Seq(Enumerable.Empty<Value>());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("wrap", new LambdaFun((scope, args) => {
				return new Seq(Enumerable.Repeat(scope["init"], 1));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("replicate", new LambdaFun((scope, args) => {
				return new Seq(Enumerable.Repeat(scope["init"], scope["len"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("len")
				}
			});

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

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Name = "fromSeq",
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Container);
		}
	}

	internal class RangeModule : Type {
		public RangeModule() : base("Range") {
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
