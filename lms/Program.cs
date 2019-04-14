using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumen.Light;

namespace Lumen.Anatomy {
	public class Program {
        public static String ProjectName { get; set; }

        private static String PagesFolder {
            get => ProjectName + "\\model\\pages";
        }

        private static String ScriptsFolder {
            get => ProjectName + "\\model\\scripts";
        }

        private static List<String> Pages { get; set; } = new List<String>();

        public static void Main() {
            while (true) {
                Console.Write("anatomy:> ");
                String command = Console.ReadLine();
                ParseCommand(command.Split(' '));
            }
        }

        private static void ParseCommand(String[] args) {
            if (args[0] == "new") {
                CreateProject(args[1], true);
            } else if (args[0] == "open") {
                OpenProject(args[1]);
            } else if (args[0] == "pages") {
                if (args.Length == 1) {
                    ListPages();
                    return;
                }
                if (args[1] == "new") {
                    CreatePage(args[2], 0);
                    return;
                }
            } else if (args[0] == "build") {
                BuildProject();
            }
        }

        private static void BuildProject() {
            MakeTmp();

            MakeDirectory(ProjectName + "\\build\\resources");
            MakeDirectory(ProjectName + "\\build\\resources\\images");
            MakeDirectory(ProjectName + "\\build\\resources\\fonts");
            MakeDirectory(ProjectName + "\\build\\scripts");
            MakeDirectory(ProjectName + "\\build\\styles");

            foreach (String i in Directory.EnumerateFiles(ScriptsFolder)) {
                File.Copy(i, ProjectName + "\\build\\scripts\\" + Path.GetFileName(i), true);
            }

            foreach (String i in Directory.EnumerateFiles(ProjectName + "\\model\\styles")) {
                File.Copy(i, ProjectName + "\\build\\styles\\" + Path.GetFileName(i), true);
            }
        }

        private static void MakeTmp() {
            // Refresh tmp directory
            foreach (String i in Directory.EnumerateFiles(PagesFolder)) {
                // If it's template file
                if (i.EndsWith(".lmt")) {
                    String tmpPath = Path.GetFullPath(ProjectName + "\\tmp\\lmt-" + Path.GetFileNameWithoutExtension(i) + ".lm");
                    String buildPath = ProjectName + "\\build\\" + Path.GetFileNameWithoutExtension(i) + ".html";
                    File.WriteAllText(tmpPath, new HTMLParser(i, Path.GetFullPath(buildPath)).Run());

                    String old = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(ProjectName + "\\tmp");
                    Interpriter.Start(tmpPath, MakeScope());
                    Directory.SetCurrentDirectory(old);
                } else {
                    File.WriteAllText(ProjectName + "\\tmp\\" + Path.GetFileName(i), File.ReadAllText(i));
                }
            }
        }

        private static Lang.Scope MakeScope() {
            Lang.Scope result = new Lang.Scope();
            result.AddUsing(Lang.Prelude.Instance);

           /* TomlTable table = Tomen.Tomen.ReadFile(ProjectName + "\\config.toml");

            Lang. val = new Lang.ModuleValue();

            val.SetField("pages", new Lang.Array(Pages.Select(i => (Lang.Value)new Lang.Prelude.Text(i)).ToList()), null);

            val.SetField("host", new Lang.Text((table["host"] as TomlString).Value), null);
            val.SetField("enable_script", new Lang.Prelude.Bool((table["enable_script"] as TomlBool).Value), null);

            result["config"] = val;
			*/
            return result;
        }

        private static void ListPages() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pages in project: ");
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (String page in Pages) {
                Console.WriteLine(page);
            }
        }

        private static void OpenProject(String v) {
            ProjectName = v;

            Pages.Clear();

			XDocument document = XDocument.Load(ProjectName + "\\config.xml");
			Pages = document.Root.Elements().First(i => i.Name == "pages").Elements().Select(i => i.Value).ToList();
        }

        private static void CreatePage(String pageName, Int32 ident) {
            String identString = "";

            if (ident == 1) {
                identString += "	";
            }

            Extensions.Print($"Creating page '{pageName}'...", ident: ident);
            File.Create(PagesFolder + "\\" + pageName + ".lm").Close();
            Console.WriteLine($"{identString}	File '{PagesFolder + "\\" + pageName + ".lm"}' created");
            File.Create(PagesFolder + "\\" + pageName + ".lmt").Close();
            Console.WriteLine($"{identString}	File '{PagesFolder + "\\" + pageName + ".lmt"}' created");
            File.Create(ScriptsFolder + "\\" + pageName + ".js").Close();
            Console.WriteLine($"{identString}	File '{ScriptsFolder + "\\" + pageName + ".js"}' created");

            ApplyTemplate(pageName);

            Pages.Add(pageName);

			XDocument document = XDocument.Load(ProjectName + "\\config.xml");
			document.Root.Elements().First(i => i.Name == "pages").Add(new XElement("page", pageName));
			document.Save(ProjectName + "\\config.xml");

			Extensions.Print("Configuration file are rebuilded", ident: ident);

            Extensions.Print($"Page '{pageName}' created successfuly", ConsoleColor.Green, ident);
        }

        private static void ApplyTemplate(String pageName) {
            File.WriteAllText(PagesFolder + "\\" + pageName + ".lm", @"#ref common");

            File.WriteAllText(PagesFolder + "\\" + pageName + ".lmt",
                String.Format(File.ReadAllText("anatomy\\skillets\\default\\default.lmt"), pageName));
        }

		/// <summary> Creates a new project in current directory. </summary>
		/// <param name="name"> Name of project. </param>
		/// <param name="allowOutput"> Allow output to console? </param>
        private static void CreateProject(String name, Boolean allowOutput) {
            ProjectName = name;

			if (allowOutput) {
				Console.WriteLine($"Creating project '{name}'...");
			}

			MakeDirectory(name);
            MakeDirectory(name + "\\model");
            MakeDirectory(PagesFolder);
            MakeDirectory(name + "\\model\\resources");
            MakeDirectory(name + "\\model\\resources\\images");
            MakeDirectory(name + "\\model\\resources\\fonts");
            MakeDirectory(name + "\\model\\scripts");
            MakeDirectory(name + "\\model\\styles");

			// creating configuration file
			XDocument config = new XDocument(
				new XElement("project",
					new XElement("name", new XText(name)),
					new XElement("host", new XText("")),
					new XElement("pages"),
					new XElement("enableScript", new XText("True"))
				)
			);

			config.Save(ProjectName + "\\config.xml");

			if (allowOutput) {
				Extensions.Print("Configuration file created", ident: 1);
			}

			// create main page
            CreatePage("index", 1);

			// resources create
            File.Create(name + "\\model\\resources\\resx.xml").Close();
            Console.WriteLine($"	File '{name + "\\model\\resources\\resx.xml"}' created");

			// create styles
            File.Copy("anatomy\\skillets\\default\\default.css", name + "\\model\\styles\\default.css", true);
            Console.WriteLine($"	File '{name + "\\model\\styles\\default.css"}' created");

            Directory.CreateDirectory(name + "\\build");
            Directory.CreateDirectory(name + "\\tmp");

			Extensions.Print($"Project '{name}' created successfuly", ConsoleColor.Green, 1);
        }

        private static void MakeDirectory(String name, Boolean logs = true) {
            Directory.CreateDirectory(name);

            if (logs) {
                Extensions.Print($"Directory '{name}' created", ident: 1);
            }
        }
    }
}
