using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class Open : Expression {
		public String v;
		public String directoryName;
		public Int32 line;
		private readonly String fileName;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public Open(String v, String fileName, Int32 line) {
			this.v = v;
			this.fileName = fileName;
			this.line = line;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			String path = /*IK.path == null ? */this.v/* : IK.path + "\\" + v*/;

			path += System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + path + ".dll") || System.IO.File.Exists(path + ".dll") ? ".dll" : ".lm";

			// make
			if (path.EndsWith(".dll")) {
				if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + path)) {
					Assembly ass = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + path);
					ass.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
					return Const.VOID;
				}
				Assembly a = Assembly.Load(path);
				a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
				return Const.VOID;
			}

			String fullPath = new FileInfo(path).FullName;

			if (StandartModule.LoadedModules.ContainsKey(fullPath)) {
				foreach (KeyValuePair<String, Value> i in StandartModule.LoadedModules[fullPath]) {
					e.Set(i.Key, i.Value);
				}
			}

			// TODO
			Scope x = new Scope(e);
			x.AddUsing(StandartModule.__Kernel__);
			Parser p = new Parser(new Lexer(System.IO.File.ReadAllText(fullPath), this.fileName).Tokenization(), fileName);
			p.Parsing(x);

			/*	if (x.IsExsists("this")) {
					new Applicate(new ValueE(x["this"]), expressions).Eval(x);
				}*/

			Dictionary<String, Value> included = new Dictionary<String, Value>();
			foreach (KeyValuePair<String, Value> i in x.variables) {
				included.Add(i.Key, i.Value);
				e.Set(i.Key, i.Value);
			}

			StandartModule.LoadedModules[fullPath] = included;

			return Const.VOID;
		}
	}
}