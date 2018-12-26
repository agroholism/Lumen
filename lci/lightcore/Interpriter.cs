using System;
using System.Collections.Generic;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	public static class Interpriter {
		public static Action<StandartLibrary.Exception, Scope> OnError;

		internal static readonly MainScope mainScope = new MainScope();

		public static Value Start(String fileName, Scope scope = null) {
			try {
				String code = System.IO.File.ReadAllText(fileName);
				List<Token> tokens = new Lexer(code, fileName).Tokenization();
				return new Parser(tokens).Parsing(scope ?? mainScope, fileName);
			}
			catch (StandartLibrary.Exception uex) {
				OnError?.Invoke(uex, scope ?? mainScope);
			}
			catch (System.Threading.ThreadAbortException threadAbortException) {
				throw threadAbortException;
			}
			catch (System.Exception ex) {
				Console.Write(ex.Message);
			}
			
			return Const.NULL;
		}

		public static void Analize(String fileName, Scope scope = null) {

		}

		public static List<Expression> GetAST(String code, String file, Scope scope = null) {
			try {
				List<Token> tokens = new Lexer(code, file).Tokenization();
				return new Parser(tokens).Parsing(file);
			}
			catch { 
			}

			return null;
		}
	}
}
