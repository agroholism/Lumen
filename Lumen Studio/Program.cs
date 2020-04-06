using System;
using System.Windows.Forms;

namespace Lumen.Studio {
	static class Program {
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main(String[] args) {
			Settings.Loader.Load();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var f = new MainForm(args);
			Application.Run(f);
		}
	}
}
