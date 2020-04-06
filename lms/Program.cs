using System;

namespace Lumen.Anatomy {
	public class Program {
		public static AnatomyProject CurrentProject { get; set; }

		private static Boolean isExitRequired = false;

		public static void Main() {
			while (!isExitRequired) {
				Console.Write("anatomy:> ");
				String command = Console.ReadLine();
				ParseCommand(command.Split(' '));
			}
		}

		private static void ParseCommand(String[] args) {
			if (args[0] == "new") {
				CreateProject(args[1]);
			}
			else if (args[0] == "open") {
				CurrentProject = AnatomyProject.OpenProject(args[1]);
			}
			else if (args[0] == "pages") {
				if (args.Length == 1) {
					ListPages();
					return;
				}
				if (args[1] == "new") {
					CurrentProject.CreatePage(args[2], 0);
					return;
				}
			}
			else if (args[0] == "build") {
				CurrentProject.Build();
			}
			else if (args[0] == "refresh") {
				CurrentProject.RefreshConfig();
			}
			else if (args[0] == "config" && args[1] == "ignore" && args[2] == "show") {
				Console.WriteLine(String.Join(Environment.NewLine, CurrentProject.Ignore));
			}
			else if (args[0] == "exit") {
				isExitRequired = true;
			}
		}

		private static void ListPages() {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Pages in project: ");
			Console.ForegroundColor = ConsoleColor.Gray;

			foreach (String page in CurrentProject.Pages) {
				Console.WriteLine(page);
			}
		}

		private static void CreateProject(String name) {
			CurrentProject = AnatomyProject.CreateProject(name);
		}
	}
}
