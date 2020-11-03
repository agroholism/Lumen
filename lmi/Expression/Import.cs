using System;
using System.Collections.Generic;
using System.IO;

using Lumen.Lang;
using Lumen.Lang.Expressions;
using Module = Lumen.Lang.Module;

namespace Lumen.Lmi {
	public class Import : Expression {
		// from
		private readonly String path;
		private readonly Int32 line;
		private readonly String fileName;
		private Boolean isFrom;
		// is imported all entities (only when isFrom)
		private Boolean importAll;
		// is imported some entities (only when isFrom)
		private List<String> entities;

		public Import(String path, Boolean isFrom, Boolean importAll, List<String> entities, String fileName, Int32 line) {
			this.path = Directory.GetParent(fileName) + "\\" + path;
			this.isFrom = isFrom;
			this.importAll = importAll;
			this.entities = entities;
			this.fileName = fileName;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			Tuple<Module, Module> module = this.ImportFromPath(scope);
	
			if(this.isFrom) {
				if(this.importAll) {
					foreach(KeyValuePair<String, Value> i in module.Item1.Members) {
						scope.Bind(i.Key, i.Value);
					}
				} else {
					foreach (String i in this.entities) {
						scope.Bind(i, module.Item1.Members[i]);
					}
				}
			} else {
				scope.Bind(module.Item2.Name, module.Item2);
			}

			return Const.UNIT;

			if (!Directory.Exists(this.path)) {
				String fullPath = new FileInfo(this.path + ".lm").FullName;

				List<String> diffbr = this.GetModulePath(Path.GetDirectoryName(fullPath));
				List<String> diffbc = this.GetModulePath(Directory.GetCurrentDirectory());

				// if this module is already imported
				if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
					if (this.isFrom) {
						// get required module
						Module m = this.GetRequiredModule(Prelude.GlobalImportCache[fullPath], diffbc, scope);

						foreach (String i in this.entities) {
							scope.Bind(i, m.GetMember(i, scope));
						}
					}
					else {
						scope.Bind(diffbc[diffbr.Count - 1], this.GetRequiredModule(Prelude.GlobalImportCache[fullPath], diffbc, scope));
					}

					return Const.UNIT;
				}

				if (!File.Exists(fullPath)) {
					throw new LumenException(Exceptions.MODULE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
				}

				Module lowLevelModule = this.CreateModules(diffbr, scope, out Module topLevelModule);
				this.ImportFileIntoModule(scope, fullPath, lowLevelModule);
				Prelude.GlobalImportCache[fullPath] = topLevelModule;

				if (this.isFrom) {
					if (this.importAll) {
						foreach (KeyValuePair<String, Value> i in lowLevelModule.Members) {
							scope.Bind(i.Key, i.Value);
						}
					}
					else {
						foreach (String i in this.entities) {
							scope.Bind(i, lowLevelModule.GetMember(i, scope));
						}
					}
				}
				else {
					scope.Bind(diffbc[diffbc.Count - 1], topLevelModule);
				}
				/*if (Directory.Exists(this.path)) {
					if (this.importAll) {
						foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
							this.ImportFile(scope, i, false);
						}
					}
					else {
						foreach (String i in this.entities) {
							Boolean isDllExtension = File.Exists(this.path + "\\" + i + ".dll");
							this.ImportFile(scope, this.path + "\\" + i + (isDllExtension ? ".dll" : ".lm"), false);
						}
					}
				}
				else if (File.Exists(this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"))) {
					Boolean isDllExtension = File.Exists(this.path + ".dll");
					this.ImportFile(scope, this.path + (isDllExtension ? ".dll" : ".lm"), true);
				}
				else {
					throw new LumenException(Exceptions.PACKAGE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
				}*/

				/*if (Directory.Exists(this.path)) {
					foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
						this.ImportFile(scope, i, false);
					}
				}
				else {
					this.ImportFile(scope, this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"), false);
				}*/
			}
			return Const.UNIT;
		}

		private Tuple<Module, Module> ImportFromPath(Scope scope) {
			// Is not directory
			if (!Directory.Exists(this.path)) {
				// Is this a lumen file?
				Boolean isLumenFile = File.Exists(new FileInfo(this.path + ".lm").FullName);

				if (isLumenFile) {
					return this.ImportLumenFile(new FileInfo(this.path + ".lm").FullName, scope);
				}

				if (!File.Exists(new FileInfo(this.path + ".dll").FullName)) {
					throw new LumenException(Exceptions.MODULE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
				}

				return this.ImportDllFile(new FileInfo(this.path + ".dll").FullName);

				String fullPath = new FileInfo(this.path + ".lm").FullName;

				List<String> diffbr = this.GetModulePath(Path.GetDirectoryName(fullPath));
				List<String> diffbc = this.GetModulePath(Directory.GetCurrentDirectory());

				// if this module is already imported
				/*if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
					if (this.isFrom) {
						// get required module
						Module m = this.GetRequiredModule(Prelude.GlobalImportCache[fullPath], diffbc, scope);

						foreach (String i in this.entities) {
							scope.Bind(i, m.GetMember(i, scope));
						}
					}
					else {
						scope.Bind(diffbc[diffbr.Count - 1], this.GetRequiredModule(Prelude.GlobalImportCache[fullPath], diffbc, scope));
					}

					return Const.UNIT;
				}*/

				/*	if (!File.Exists(fullPath)) {
						throw new LumenException(Exceptions.MODULE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
					}

					Module lowLevelModule = this.CreateModules(diffbr, scope, out Module topLevelModule);
					this.ImportFileIntoModule(scope, fullPath, lowLevelModule);
					Prelude.GlobalImportCache[fullPath] = topLevelModule;

					if (this.isFrom) {
						if (importAll) {
							foreach (var i in lowLevelModule.Members) {
								scope.Bind(i.Key, i.Value);
							}
						}
						else {
							foreach (String i in this.entities) {
								scope.Bind(i, lowLevelModule.GetMember(i, scope));
							}
						}
					}
					else {
						scope.Bind(diffbc[diffbc.Count - 1], topLevelModule);
					}*/
				/*if (Directory.Exists(this.path)) {
					if (this.importAll) {
						foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
							this.ImportFile(scope, i, false);
						}
					}
					else {
						foreach (String i in this.entities) {
							Boolean isDllExtension = File.Exists(this.path + "\\" + i + ".dll");
							this.ImportFile(scope, this.path + "\\" + i + (isDllExtension ? ".dll" : ".lm"), false);
						}
					}
				}
				else if (File.Exists(this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"))) {
					Boolean isDllExtension = File.Exists(this.path + ".dll");
					this.ImportFile(scope, this.path + (isDllExtension ? ".dll" : ".lm"), true);
				}
				else {
					throw new LumenException(Exceptions.PACKAGE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
				}*/

				/*if (Directory.Exists(this.path)) {
					foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
						this.ImportFile(scope, i, false);
					}
				}
				else {
					this.ImportFile(scope, this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"), false);
				}*/
			}

			return null;
		}

		private Tuple<Module, Module> ImportDllFile(String fullName) {
			throw new NotImplementedException();
		}

		private Tuple<Module, Module> ImportLumenFile(String fullPath, Scope scope) {
			List<String> diffbr = this.GetModulePath(Path.GetDirectoryName(fullPath));
			List<String> diffbc = this.GetModulePath(Directory.GetCurrentDirectory());
			diffbr.Add(Path.GetFileNameWithoutExtension(fullPath));
			// if this module is already imported
			if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
				return new Tuple<Module, Module>(this.GetRequiredModule(Prelude.GlobalImportCache[fullPath], diffbc, scope), Prelude.GlobalImportCache[fullPath]);
			}
		
			Module lowLevelModule = this.CreateModules(diffbr, scope, out Module topLevelModule);
			this.ImportFileIntoModule(scope, fullPath, lowLevelModule);
			Prelude.GlobalImportCache[fullPath] = topLevelModule;
		
			return new Tuple<Module, Module>(lowLevelModule, topLevelModule);
						/*if (Directory.Exists(this.path)) {
				if (this.importAll) {
					foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
						this.ImportFile(scope, i, false);
					}
				}
				else {
					foreach (String i in this.entities) {
						Boolean isDllExtension = File.Exists(this.path + "\\" + i + ".dll");
						this.ImportFile(scope, this.path + "\\" + i + (isDllExtension ? ".dll" : ".lm"), false);
					}
				}
			}
			else if (File.Exists(this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"))) {
				Boolean isDllExtension = File.Exists(this.path + ".dll");
				this.ImportFile(scope, this.path + (isDllExtension ? ".dll" : ".lm"), true);
			}
			else {
				throw new LumenException(Exceptions.PACKAGE_DOES_NOT_EXISTS.F(this.path), line: this.line, fileName: this.fileName);
			}*/

			/*if (Directory.Exists(this.path)) {
				foreach (String i in Directory.EnumerateFiles(this.path, "*.lm")) {
					this.ImportFile(scope, i, false);
				}
			}
			else {
				this.ImportFile(scope, this.path + (File.Exists(this.path + ".dll") ? ".dll" : ".lm"), false);
			}*/
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		private Module GetRequiredModule(Module topLevelModule, List<String> path, Scope scope) {
			for (Int32 i = 1; i < path.Count; i++) {
				topLevelModule = topLevelModule.GetMember(path[i], scope) as Module;
			}

			return topLevelModule;
		}

		private Module CreateModules(List<String> modulePath, Scope scope, out Module topLevelModule) {
			Module result = scope.ExistsInThisScope(modulePath[0]) ? scope[modulePath[0]] as Module : new Module(modulePath[0]);

			topLevelModule = result;

			for (Int32 i = 1; i < modulePath.Count; i++) {
				result.SetMember(modulePath[i], result.Contains(modulePath[i]) ? result.GetMember(modulePath[i], scope) as Module : new Module(modulePath[i]));
				result = result.GetMember(modulePath[i], scope) as Module;
			}

			return result;
		}

		/*private void ImportFile(Scope scope, String path, Boolean onlyEntites) {
			String fullPath = new FileInfo(path).FullName;

			if (!File.Exists(fullPath)) {
				throw new LumenException(Exceptions.MODULE_DOES_NOT_EXISTS.F(path), line: this.line, fileName: this.fileName);
			}

			if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
				foreach (KeyValuePair<String, Value> i in Prelude.GlobalImportCache[fullPath]) {
					if (onlyEntites && this.entities.Contains(i.Key)) {
						scope.Bind(i.Key, i.Value);
					}
					else if (!onlyEntites) {
						scope.Bind(i.Key, i.Value);
					}
				}

				return;
			}

			// make
			if (path.EndsWith(".dll")) {
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path)) {
					Assembly ass = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + path);
					ass.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { scope, "" });
					return;
				}
				Assembly a = Assembly.Load(path);
				a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { scope, "" });
				return;
			}

			Scope innerScope = new Scope(scope);
			innerScope.AddUsing(Prelude.Instance);
			Parser parser = new Parser(new Lexer(File.ReadAllText(fullPath), fullPath).Tokenization(), fullPath);
			List<Expression> expressions = parser.Parse();

			foreach (Expression expression in expressions) {
				expression.Eval(innerScope);
			}

			Dictionary<String, Value> included = new Dictionary<String, Value>();
			foreach (KeyValuePair<String, Value> i in innerScope.variables) {
				included.Add(i.Key, i.Value);
				if (onlyEntites && this.entities.Contains(i.Key)) {
					scope.Bind(i.Key, i.Value);
				}
				else if (!onlyEntites) {
					scope.Bind(i.Key, i.Value);
				}
			}

			Prelude.GlobalImportCache[fullPath] = included;
		}
		*/
		private void ImportFileIntoModule(Scope scope, String path, Module module) {
			Scope innerScope = new Scope(scope);
			innerScope.AddUsing(Prelude.Instance);
			Parser parser = new Parser(new Lexer(File.ReadAllText(path), path).Tokenization(), path);
			List<Expression> expressions = parser.Parse();

			foreach (Expression expression in expressions) {
				expression.Eval(innerScope);
			}

			foreach (KeyValuePair<String, Value> i in innerScope.variables) {
				module.SetMember(i.Key, i.Value);
			}
		}

		private List<String> GetModulePath(String requiredModulePath) {
			return this.GetPathDiff(Interpriter.BasePath, requiredModulePath);
		}

		private List<String> GetPathDiff(String currentModulePath, String requiredModulePath) {
			List<String> result = new List<String>();

			String[] bp = currentModulePath.Split('\\');
			String[] rp = requiredModulePath.Split('\\');

			for (Int32 i = bp.Length; i < rp.Length; i++) {
				result.Add(rp[i]);
			}

			return result;
		}

		public Expression Closure(ClosureManager manager) {
			Scope scope = new Scope();
			this.Eval(scope);

			foreach (KeyValuePair<String, Value> i in scope.variables) {
				manager.Declare(i.Key);
			}

			return this;
		}
	}
}