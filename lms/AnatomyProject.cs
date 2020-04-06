using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumen.Lmi;

namespace Lumen.Anatomy {
	public class AnatomyProject {
		public String Name { get; set; }
		public String AbsolutePath { get; set; }

		public String PagesFolder {
			get => AbsolutePath + "\\model\\pages";
		}
		public String ScriptsFolder {
			get => AbsolutePath + "\\model\\scripts";
		}

		public List<String> Pages { get; set; } = new List<String>();
		public List<String> Ignore { get; set; } = new List<String>();

		public Boolean EnabledScript { get; set; } = true;

		public String GetHost() {
			XDocument document = XDocument.Load(AbsolutePath + "\\config.xml");
			return document.Root.Elements().First(i => i.Name == "host").Value;
		}

		public static AnatomyProject CreateProject(String name) {
			Console.WriteLine($"Creating project '{name}'...");

			AnatomyProject result = new AnatomyProject {
				Name = name,
				AbsolutePath = Directory.GetCurrentDirectory() + "\\" + name
			};

			MakeDirectory(name);
			MakeDirectory(name + "\\model");
			MakeDirectory(result.PagesFolder);
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

			config.Save(result.AbsolutePath + "\\config.xml");

			Extensions.Print("Configuration file created", ident: 1);

			// create main page
			result.CreatePage("index", 1);

			// resources create
			File.Create(result.AbsolutePath + "\\model\\resources\\resx.xml").Close();
			Console.WriteLine($"	File '{name + "\\model\\resources\\resx.xml"}' created");

			// create styles
			File.Copy("anatomy\\skillets\\default\\default.css", name + "\\model\\styles\\default.css", true);
			Console.WriteLine($"	File '{name + "\\model\\styles\\default.css"}' created");

			Directory.CreateDirectory(result.AbsolutePath + "\\build");
			Directory.CreateDirectory(result.AbsolutePath + "\\tmp");

			Extensions.Print($"Project '{name}' created successfuly", ConsoleColor.Green, 1);

			return result;
		}

		public static AnatomyProject OpenProject(String name) {
			AnatomyProject result = new AnatomyProject {
				Name = name,
				AbsolutePath = Directory.GetCurrentDirectory() + "\\" + name
			};

			XDocument document = XDocument.Load(result.AbsolutePath + "\\config.xml");
			result.Pages = document.Root.Elements().First(i => i.Name == "pages").Elements().Select(i => i.Value).ToList();
			result.Ignore = document.Root.Elements().First(i => i.Name == "ignore").Elements().Select(i => i.Value).ToList();
			result.EnabledScript = Boolean.Parse(document.Root.Elements().First(i => i.Name == "enableScript").Value);

			return result;
		}

		public void RefreshConfig() {
			XDocument document = XDocument.Load(this.AbsolutePath + "\\config.xml");
			this.Pages = document.Root.Elements().First(i => i.Name == "pages").Elements().Select(i => i.Value).ToList();
			this.Ignore = document.Root.Elements().First(i => i.Name == "ignore").Elements().Select(i => i.Value).ToList();
			this.EnabledScript = Boolean.Parse(document.Root.Elements().First(i => i.Name == "enableScript").Value);
		}

		public void Build() {
			MakeTmp();

			MakeDirectory(Name + "\\build\\resources");
			MakeDirectory(Name + "\\build\\resources\\images");
			MakeDirectory(Name + "\\build\\resources\\fonts");
			MakeDirectory(Name + "\\build\\scripts");
			MakeDirectory(Name + "\\build\\styles");

			foreach (String i in Directory.EnumerateFiles(ScriptsFolder)) {
				File.Copy(i, Name + "\\build\\scripts\\" + Path.GetFileName(i), true);
			}

			foreach (String i in Directory.EnumerateFiles(Name + "\\model\\styles")) {
				File.Copy(i, Name + "\\build\\styles\\" + Path.GetFileName(i), true);
			}
		}

		public void CreatePage(String pageName, Int32 ident) {
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

			XDocument document = XDocument.Load(Name + "\\config.xml");
			document.Root.Elements().First(i => i.Name == "pages").Add(new XElement("page", pageName));
			document.Save(Name + "\\config.xml");

			Extensions.Print("Configuration file are rebuilded", ident: ident);

			Extensions.Print($"Page '{pageName}' created successfuly", ConsoleColor.Green, ident);
		}

		private void ApplyTemplate(String pageName) {
			File.WriteAllText(PagesFolder + "\\" + pageName + ".lm", "");

			File.WriteAllText(PagesFolder + "\\" + pageName + ".lmt",
				String.Format(File.ReadAllText("anatomy\\skillets\\default\\default.lmt"), pageName));
		}

		private void MakeTmp() {
			// Refresh tmp directory
			foreach (String i in Directory.EnumerateFiles(PagesFolder)) {
				// If it's template file
				if (i.EndsWith(".lmt") && this.Ignore.All(x => PagesFolder + "\\" + x + ".lmt" != i)) {
					String tmpPath = Path.GetFullPath(Name + "\\tmp\\lmt-" + Path.GetFileNameWithoutExtension(i) + ".lm");
					String buildPath = Name + "\\build\\" + Path.GetFileNameWithoutExtension(i) + ".html";
					File.WriteAllText(tmpPath, new HTMLParser(i, Path.GetFullPath(buildPath)).Run());

					String old = Directory.GetCurrentDirectory();
					Directory.SetCurrentDirectory(Name + "\\tmp");
					Interpriter.Start(tmpPath, MakeScope());
					Directory.SetCurrentDirectory(old);
				}
				else {
					File.WriteAllText(Name + "\\tmp\\" + Path.GetFileName(i), File.ReadAllText(i));
				}
			}
		}

		private Lang.Scope MakeScope() {
			Lang.Scope result = new Lang.Scope();

			result.AddUsing(Lang.Prelude.Instance);
			result.AddUsing(new Anatomy(this));

			return result;
		}

		private static void MakeDirectory(String name, Boolean logs = true) {
			Directory.CreateDirectory(name);

			if (logs) {
				Extensions.Print($"Directory '{name}' created", ident: 1);
			}
		}
	}
}
