namespace EuIDE {
	partial class MainForm {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.textBox = new FastColoredTextBoxNS.FastColoredTextBox();
			this.menu = new System.Windows.Forms.MenuStrip();
			this.runToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.textBox)).BeginInit();
			this.menu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox._UnsafeText = "";
			this.textBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.textBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
			this.textBox.BackBrush = null;
			this.textBox.BracketsHighlightStringategy = FastColoredTextBoxNS.BracketsHighlightStringategy.StringATEGY_1;
			this.textBox.CharHeight = 14;
			this.textBox.CharWidth = 8;
			this.textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
			this.textBox.Hotkeys = resources.GetString("textBox.Hotkeys");
			this.textBox.IsReplaceMode = false;
			this.textBox.LineNumberColor = System.Drawing.Color.Black;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Name = "textBox";
			this.textBox.Paddings = new System.Windows.Forms.Padding(0);
			this.textBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.textBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("textBox.ServiceColors")));
			this.textBox.ServiceLinesColor = System.Drawing.Color.White;
			this.textBox.Size = new System.Drawing.Size(596, 200);
			this.textBox.TabIndex = 0;
			this.textBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
			this.textBox.Zoom = 100;
			// 
			// menu
			// 
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem1});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menu.Size = new System.Drawing.Size(596, 24);
			this.menu.TabIndex = 1;
			// 
			// runToolStripMenuItem1
			// 
			this.runToolStripMenuItem1.Name = "runToolStripMenuItem1";
			this.runToolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
			this.runToolStripMenuItem1.Text = "Run";
			this.runToolStripMenuItem1.Click += new System.EventHandler(this.RunToolStripMenuItem1_Click);
			// 
			// splitContainer
			// 
			this.splitContainer.BackColor = System.Drawing.Color.WhiteSmoke;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 24);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.textBox);
			this.splitContainer.Size = new System.Drawing.Size(596, 320);
			this.splitContainer.SplitterDistance = 200;
			this.splitContainer.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(596, 344);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.menu);
			this.MainMenuStrip = this.menu;
			this.Name = "MainForm";
			this.Text = "EuIDE";
			((System.ComponentModel.ISupportInitialize)(this.textBox)).EndInit();
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FastColoredTextBoxNS.FastColoredTextBox textBox;
		private System.Windows.Forms.MenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem1;
		private System.Windows.Forms.SplitContainer splitContainer;
	}
}

