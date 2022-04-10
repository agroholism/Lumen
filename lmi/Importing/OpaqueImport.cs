#nullable enable

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi.Importing {
	internal class OpaqueImport {
		private readonly String fileName;
		private readonly Int32 lineNo;

		public OpaqueImport(Int32 lineNo, String fileName) {
			this.lineNo = lineNo;
			this.fileName = fileName;
		}

		public (Module root, Value value) Import(String currentModuleDirectory, List<String> modulesPath) {
			if (modulesPath.Count == 0) {
				throw new LumenImportException("please specify at least one module to import", this.lineNo, this.fileName);
			}

			String? rootImportLocation =
				ModuleImportManager.FindRootImportLocation(currentModuleDirectory, modulesPath.First());

			if (rootImportLocation == null) {
				String message = modulesPath.Count == 1
					? $"can not find module \"{modulesPath.First()}\" to import"
					: $"can not find module \"{modulesPath.First()}\" when importing \"{String.Join(".", modulesPath)}\"";

				List<String> similarNamedModules =
					ModuleImportManager.FindSimilarNamedModules(modulesPath.First(), currentModuleDirectory);

				throw new LumenImportException(message, this.lineNo, this.fileName) {
					Note = NameHelper.MakeNamesNote(similarNamedModules)
				};
			}

			String currentImportLocation = rootImportLocation;

			Boolean isVirtualImport = false;

			Module? rootModule = null;
			Module? currentModule = null;

			for (int i = 0; i < modulesPath.Count - 1; i++) {
				String currentRequiredModuleName = modulesPath[i];

				if (isVirtualImport) {
					if (currentModule == null || rootModule == null) {
						throw new LumenImportException("impossible virtual import without module", this.lineNo, this.fileName);
					}

					return (rootModule, ImportRestAsVirtual(modulesPath.Skip(i + 1).ToList(), currentModule));
				}

				(currentModule, isVirtualImport) = 
					ImportIntermediateModule(currentImportLocation, currentRequiredModuleName, currentModule);

				// So module was not founded
				if (currentModule == null) {
					String message = 
						$"can not find module \"{String.Join(".", modulesPath.Take(i + 1))}\" when importing \"{String.Join(".", modulesPath)}\"";

					List<String> similarNamedModules =
						ModuleImportManager.FindSimilarNamedModules(currentRequiredModuleName, currentModuleDirectory, currentImportLocation)
						.Select(similarName => String.Join(".", modulesPath.Take(i)) + "." + similarName)
						.ToList();

					throw new LumenImportException(message, this.lineNo, this.fileName) {
						Note = NameHelper.MakeNamesNote(similarNamedModules)
					};
				}

				// Top-level module is a first founded module
				if (rootModule == null) {
					rootModule = currentModule;
				}

				currentImportLocation = Path.Combine(currentImportLocation, currentRequiredModuleName);
			}

			if (isVirtualImport) {
				if (currentModule == null || rootModule == null) {
					throw new LumenImportException("impossible virtual import without module", this.lineNo, this.fileName);
				}

				return (rootModule, ImportRestAsVirtual(new List<String> { modulesPath.Last() }, currentModule));
			}

			currentModule = ImportTerminalModule(currentImportLocation, modulesPath[modulesPath.Count - 1], currentModule);

			if (currentModule == null) {
				String message =
					$"can not find module \"{String.Join(".", modulesPath)}\" when importing \"{String.Join(".", modulesPath)}\"";

				List<String> similarNamedModules =
					ModuleImportManager.FindSimilarNamedModules(modulesPath.Last(), currentModuleDirectory, currentImportLocation)
					.Select(similarName => String.Join(".", modulesPath.Take(modulesPath.Count - 1)) + "." + similarName)
					.ToList();

				throw new LumenImportException(message, this.lineNo, this.fileName) {
					Note = NameHelper.MakeNamesNote(similarNamedModules)
				};
			}

			// Is the case when we import only top-level module, cycle above is not runs
			if (rootModule == null) {
				rootModule = currentModule;
			}

			return (rootModule, currentModule);
		}

		private static (Module? module, Boolean nextIsVirtual) ImportIntermediateModule(String currentImportLocation, String requiredModuleName, Module? parent) {
			String absolutePath = Path.Combine(currentImportLocation, requiredModuleName);

			if (Directory.Exists(absolutePath)) {
				Module? result = ModuleImportManager.GetCachedModule(absolutePath);

				if (result == null) {
					result = new Module(requiredModuleName);
					ModuleImportManager.SetCachedModule(absolutePath, result);
				}

				if (parent != null) {
					// TODO: How about name's conflicts?
					parent.SetMemberIfAbsent(requiredModuleName, result);
					result.SetMemberIfAbsent(Constants.PARENT_MODULE_SPECIAL_NAME, parent);
				}

				return (result, false);
			}

			String lumenFilePath = absolutePath + ".lm";

			if (File.Exists(lumenFilePath)) {
				Module? result = ModuleImportManager.GetCachedModule(lumenFilePath);

				// If there is no cached module - create and cache it
				if (result == null) {
					result = new Module(requiredModuleName);
					ModuleImportManager.SetCachedModule(lumenFilePath, result);
					ImportFileIntoModule(lumenFilePath, result);
				}

				if (parent != null) {
					// TODO: How about name's conflicts?
					parent.SetMemberIfAbsent(requiredModuleName, result);
					result.SetMemberIfAbsent(Constants.PARENT_MODULE_SPECIAL_NAME, parent);
				}

				return (result, true);
			}

			return (null, false);
		}

		private static Module? ImportTerminalModule(String currentImportLocation, String requiredModuleName, Module? parent) {
			String absolutePath = Path.Combine(currentImportLocation, requiredModuleName);

			if (Directory.Exists(absolutePath)) {
				Module? result = ModuleImportManager.GetCachedModule(absolutePath);

				if (result == null) {
					result = new Module(requiredModuleName);

					ModuleImportManager.SetCachedModule(absolutePath, result);
				}

				if (parent != null) {
					// TODO: How about name's conflicts?
					parent.SetMemberIfAbsent(requiredModuleName, result);
					result.SetMemberIfAbsent(Constants.PARENT_MODULE_SPECIAL_NAME, parent);
				}

				foreach (String directory in Directory.EnumerateDirectories(absolutePath)) {
					String childModuleName = new DirectoryInfo(directory).Name;

					if (NameHelper.IsValidModuleName(childModuleName)) {
						ImportTerminalModule(absolutePath, childModuleName, result);
					}
				}

				foreach (String file in Directory.EnumerateFiles(absolutePath)) {
					String childModuleName = Path.GetFileNameWithoutExtension(file);

					if (NameHelper.IsValidModuleName(childModuleName)) {
						ImportTerminalModule(absolutePath, childModuleName, result);
					}
				}

				return result;
			}

			String lumenFilePath = absolutePath + ".lm";

			if (File.Exists(lumenFilePath)) {
				Module? result = ModuleImportManager.GetCachedModule(lumenFilePath);

				if (result == null) {
					result = new Module(requiredModuleName);

					ModuleImportManager.SetCachedModule(lumenFilePath, result);

					ImportFileIntoModule(lumenFilePath, result);
				}

				if (parent != null) {
					// TODO: How about name's conflicts?
					parent.SetMemberIfAbsent(requiredModuleName, result);
					result.SetMemberIfAbsent(Constants.PARENT_MODULE_SPECIAL_NAME, parent);
				}

				return result;
			}

			return null;
		}

		private static Value ImportRestAsVirtual(List<String> namesToImport, IType value) {
			Value result = value;

			foreach (String name in namesToImport) {
				result = ImportFromVirtualModule(name, (result as IType)!);
			}

			return result;
		}

		private static Value ImportFromVirtualModule(String requiredModuleName, IType parent) {
			return parent.GetMember(requiredModuleName, null);
		}

		private static void ImportFileIntoModule(String path, Module module) {
			Scope moduleScope = new Scope();

			Parser parser = new Parser(new Lexer(File.ReadAllText(path), path).Tokenization(), path);
			List<Expression> expressions = parser.Parse();

			foreach (Expression expression in expressions) {
				expression.Eval(moduleScope);
			}

			foreach (KeyValuePair<String, Value> i in moduleScope.variables) {
				module.SetMember(i.Key, i.Value);
			}

			foreach (String privateName in moduleScope.Privates) {
				module.DeclarePrivate(privateName);
			}
		}
	}
}
