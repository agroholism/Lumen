#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang;

namespace Lumen.Lmi.Importing {
	internal class ModuleImportManager {
		private static readonly List<String> importLocations = new List<String>();

		private static readonly Dictionary<String, Module> globalImportCache = new Dictionary<String, Module>();

		private static readonly IEnumerable<String> allowedModuleExtensions = new[] { ".lm", ".dll" };

		static ModuleImportManager() {
			String interpreterDirectory = AppDomain.CurrentDomain.BaseDirectory;
			importLocations.Add(Path.Combine(interpreterDirectory, "libs"));
			importLocations.Add(Path.Combine(interpreterDirectory, "gpgs"));
		}

		public static String? FindRootImportLocation(String currentModuleDirectory, String requiredModule) {
			IEnumerable<String> importLocations = GetImportLocationsForDirectory(currentModuleDirectory);

			return importLocations.FirstOrDefault(importLocation => IsModuleExists(importLocation, requiredModule));
		}

		public static List<String> FindSimilarNamedModules(String module, String currentModuleLocation, String? importLocation = null) {
			IEnumerable<String> importLocations = importLocation == null
				? GetImportLocationsForDirectory(currentModuleLocation)
				: new List<String> { importLocation };

			HashSet<String> availableModules = new HashSet<String>();

			foreach (IEnumerable<String> modulesNames in importLocations.Select(GetModulesInImportLocation)) {
				foreach (String moduleName in modulesNames) {
					availableModules.Add(moduleName);
				}
			}

			return NameHelper.FindSimilarNames(availableModules, module).ToList();
		}

		public static IEnumerable<String> GetModulesInImportLocation(String importLocation) {
			if (!Directory.Exists(importLocation)) {
				yield break;
			}

			foreach (String directory in Directory.GetDirectories(importLocation)) {
				yield return new DirectoryInfo(directory).Name;
			}

			foreach (String file in Directory.GetFiles(importLocation)) {
				String extension = Path.GetExtension(file).ToLower();
				if (allowedModuleExtensions.Contains(extension)) {
					yield return Path.GetFileNameWithoutExtension(file);
				}
			}
		}

		private static Boolean IsModuleExists(String directory, String requiredModule) {
			String combinedPath = Path.Combine(directory, requiredModule);

			return Directory.Exists(combinedPath) || allowedModuleExtensions.Any(extension => File.Exists(combinedPath + extension));
		}

		public static IEnumerable<String> GetImportLocationsForDirectory(String directory) {
			yield return directory;

			foreach (String location in importLocations) {
				yield return location;
			}
		}

		public static Module? GetCachedModule(String absolutePath) {
			return globalImportCache.TryGetValue(absolutePath, out Module module) ? module : null;
		}

		public static void SetCachedModule(String absolutePath, Module module) {
			globalImportCache[absolutePath] = module;
		}

		public static Boolean IsPackageModule(String absolutePath) {
			return !Path.HasExtension(absolutePath);
		}
	}
}
