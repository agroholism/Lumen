namespace Lumen.Studio {
	partial class RegexTester {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegexTester));
			this.fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
			this.fastColoredTextBox2 = new FastColoredTextBoxNS.FastColoredTextBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// fastColoredTextBox1
			// 
			this.fastColoredTextBox1._UnsafeText = "regex";
			this.fastColoredTextBox1.AutoCompleteBracketsList = new char[] {
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
			this.fastColoredTextBox1.AutoScrollMinSize = new System.Drawing.Size(37, 15);
			this.fastColoredTextBox1.BackBrush = null;
			this.fastColoredTextBox1.BracketsHighlightStringategy = FastColoredTextBoxNS.BracketsHighlightStringategy.StringATEGY_1;
			this.fastColoredTextBox1.CharHeight = 15;
			this.fastColoredTextBox1.CharWidth = 7;
			this.fastColoredTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.fastColoredTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.fastColoredTextBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.fastColoredTextBox1.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
			this.fastColoredTextBox1.Hotkeys = resources.GetString("fastColoredTextBox1.Hotkeys");
			this.fastColoredTextBox1.IsReplaceMode = false;
			this.fastColoredTextBox1.Location = new System.Drawing.Point(1, 1);
			this.fastColoredTextBox1.Multiline = false;
			this.fastColoredTextBox1.Name = "fastColoredTextBox1";
			this.fastColoredTextBox1.Paddings = new System.Windows.Forms.Padding(0);
			this.fastColoredTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.fastColoredTextBox1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fastColoredTextBox1.ServiceColors")));
			this.fastColoredTextBox1.ShowLineNumbers = false;
			this.fastColoredTextBox1.ShowScrollBars = false;
			this.fastColoredTextBox1.Size = new System.Drawing.Size(427, 20);
			this.fastColoredTextBox1.TabIndex = 0;
			this.fastColoredTextBox1.Text = "regex";
			this.fastColoredTextBox1.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
			this.fastColoredTextBox1.Zoom = 100;
			// 
			// fastColoredTextBox2
			// 
			this.fastColoredTextBox2._UnsafeText = "Test of regex";
			this.fastColoredTextBox2.AutoCompleteBracketsList = new char[] {
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
			this.fastColoredTextBox2.AutoScrollMinSize = new System.Drawing.Size(93, 15);
			this.fastColoredTextBox2.BackBrush = null;
			this.fastColoredTextBox2.BracketsHighlightStringategy = FastColoredTextBoxNS.BracketsHighlightStringategy.StringATEGY_1;
			this.fastColoredTextBox2.CharHeight = 15;
			this.fastColoredTextBox2.CharWidth = 7;
			this.fastColoredTextBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.fastColoredTextBox2.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.fastColoredTextBox2.Font = new System.Drawing.Font("Consolas", 9.75F);
			this.fastColoredTextBox2.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
			this.fastColoredTextBox2.Hotkeys = resources.GetString("fastColoredTextBox2.Hotkeys");
			this.fastColoredTextBox2.IsReplaceMode = false;
			this.fastColoredTextBox2.Location = new System.Drawing.Point(1, 22);
			this.fastColoredTextBox2.Name = "fastColoredTextBox2";
			this.fastColoredTextBox2.Paddings = new System.Windows.Forms.Padding(0);
			this.fastColoredTextBox2.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.fastColoredTextBox2.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fastColoredTextBox2.ServiceColors")));
			this.fastColoredTextBox2.ShowLineNumbers = false;
			this.fastColoredTextBox2.Size = new System.Drawing.Size(428, 175);
			this.fastColoredTextBox2.TabIndex = 1;
			this.fastColoredTextBox2.Text = "Test of regex";
			this.fastColoredTextBox2.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
			this.fastColoredTextBox2.Zoom = 100;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 227);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 2;
			// 
			// RegexTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 247);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.fastColoredTextBox2);
			this.Controls.Add(this.fastColoredTextBox1);
			this.Name = "RegexTester";
			this.Text = "RegexTester";
			((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
		private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox2;
		private System.Windows.Forms.Label label1;
	}
}