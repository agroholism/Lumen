using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lmi;

namespace Lumen.Anatomy {
	class Anatomy : Module {
		public Anatomy(AnatomyProject project) {
			this.Name = "Anatomy";

			this.SetMember("host", new Text(project.GetHost()));

			this.SetField("rel x", (scope, args) => {
				return new Text(this.GetMember("host", scope) + scope["x"].ToString(scope));
			});

			this.SetField("script name", (scope, args) => {
				if (project.EnabledScript) {
					return new Text($@"<script type=""text/javascript"" src=""{this.GetMember("host", scope) + $"scripts/{scope["name"]}.js"}""> </script>");
				}
				else {
					return new Text("");
				}
			});

			this.SetField("style name", (scope, args) => {
				return new Text($@"<link rel=""stylesheet"" =""{this.GetMember("host", scope) + $"styles/{scope["name"]}.css"}""> </script>");
			});

			this.SetField("getTemplate name", (scope, args) => {
				return new Template(scope["name"].ToString(scope), project);
			});
		}
	}

	class TemplateType : Module {
		public static Module Template = new TemplateType();

		public TemplateType() {
			this.SetField("buildInto self params", (scope, args) => {
				Template template = scope["self"] as Template;

				String old = Directory.GetCurrentDirectory();
				//Directory.SetCurrentDirectory(template.project.Name + "\\tmp");

				Interpriter.Start(template.templ, new Scope { ["params"] = scope["params"] });
				Directory.SetCurrentDirectory(old);

				return Const.UNIT;
			});
		}
	}

	class Template : Value {
		public IType Type => TemplateType.Template;

		public String templ;
		public AnatomyProject project;

		public Template(String name, AnatomyProject project) {
			String tmpPath = project.AbsolutePath + "\\tmp\\lmt-" + name + ".lm";
			String buildPath = project.AbsolutePath + "\\build\\" + name + ".html";
			File.WriteAllText(tmpPath, new HTMLParser(project.AbsolutePath + "\\model\\pages\\" + name + ".lmt", Path.GetFullPath(buildPath)).Run());

			templ = tmpPath;
			this.project = project;
		}

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return "[#template]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return "[#template]";
		}
	}
}
