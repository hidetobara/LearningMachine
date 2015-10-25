using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrepareDesktop
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
		}

		private void ButtonSprite_Click(object sender, EventArgs e)
		{
			try
			{
				SpriteStrategy strategry = new SpriteStrategy();
				strategry.Run(TextBoxSprite.Text);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "@" + ex.StackTrace, "エラー");
			}
		}

		private void ButtonSpriteFile_Click(object sender, EventArgs e)
		{
			if (OpenFileDialogDefault.ShowDialog() != DialogResult.OK) return;

			if (sender == ButtonSpriteFile) TextBoxSprite.Text = OpenFileDialogDefault.FileName;
		}
	}
}
