using System;
using System.Collections.Generic;
using System.IO;

using StandartLibrary;

namespace Stereotype {
	[Serializable]
	public class MainScope : Scope {
		public static List<String> CONSTANTS = new List<String> { "Kernel" };
		public static StandartLibrary.StandartModule Kernel;

		public MainScope() : base(null) {
			Kernel = global::StandartLibrary.StandartModule.__Kernel__;

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
						return Const.NULL;
					}
					System.Reflection.Assembly a = System.Reflection.Assembly.Load(path);
					a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
					return Const.NULL;
				}

				String fullPath = new FileInfo(path).FullName;

				if (global::StandartLibrary.StandartModule.LoadedModules.ContainsKey(fullPath)) {
					foreach (KeyValuePair<String, Value> i in global::StandartLibrary.StandartModule.LoadedModules[fullPath]) {
						e.parent.Set(i.Key, i.Value);
					}
				}

				// TODO
				Scope x = new Scope(e.parent);
				x.AddUsing(Kernel);
				Parser p = new Parser(new Lexer(System.IO.File.ReadAllText(path), "").Tokenization());
				p.Parsing(x, fullPath);

			/*	if (x.IsExsists("this")) {
					new Applicate(new ValueE(x["this"]), expressions).Eval(x);
				}*/

				Dictionary<String, Value> included = new Dictionary<String, Value>();
				foreach (KeyValuePair<String, Value> i in x.variables) {
					included.Add(i.Key, i.Value);
					e.parent.Set(i.Key, i.Value);
				}

				global::StandartLibrary.StandartModule.LoadedModules[fullPath] = included;

				return Const.NULL;
			}, "Kernel.require"));
		}
	}
}
