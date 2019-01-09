using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Tomen;

namespace Anatomy {
	public class Program {
		public static void Main(String[] args) {
			if(args.Length > 1) {
				ParseCommand(args);
				return;
			}

			String file;

			if(args.Length != 0) {
				file = args[0].ToString();
			} else {
				file = Console.ReadLine();
			}

			new HTMLParser(file).Run();

			Stereotype.Interpriter.Host = Path.GetDirectoryName(file) + "\\tmp";

			foreach (var item in Directory.GetFiles(Path.GetDirectoryName(file) + "\\tmp")) {
				Stereotype.Interpriter.Start(item);
			}
			
		}

		private static void ParseCommand(String[] args) {
			if(args[0] == "new") {
				CreateProject(args[1]);
			}
		}

		private static void CreateProject(String name) {
			Directory.CreateDirectory(name);
			Directory.CreateDirectory(name + "\\model");
			Directory.CreateDirectory(name + "\\model\\pages");
			Directory.CreateDirectory(name + "\\model\\res");
			Directory.CreateDirectory(name + "\\model\\res\\img");
			Directory.CreateDirectory(name + "\\model\\scripts");
			Directory.CreateDirectory(name + "\\model\\styles");

			File.Create(name + "\\model\\pages\\index.lmt").Close();
			File.Create(name + "\\model\\pages\\index.lm").Close();

			File.Create(name + "\\config.toml").Close();

			File.Create(name + "\\model\\res\\resx.toml").Close();

			File.Create(name + "\\model\\scripts\\index.js");
			File.Create(name + "\\model\\styles\\default.css");

			Directory.CreateDirectory(name + "\\bin");
			Directory.CreateDirectory(name + "\\tmp");
		}
	}

	class Writer : TextWriter {
		public override Encoding Encoding => throw new NotImplementedException();
	}

	public class HTMLParser {
		private readonly String source;

		private readonly StringBuilder builder;
		private Int32 position;

		private readonly String file;

		private readonly StringBuilder finalBuilder;

		public HTMLParser(String fileName) {
			this.file = Path.GetFullPath(fileName);
			this.source = File.ReadAllText(fileName);
			this.builder = new StringBuilder();
			this.position = 0;
			this.finalBuilder = new StringBuilder("let res := vec()" + Environment.NewLine);
		}

		public void Run() {
			Char current = Current();
			while (current != '\0') {
				if (current == '<' && At(1) == '%') {
					current = Next();
					current = Next();
					this.finalBuilder.Append("res += \"" + this.builder.ToString().Replace("\"", "\\\"").Replace("#", "\\#") + "\"").Append(Environment.NewLine);
					this.builder.Clear();

					BuildCode();
				}
				else {
					this.builder.Append(current);
				}
				current = Next();
			}

			this.finalBuilder.Append("res += \"" + this.builder.ToString().Replace("\"", "\\\"").Replace("#", "\\#") + "\"").Append(Environment.NewLine);

			this.finalBuilder.Append($"fwrite(\"{(Path.GetDirectoryName(this.file) + "\\rls\\" + Path.GetFileNameWithoutExtension(this.file)).Replace("\\b", "\\\\b").Replace("\\t", "\\\\t").Replace("\\v", "\\\\v").Replace("\\r", "\\\\r")}.html\", res * \"\")"); //C:\Users\пк\Desktop\Projects\LumenPad\lms\bin\Debug\manuals\standart\libs\vec.tmpl

			WorkWithDir(Path.GetDirectoryName(this.file));

			File.WriteAllText((Path.GetDirectoryName(this.file) + "\\tmp\\" + Path.GetFileNameWithoutExtension(this.file) + ".lm"), this.finalBuilder.ToString());
		}

		private void WorkWithDir(String dir) {
			Directory.CreateDirectory(dir + "\\tmp");
			Directory.CreateDirectory(dir + "\\tmp\\helpers");
			Directory.CreateDirectory(dir + "\\rls");

			foreach (String i in Directory.GetFiles(dir + "\\helpers")) {
				if(File.Exists(dir + "\\tmp\\helpers\\" + Path.GetFileName(i))) {
					File.Delete(dir + "\\tmp\\helpers\\" + Path.GetFileName(i));
				}

				File.Copy(i, dir + "\\tmp\\helpers\\" + Path.GetFileName(i));
			}
		}

		private void BuildCode() {
			System.Char current = Current();

			Boolean os = false;
			if (current == '=') {
				current = Next();
				os = true;
			}

			while (true) {
				if (current == '%' && At(1) == '>')
					break;

				builder.Append(current);
				current = Next();
			}

			Next();
			if (os) {
				this.finalBuilder.Append("res += " + builder.ToString()).Append(Environment.NewLine);
			}
			else {
				this.finalBuilder.Append(builder.ToString()).Append(Environment.NewLine);
			}
			this.builder.Clear();
		}

		public System.Char At(Int32 position) {
			return this.source[this.position + position];
		}

		public System.Char Next() {
			if (this.position == this.source.Length) {
				return '\0';
			}
			this.position++;
			return Current();
		}

		private System.Char Current() {
			if (this.position == this.source.Length) {
				return '\0';
			}
			return this.source[this.position];
		}
	}
}
