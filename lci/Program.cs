using System;

namespace lci {
	class Program {
		static void Main(string[] args) {
			Stereotype.Provider.Eval(args[0]);
			Console.ReadLine();
		}
	}
}
