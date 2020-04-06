using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Studio {
	public class AnamomyWebApplication : ProjectType {
		public AnamomyWebApplication() {
			this.Name = "Anatomy Web Application";
			this.Language = Settings.Languages.First(i => i.Name == "Lumen");
		}

		public override IRunResult Build(Project project) {
			throw new NotImplementedException();
		}

		public override Project InitNewProject(String projectName, Dictionary<String, String> options) {
			throw new NotImplementedException();
		}
	}

	public class LumenConsoleApplication : ProjectType {
        public LumenConsoleApplication() {
            this.Name = "Lumen Console Application";
            this.Language = Settings.Languages.First(i => i.Name == "Lumen");
        }

        public override IRunResult Build(Project project) {
            throw new NotImplementedException();
        }

        public override Project InitNewProject(String projectName, Dictionary<String, String> options) {
            throw new NotImplementedException();
        }
    }

    public class LumenFormsApplication : ProjectType {
        public LumenFormsApplication() {
            this.Name = "Lumen Form Application";
            this.Language = Settings.Languages.First(i => i.Name == "Lumen");
        }

        public override IRunResult Build(Project project) {
            throw new NotImplementedException();
        }

        public override Project InitNewProject(String projectName, Dictionary<String, String> options) {
            throw new NotImplementedException();
        }
    }
}
