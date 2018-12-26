using StandartLibrary;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Stereotype.FunctionInclude {
	[Serializable]
	class STD : global::StandartLibrary.Module {
		public STD() {
		//	Set("compute", new Interprate());
			//Set("sleep", new Sleep());
			//Set("abort", new TAbort());
			//Set("Compiller", new Compile());
		}

	/*	class Interprate : SystemFun {
			public override Value Run(Scope e, params Value[] Args) {
				return new Parser(new Lexer(Args[0].ToString(), "").Tokenization()).Parsing(false, System.IO.Directory.GetCurrentDirectory());
			}
		}
		*/
		/*class Sleep : SystemFun {
			public override Value Run(Scope e, params Value[] Args) {
				System.Threading.Thread.Sleep((Int32)Args[0].ToDouble(e));
				return new Null();
			}
		}
		*/
		/*class TAbort : SystemFun {
			public override Value Run(Scope e, params Value[] Args) {
				System.Threading.Thread.CurrentThread.Abort();
				return new Null();
			}
		}*/
	}

	/*class Compile : SystemFun {
		public override Value Run(Scope e, params Value[] Args) {
			// Source code для компиляции      
			string source = Args[0].ToString();
			// Настройки компиляции
			Dictionary<string, string> providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
			CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
			CompilerParameters compilerParams = new CompilerParameters { OutputAssembly = Args[1].ToString(), GenerateExecutable = true };
			// Компиляция      
			// compilerParams.ReferencedAssemblies.Add("Stereotype.Dll");
			CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);
			// Выводим информацию об ошибках      
			Console.WriteLine("Number of Errors: {0}", results.Errors.Count);
			foreach (CompilerError err in results.Errors)
				Console.WriteLine("ERROR {0}", err.ErrorText);
			return new Null();
		}
	}*/
}
