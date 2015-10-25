namespace PrepareDesktop
{
	partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TextBoxSprite = new System.Windows.Forms.TextBox();
			this.ButtonSprite = new System.Windows.Forms.Button();
			this.OpenFileDialogDefault = new System.Windows.Forms.OpenFileDialog();
			this.ButtonSpriteFile = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TextBoxSprite
			// 
			this.TextBoxSprite.Location = new System.Drawing.Point(12, 12);
			this.TextBoxSprite.Name = "TextBoxSprite";
			this.TextBoxSprite.Size = new System.Drawing.Size(198, 19);
			this.TextBoxSprite.TabIndex = 0;
			// 
			// ButtonSprite
			// 
			this.ButtonSprite.Location = new System.Drawing.Point(297, 10);
			this.ButtonSprite.Name = "ButtonSprite";
			this.ButtonSprite.Size = new System.Drawing.Size(75, 23);
			this.ButtonSprite.TabIndex = 1;
			this.ButtonSprite.Text = "分割";
			this.ButtonSprite.UseVisualStyleBackColor = true;
			this.ButtonSprite.Click += new System.EventHandler(this.ButtonSprite_Click);
			// 
			// OpenFileDialogDefault
			// 
			this.OpenFileDialogDefault.Filter = "*.*|*.*";
			// 
			// ButtonSpriteFile
			// 
			this.ButtonSpriteFile.Location = new System.Drawing.Point(216, 10);
			this.ButtonSpriteFile.Name = "ButtonSpriteFile";
			this.ButtonSpriteFile.Size = new System.Drawing.Size(75, 23);
			this.ButtonSpriteFile.TabIndex = 2;
			this.ButtonSpriteFile.Text = "ファイル選択";
			this.ButtonSpriteFile.UseVisualStyleBackColor = true;
			this.ButtonSpriteFile.Click += new System.EventHandler(this.ButtonSpriteFile_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 262);
			this.Controls.Add(this.ButtonSpriteFile);
			this.Controls.Add(this.ButtonSprite);
			this.Controls.Add(this.TextBoxSprite);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "Prepare";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TextBoxSprite;
		private System.Windows.Forms.Button ButtonSprite;
		private System.Windows.Forms.OpenFileDialog OpenFileDialogDefault;
		private System.Windows.Forms.Button ButtonSpriteFile;
	}
}

