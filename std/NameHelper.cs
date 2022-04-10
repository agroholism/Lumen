#nullable enable

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public static class NameHelper {
		private static readonly Regex moduleNameRegex = new Regex(@"^[\p{Ll}\p{Lu}][\w_]+$", RegexOptions.Compiled);

		public static IEnumerable<String> FindSimilarNames(IEnumerable<String> names, String toName) {
			return (
				from name in names
				let distance = Levenshtein(toName, name)
				let relativeDistance = distance / toName.Length
				where relativeDistance < 0.4
				orderby relativeDistance
				select name
			).Distinct();
		}

		private static Int32 Levenshtein(String s, String t) {
			Int32 n = s.Length;
			Int32 m = t.Length;
			Int32[,] d = new Int32[n + 1, m + 1];

			// Verify arguments.
			if (n == 0) {
				return m;
			}

			if (m == 0) {
				return n;
			}

			// Initialize arrays.
			for (Int32 i = 0; i <= n; d[i, 0] = i++) {
			}

			for (Int32 j = 0; j <= m; d[0, j] = j++) {
			}

			// Begin looping.
			for (Int32 i = 1; i <= n; i++) {
				for (Int32 j = 1; j <= m; j++) {
					// Compute cost.
					Int32 cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost
					);
				}
			}
			// Return cost.
			return d[n, m];
		}

		public static String? MakeNamesNote(List<String> similarNames) {
			if (similarNames.Count == 1) {
				return $"Perhaps you meant '{similarNames.First()}'?";
			} else if (similarNames.Count > 3) {
				return $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", similarNames.Take(3))}";
			} else if (similarNames.Count > 1) {
				return $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", similarNames)}";
			}

			return null;
		}

		public static String? MakeNamesNote(IEnumerable<String> names, String toName) {
			return MakeNamesNote(FindSimilarNames(names, toName).ToList());
		}

		public static Boolean IsValidModuleName(String moduleName) {
			return moduleNameRegex.IsMatch(moduleName);
		}
	}
}
