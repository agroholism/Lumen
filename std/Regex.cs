using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using NetRegex = System.Text.RegularExpressions.Regex;

namespace Lumen.Lang {
	internal class RegexModule : Module {
		public RegexModule() {
			this.Name = "Regex";

			this.SetMember("isMatch", new LambdaFun((scope, args) => {
				return new Bool(NetRegex.IsMatch(scope["input"].ToString(), scope["pattern"].ToString()));
			}));

			this.SetMember("replace", new LambdaFun((scope, args) => {
				return new Text(NetRegex.Replace(scope["input"].ToString(), scope["pattern"].ToString(), scope["replacement"].ToString()));
			}));
		}
	}
}
