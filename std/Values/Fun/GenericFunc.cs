using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang.Std {
	[Serializable]
	public class GenericFunc : UserFun {
		internal Dictionary<String, Value> genericParams;
		internal Dictionary<List<Value>, Fun> memo = new Dictionary<List<Value>, Fun>();
		internal Expression exp;
		internal String name;

		public GenericFunc(Dictionary<String, Value> genericParams, Expression exp, String name) {
			this.genericParams = genericParams;
			this.exp = exp;
			this.name = name;
			StandartModule.Function.SetAttribute("[]", new LambdaFun((e, args) => {
				return MakeType(args.ToList(), e);
			}));
		}

		public Fun MakeType(List<Value> @params, Scope e) {
			foreach (KeyValuePair<List<Value>, Fun> i in this.memo) {
				Boolean match = true;

				for (Int32 z = 0; z < i.Key.Count; z++) {
					if (i.Key[z] != @params[z]) {
						match = false;
						break;
					}
				}

				if (match) {
					return i.Value;
				}
			}

			Scope scope = new Scope(e);

			Int32 index = 0;
			foreach (KeyValuePair<String, Value> i in this.genericParams) {
				if (index < @params.Count) {
					scope[i.Key] = @params[index];
				}
				else {
					scope[i.Key] = i.Value;
				}

				index++;
			}

			this.exp.Eval(scope);

			Fun re = scope[this.name] as Fun;
			re.Attributes["name"] = (KString)(this.name + $"[{String.Join(", ", @params)}]");
			this.memo[@params] = re;
			return re;
		}
	}
}
