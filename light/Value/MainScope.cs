using System;
using System.Collections.Generic;
using System.IO;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	public class MainScope : Scope {
		public static List<String> CONSTANTS = new List<String> { "Kernel" };
		public static Lumen.Lang.Std.StandartModule Kernel;

		public MainScope() : base(null) {
			Kernel = global::Lumen.Lang.Std.StandartModule.__Kernel__;

			AddUsing(Kernel);
			Set("Kernel", Kernel);

			Kernel.Set("require", new LambdaFun((e, args) => {
				String path = /*IK.path + "\\" + */args[0].ToString(e);

				path += System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + path + ".dll") || System.IO.File.Exists(path + ".dll") ? ".dll" : ".ks";
				
				// make
				if (path.EndsWith(".dll")) {
					if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + path)) {
						System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + path);
						ass.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e.parent, "" });
						return Const.VOID;
					}
					System.Reflection.Assembly a = System.Reflection.Assembly.Load(path);
					a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
					return Const.VOID;
				}

				String fullPath = new FileInfo(path).FullName;

				if (global::Lumen.Lang.Std.StandartModule.LoadedModules.ContainsKey(fullPath)) {
					foreach (KeyValuePair<String, Value> i in global::Lumen.Lang.Std.StandartModule.LoadedModules[fullPath]) {
						e.parent.Set(i.Key, i.Value);
					}
				}

				// TODO
				Scope x = new Scope(e.parent);
				x.AddUsing(Kernel);
				Parser p = new Parser(new Lexer(System.IO.File.ReadAllText(path), "").Tokenization(), "");
				p.Parsing(x);

			/*	if (x.IsExsists("this")) {
					new Applicate(new ValueE(x["this"]), expressions).Eval(x);
				}*/

				Dictionary<String, Value> included = new Dictionary<String, Value>();
				foreach (KeyValuePair<String, Value> i in x.variables) {
					included.Add(i.Key, i.Value);
					e.parent.Set(i.Key, i.Value);
				}

				global::Lumen.Lang.Std.StandartModule.LoadedModules[fullPath] = included;

				return Const.VOID;
			}, "Kernel.require"));
		}
	}
}
