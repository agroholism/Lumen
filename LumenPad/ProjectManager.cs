using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lumen.Studio {
	public static class ProjectManager {
		public static Project Project { get; set; }
		public static String CurrentFile { get; set; }
		public static Boolean AllowRegistrationChanges = true;

		public static void OpenFile(String path, Boolean savePrevious) {
			if (File.Exists(path)) {
				if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".ico")) {
					OpenImage(path);
					return;
				}

				MainForm.Instance.MakeControlActiveInWorkingArea(MainForm.MainTextBoxManager.TextBox);

				if (savePrevious) {
					SaveFile();
				}

				ProcessExtenstion(Path.GetExtension(path));
				AllowRegistrationChanges = false;

				MainForm.MainTextBoxManager.TextBox.Text = File.ReadAllText(path);

				AllowRegistrationChanges = true;

				MainForm.MainTextBoxManager.TextBox.ClearUndo();

				CurrentFile = path;
			}
		}

		public static void OpenProject(String selectedPath) {
			Project = new Project {
				Name = MainForm.Instance.GetName(selectedPath),
				Path = selectedPath
			};

			MainForm.Instance.Fill(MainForm.Instance.treeView1.Nodes.Add(Project.Name), Path.GetFullPath(selectedPath));
			MainForm.Instance.Refresh();
		}

		public static void CreateFile(String fileName) {
			//
		}

		public static void CreateProject(String fileName) {
			//
		}

		public static void ProcessExtenstion(String extension) {
			foreach (Language language in Settings.Languages) {
				if (language.Extensions.Contains(extension)) {
					MainForm.Instance.CustomizeForLanguage(language);
					return;
				}
			}
		}

		public static void SaveFile() {
			if (CurrentFile != null) {
				File.WriteAllText(CurrentFile, MainForm.MainTextBoxManager.TextBox.Text);
				MainForm.MainTextBoxManager.ChangesSaved = true;
			}
		}

		public static void SaveFileAs() {
			if (CurrentFile != null) {
				File.WriteAllText(CurrentFile, MainForm.MainTextBoxManager.TextBox.Text);
				MainForm.MainTextBoxManager.ChangesSaved = true;
			}
		}

		public static void OpenImage(String path) {
			MainForm.Instance.MakeControlActiveInWorkingArea(MainForm.MainPictureBox);
			MainForm.MainPictureBox.ImageLocation = path;
		}
	}

	public static class DirectoryHelper {
		public static void DeleteRecursive(String dir) {
			foreach (String d in Directory.EnumerateDirectories(dir)) {
				DeleteRecursive(d);
			}

			foreach (String f in Directory.EnumerateFiles(dir)) {
				File.Delete(f);
			}

			Directory.Delete(dir);
		}

		public static void FileWriteWithCreating(String path, String text) {
			String[] s = path.Split('\\');

			for (Int32 i = 1; i < s.Length - 1; i++) {
				String p = "";
				for (Int32 j = 0; j <= i; j++) {
					p += s[j] + "\\";
				}

				if (!Directory.Exists(p)) {
					Directory.CreateDirectory(p);
				}
			}

			File.WriteAllText(path, text);
		}
	}
}
