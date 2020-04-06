using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	public class Import : Expression {
		private readonly String path;
		private readonly Int32 line;
		private readonly String fileName;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Import(String path, String fileName, Int32 line) {
			this.path = path;
			this.fileName = fileName;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			String path = this.path +
				(File.Exists(AppDomain.CurrentDomain.BaseDirectory + this.path + ".dll") || File.Exists(this.path + ".dll") ? ".dll" : ".lm");

			String fullPath = new FileInfo(path).FullName;

			/*if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
				foreach (KeyValuePair<String, Value> i in Prelude.GlobalImportCache[fullPath]) {
					scope.Bind(i.Key, i.Value);
				}

				return Const.UNIT;
			}
			*/
			// make
			if (path.EndsWith(".dll")) {
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path)) {
					Assembly ass = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + path);
					ass.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { scope, "" });
					return Const.UNIT;
				}
				Assembly a = Assembly.Load(path);
				a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { scope, "" });
				return Const.UNIT;
			}

			Scope innerScope = new Scope(scope);
			innerScope.AddUsing(Prelude.Instance);
			Parser parser = new Parser(new Lexer(File.ReadAllText(fullPath), this.fileName).Tokenization(), this.fileName);
			List<Expression> expressions = parser.Parsing();

			foreach(Expression expression in expressions) {
				expression.Eval(innerScope);
			}

			Dictionary<String, Value> included = new Dictionary<String, Value>();
			foreach (KeyValuePair<String, Value> i in innerScope.variables) {
				included.Add(i.Key, i.Value);
				scope.Bind(i.Key, i.Value);
			}

			//Prelude.GlobalImportCache[fullPath] = included;

			return Const.UNIT;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}
}