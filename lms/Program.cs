using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumen.Light;
using Lumen.Tomen;

namespace Lumen.Anatomy {
	public class Program {
        public static String ProjectName { get; set; }

        private static String PagesFolder {
            get => ProjectName + "\\model\\pages";
        }

        private static String ScriptsFolder {
            get => ProjectName + "\\model\\scripts";
        }

        private static List<String> Pages { get; } = new List<String>();

        public static void Main() {
            while (true) {
                Console.Write("anatomy:> ");
                String command = Console.ReadLine();
                ParseCommand(command.Split(' '));
            }
        }

        private static void ParseCommand(String[] args) {
            if (args[0] == "new") {
                CreateProject(args[1]);
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

            TomlTable table = Tomen.Tomen.ReadFile(ProjectName + "\\config.toml");
            TomlArray array = table["pages"] as TomlArray;
            foreach (ITomlValue item in array.Value) {
                Pages.Add((item as TomlString).Value);
            }
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

            TomlTable table = Tomen.Tomen.ReadFile(ProjectName + "\\config.toml");

            table["pages"] = new TomlArray(Pages.Select(i => (ITomlValue)new TomlString(i)).ToList());

            TomlTable t = new TomlTable(pageName);
            t["name"] = new TomlString(pageName);
            t["enable_script"] = new TomlBool(true);
            t["path"] = new TomlString(PagesFolder);

            table[pageName] = t;

            Tomen.Tomen.WriteFile(ProjectName + "\\config.toml", table);

            Extensions.Print("Configuration file are rebuilded", ident: ident);

            Extensions.Print($"Page '{pageName}' created successfuly", ConsoleColor.Green, ident);
        }

        private static void ApplyTemplate(String pageName) {
            File.WriteAllText(PagesFolder + "\\" + pageName + ".lm", @"open common");

            File.WriteAllText(PagesFolder + "\\" + pageName + ".lmt",
                String.Format(File.ReadAllText("anatomy\\skillets\\default\\default.lmt"), pageName));
        }

        private static void CreateProject(String name) {
            ProjectName = name;

            Console.WriteLine($"Creating project '{name}'...");

            MakeDirectory(name);
            MakeDirectory(name + "\\model");
            MakeDirectory(PagesFolder);
            MakeDirectory(name + "\\model\\resources");
            MakeDirectory(name + "\\model\\resources\\images");
            MakeDirectory(name + "\\model\\resources\\fonts");
            MakeDirectory(name + "\\model\\scripts");
            MakeDirectory(name + "\\model\\styles");

            File.Create(name + "\\config.toml").Close();

            TomlTable table = new TomlTable(null);
            table["name"] = new TomlString(name);
            table["host"] = new TomlString("");
            table["pages"] = new TomlArray(new List<ITomlValue>());
            table["enable_script"] = new TomlBool(true);

            Tomen.Tomen.WriteFile(name + "\\config.toml", table);

            Extensions.Print("Configuration file created", ident: 1);

            CreateCommonHelper();
            CreatePage("index", 1);

            File.Create(name + "\\model\\resources\\resx.toml").Close();
            Console.WriteLine($"	File '{name + "\\model\\resources\\resx.toml"}' created");

            File.Copy("anatomy\\skillets\\default\\default.css", name + "\\model\\styles\\default.css", true);
            Console.WriteLine($"	File '{name + "\\model\\styles\\default.css"}' created");

            Directory.CreateDirectory(name + "\\build");
            Directory.CreateDirectory(name + "\\tmp");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"	Project '{name}' created successfuly");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void CreateCommonHelper() {
            File.Copy("anatomy\\common.lm", PagesFolder + "\\common.lm", true);

            Extensions.Print($"File {PagesFolder + "\\common.lm"} created", ident: 1);
        }

        private static void MakeDirectory(String name, Boolean logs = true) {
            Directory.CreateDirectory(name);

            if (logs) {
                Extensions.Print($"Directory '{name}' created", ident: 1);
            }
        }
    }
}
