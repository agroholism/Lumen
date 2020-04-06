using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Argent.Xenon {
	public class Interpreter {
		public static event Action<XenonException> Handle;
		public static event Action<Ast.ExitException> OnExit;

		private static void Main(String[] args) {
			if (args.Length == 0) {
				RunInteractive();
			}
			else {
				//Interpriter.Start(args[0]);
				Console.ReadKey();
			}
		}

		public static void Run(String source, String fileName) {
			Runtime.Scope scope = new Runtime.Scope();

			try {
				List<Ast.Expression> x = new Ast.Parser(new Ast.Lexer(source, fileName).Tokenization(), fileName).Parse();
				foreach (Ast.Expression i in x) {
					i.Eval(scope);
				}
			}
			catch (XenonException euex)  {
				Handle?.Invoke(euex);
			} catch(Ast.ExitException exit) {
				OnExit?.Invoke(exit);
			}
		}

		private static void RunInteractive() {
			Runtime.Scope scope = new Runtime.Scope();

			List<Ast.Expression> x = new Ast.Parser(new Ast.Lexer(System.IO.File.ReadAllText("test.eunet"), "repl").Tokenization(), "repl").Parse();
			foreach (Ast.Expression i in x) {
				i.Eval(scope);
			}
			Console.ReadKey();
		}
	}
}
