using Lumen.Lmi;
using System;
using Lumen.Lang;
using System.IO;

namespace lmi {
	public class Program {
        public static void Main(String[] args) {
            Interpriter.BasePath = Directory.GetCurrentDirectory();
            if (args.Length == 0) {
                RunInteractive();
            } else {
                Interpriter.Start(args[0]);
            }
        }

        private static void RunInteractive() {
            Scope mainScope = new Scope();

            while (true) {
                Console.Write(">>> ");
                String command = Console.ReadLine();
                while (!command.TrimEnd().EndsWith(";;")) {
                    Console.Write("... ");
                    command += Environment.NewLine + Console.ReadLine();
                }

                if(command.Trim() == "#reload;;") {
                    mainScope = new Scope();
                    continue;
                }

                if (command.Trim() == "#clear;;") {
                    Console.Clear();
                    continue;
                }

                if (command.Trim() == "#list;;") {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    foreach (System.Collections.Generic.KeyValuePair<String, Value> i in mainScope.variables) {
                        Console.WriteLine($"{i.Key} = {i.Value} :: {i.Value.Type}");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    continue;
                }

                Value result = Interpriter.Eval(command.TrimEnd(new Char[] { ' ', '\t', '\r', '\n', ';' }), "interactive" ,mainScope);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"//-> {result} :: {result.Type}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
