using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lumen.Studio {
	public static class ProjectManager {
		public static Project Project { get; set; }
		public static String CurrentFile { get; set; }
		public static Boolean AllowRegistrationChanges = true;

		public static TreeView ProjectViewier { get; set; } = new TreeView {
			Visible = true,
			BackColor = Color.FromArgb(2, 44, 55),
			ForeColor = Color.White,
			BorderStyle = BorderStyle.None,
			Dock = DockStyle.Fill,
			TreeViewNodeSorter = new TreeNodeComparer(),
			ShowRootLines = false,
			ImageIndex = 0,
			ImageList = MainForm.Instance.projectImages,
			ItemHeight = 16,
			ShowPlusMinus = false
		};
		public static ContextMenuStrip Menu = new ContextMenuStrip();
		public static FileSystemWatcher Watcher { get; set; }

		static ProjectManager() {
			ProjectViewier.NodeMouseDoubleClick += (sender, e) => OpenFile((e.Node as FileNode).Path, true);
			ProjectViewier.NodeMouseClick += NodeMouseClick;
			ProjectViewier.AfterExpand += (sender, e) => {
				if (e.Node.ImageIndex == 14) {
					e.Node.ImageIndex = 15;
					e.Node.SelectedImageIndex = 15;
				}
			};
			ProjectViewier.AfterCollapse += (sender, e) => {
				if (e.Node.ImageIndex == 15) {
					e.Node.ImageIndex = 14;
					e.Node.SelectedImageIndex = 14;
				}
			};
			//ProjectViewier.ContextMenuStrip = Menu;
			ProjectViewier.ShowPlusMinus = false;
			/*Menu.Items.Add("Добавить файл");
			Menu.Items[0].Click += AppendFile;
			Menu.Items.Add("Добавить папку");
			Menu.Items[1].Click += AppendFolder;
			*/
			/*Watcher.Created += (senderx, ex) => {
				MainForm.Instance.InvokeNeded(() => {
					if (ex.FullPath.Contains('.')) {
						AppendNode(new FileNode(ex.FullPath), GetNessesaryNode(ex.FullPath));
					}
					else {
						AppendNode(new FolderNode(ex.FullPath), GetNessesaryNode(ex.FullPath));
					}
					ProjectViewier.Sort();
				});
			};*/

			/*Watcher.Deleted += (senderx, ex) => {
				MainForm.Instance.InvokeNeded(() => {
					GetNessesaryNode(ex.FullPath).Remove();
					ProjectViewier.Sort();
				});
			};*/

			/*Watcher.Renamed += (senderx, ex) => {
				MainForm.Instance.InvokeNeded(() => {
					TreeNode node = GetNessesaryNode(ex.OldFullPath);
					if (node is FileNode fnode) {
						fnode.Path = ex.FullPath;
						fnode.Text = Path.GetFileName(ex.FullPath);
					}
					else if (node is FolderNode dnode) {
						dnode.Path = ex.FullPath;
						dnode.Text = GetNameFold(ex.FullPath);
					}
					ProjectViewier.Sort();
				});
			};*/
		}

		private static void NodeMouseClick(Object sender, TreeNodeMouseClickEventArgs e) {
			ProjectViewier.SelectedNode = e.Node;
		}

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

			Watcher = new FileSystemWatcher(Project.Path) {
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			};

			MainForm.Instance.Fill(ProjectViewier.Nodes.Add(Project.Name), Path.GetFullPath(selectedPath));
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

		class FileNode : TreeNode {
			public static ContextMenuStrip menu = new ContextMenuStrip();

			public String Path { get; set; }

			static FileNode() {
				menu.Items.Add("Удалить");
				menu.Items[0].Click += (sender, e) => {
					FileNode node = ProjectViewier.SelectedNode as FileNode;
					File.Delete(node.Path);
				};
			}

			public FileNode(String path, Int32 image = 13) {
				this.Path = path;
				this.Text = System.IO.Path.GetFileName(path);
				this.ImageIndex = image;
				this.SelectedImageIndex = image;
				this.ContextMenuStrip = menu;
			}
		}

		class FolderNode : TreeNode {
			public static ContextMenuStrip menu = new ContextMenuStrip {

			};

			static FolderNode() {
				menu.Items.Add("Удалить");
				menu.Items[0].Click += (sender, e) => {
					FolderNode node = ProjectViewier.SelectedNode as FolderNode;
					Directory.Delete(node.Path, true);
				};

				/*menu.Items.Add("Создать файл");
				menu.Items[1].Click += (sender, e) => {
					FolderNode node = ProjectViewier.SelectedNode as FolderNode;
					CreateNewProjectFile form = new CreateNewProjectFile();
					form.ShowDialog();
					String fullFileName = node.Path + "\\" + form.NameFile;
					File.Create(fullFileName).Close();
					//	AppendNode(new FileNode(fullFileName), node);
				};*/

				/*menu.Items.Add("Создать папку");
				menu.Items[2].Click += (sender, e) => {
					FolderNode node = ProjectViewier.SelectedNode as FolderNode;
					AppendFolderWindow form = new AppendFolderWindow();
					form.ShowDialog();
					String fullFolderName = node.Path + "\\" + form.NameFolder;
					Directory.CreateDirectory(fullFolderName);
					//AppendNode(new FolderNode(fullFolderName), node);
				};*/
			}

			public String Path { get; set; }

			public FolderNode(String path, Int32 image = 14) {
				this.Path = path;
				this.Text = GetName(path);
				this.ImageIndex = image;
				this.SelectedImageIndex = image;
				this.ContextMenuStrip = menu;
			}

			public String GetName(String path) {
				String[] cons = path.Split('\\');

				return cons[cons.Length - 1];
			}
		}

		class TreeNodeComparer : System.Collections.IComparer {
			public Int32 Compare(Object x, Object y) {
				if (x is FolderNode && y is FileNode) {
					return -1;
				}

				if (x is FileNode && y is FolderNode) {
					return 1;
				}

				if (x is TreeNode n1 && y is TreeNode n2) {
					return n1.Text[0].CompareTo(n2.Text[1]);
				}
				return 0;
			}
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
