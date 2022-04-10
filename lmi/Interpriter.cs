

using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;
using System.Linq;
using System.IO;

namespace Lumen.Lmi {
	public static class Interpriter {
		public static String BasePath { get; set; }

		public static ConsoleColor Color { get; set; } = ConsoleColor.Gray;

		internal static readonly Scope mainScope = new Scope();

		public static IValue? Start(String fileName, Scope? scope = null) {
			String? code = TryRead(fileName);

			if (code != null) {
				return Eval(code, fileName, scope);
			}

			Console.ReadKey();
			return null;
		}

		private static String? TryRead(String fileName) {
			if (System.IO.File.Exists(fileName)) {
				return System.IO.File.ReadAllText(fileName);
			}

			WriteException($"Error: file {fileName} is not exists");
			return null;
		}

		private static void WriteException(String text) {
			Color = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(text);
			Console.ForegroundColor = ConsoleColor.Gray;
			Color = ConsoleColor.Gray;
		}

		public static IValue Eval(String source, String fileName = "", Scope? scope = null) {
			try {
				List<Token> tokens = new Lexer(source, fileName).Tokenization();
				return new Parser(tokens, fileName).Parse().Select(i =>
					i.Eval(scope ?? mainScope)).Last();
			//try { 
			}
			catch (LumenException uex) {
				WriteException(uex.ToString());

				if (uex.Line != -1 && uex.File != null && File.Exists(uex.File)) {
					WriteException("");

					String[] lines = File.ReadAllText(uex.File).Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
					WriteException($"{uex.Line}| {lines[uex.Line - 1]}");
				}

			}
			catch (System.Threading.ThreadAbortException threadAbortException) {
				throw threadAbortException;
			}
			catch (Exception ex) {
				WriteException(ex.Message);
			}

			return Const.UNIT;
		}

		public static List<Expression>? GetAST(String code, String file, Scope? scope = null) {
			try {
				List<Token> tokens = new Lexer(code, file).Tokenization();
				return new Parser(tokens, file).Parse();
			}
			catch {

			}

			return null;
		}
	}
}
