using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LumenPad {
	public class Project {
		public String Name { get; set; }
		public String Path { get; set; }

		public Dictionary<String, Object> Metadata { get; set; }

		public ProjectType Type { get; set; }
	}

	public class ProjectType {


		public IRunResult Build(Project project) {

			return null;
		}
	}
}
