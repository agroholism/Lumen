using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lumen.Studio {
    public partial class CreateProjectDialog : LumenStudioForm/* Form/**/ {
        public CreateProjectDialog() : base() {
            this.InitializeComponent();

            //* 
            this.panel1.MouseMove += this.FormMouseMove;
            this.panel1.MouseDown += this.FormMouseDown;
            this.panel1.MouseUp += this.FormMouseUp;
            //*/

            this.StartPosition = FormStartPosition.CenterScreen;

            this.BackColor = Settings.LinesColor;
            this.ForeColor = Settings.ForegroundColor;

            this.projectTypesList.BackColor = Settings.BackgroundColor;
            this.projectTypesList.ForeColor = Settings.ForegroundColor;

            this.panel1.BackColor = Settings.BackgroundColor;
            this.panel1.ForeColor = Settings.ForegroundColor;
        }

        private void ShowProjectTypes() {
            Int32 index = 0;
            foreach (ProjectType i in Settings.ProjectTypes) {
                this.projectTypesList.Items.Add(i.Name, index);
                index++;
            }
        }

        private void CreateProjectWindow_Load(Object sender, EventArgs e) {
			this.ShowProjectTypes();
        }
    }
}
