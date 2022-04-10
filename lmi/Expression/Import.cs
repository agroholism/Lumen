#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lmi.Importing;
using Module = Lumen.Lang.Module;

namespace Lumen.Lmi {
	public class Import : Expression {
		private readonly List<String> importPath;
		private readonly List<(String name, String? alias)> namesToImport;
		private readonly String mainAlias;
		private readonly Int32 line;
		private readonly String fileName;

		private Boolean IsFrom { get => this.namesToImport.Count > 0; }

		private String FullImportPath { get => String.Join(".", this.importPath); }

		public Import(List<String> importPath, List<(String name, String? alias)> namesToImport, String mainAlias, String fileName, Int32 line) {
			this.importPath = importPath;
			this.namesToImport = namesToImport;
			this.fileName = fileName;
			this.line = line;
			this.mainAlias = mainAlias;
		}

		public Value Eval(Scope scope) {
			(Module root, Value result) = new OpaqueImport(this.line, this.fileName)
				.Import(Path.GetDirectoryName(this.fileName), this.importPath);

			if (this.IsFrom) {
				if (result is not Module module) {
					throw new LumenException($"can not import any names from not-module value \"{this.FullImportPath}\"", this.line, this.fileName);
				}

				List<String> privateNames = this.namesToImport
					.Where(pair => module.IsPrivate(pair.name))
					.Distinct()
					.Select(name => $"\"{name}\"")
					.ToList();

				if (privateNames.Count > 0) {
					throw new LumenException(Prelude.PrivacyError.constructor, $"can not import private name{(privateNames.Count > 1 ? "s" : "")} {String.Join(", ", privateNames)} from module \"{this.FullImportPath}\"", this.line, this.fileName);
				}

				foreach ((String name, String? alias) in this.namesToImport) {
					if (module.TryGetMember(name, out Value importedValue)) {
						String actualName = alias ?? name;
						scope.Bind(actualName, importedValue);
					} else {
						throw new LumenException($"can not find name \"{name}\" in module \"{this.FullImportPath}\"", this.line, this.fileName);
					}
				}

				return Const.UNIT;
			}

			if (this.mainAlias != null) {
				scope.Bind(this.mainAlias, result);
			} else {
				scope.Bind(this.importPath[0], root);
			}

			return Const.UNIT;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
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