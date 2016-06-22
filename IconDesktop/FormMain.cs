using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IconLibrary;


namespace IconDesktop
{
	public partial class FormMain : Form
	{
		LearningImage.ColorType _Color = LearningImage.ColorType.Gray;
		LearningUnit _Learning;

		public FormMain()
		{
			InitializeComponent();
			//LearningUnit.Instance = new LearningDigits();
			LearningUnit.Instance = new LearningIPCA();
		}

		private void ButtonDirectory_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialogMain.ShowDialog() != DialogResult.OK) return;

			if (sender == ButtonNeuroDirectory) TextBoxNeuroDirectory.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonInputDirectory) TextBoxInputDirectory.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonForecast) TextBoxForecast.Text = FolderBrowserDialogMain.SelectedPath;
		}

		private void ButtonPath_Click(object sender, EventArgs e)
		{

		}

		private void ButtonRun_Click(object sender, EventArgs e)
		{
			IconTask task = new IconTask() { NeuroDirectory = TextBoxNeuroDirectory.Text };
			if (sender == ButtonRunTraining)
			{
				task.Type = IconTaskType.Training;
				task.Inputs = new List<string>(Directory.GetFiles(TextBoxInputDirectory.Text, "*.jpg", SearchOption.AllDirectories));
//				task.Inputs.AddRange(Directory.GetFiles(TextBoxInputDirectory.Text, "*.png", SearchOption.AllDirectories));
			}
			else if(sender == ButtonRunForecast)
			{
				task.Type = IconTaskType.Forecast;
				task.Inputs = new List<string>(Directory.GetFiles(TextBoxForecast.Text, "*.png", SearchOption.AllDirectories));
				task.Details = new List<int>() { (int)NumericUpDownPrimary0.Value, (int)NumericUpDownPrimary1.Value, (int)NumericUpDownPrimary2.Value };
			}
			Properties.Settings.Default.Save();
			if (task.Type == IconTaskType.None) return;

			EnableButtons(false);
			BackgroundWorkerMain.RunWorkerAsync(task);
		}

		private void BackgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				IconTask task = e.Argument as IconTask;
				if (task == null) return;

				if (_Learning == null)
				{
					_Learning = LearningUnit.Instance;
					if (!_Learning.Load(GetNeuroPath(task.NeuroDirectory))) _Learning.Initialize();
				}

				if (task.Type == IconTaskType.Training)
				{
					_Learning.Learn(task.Inputs);
					_Learning.Save(GetNeuroPath(task.NeuroDirectory));
				}
				else if (task.Type == IconTaskType.Forecast && task.Inputs.Count > 0)
				{
					foreach (var path in task.Inputs)
					{
						string filename = Path.GetFileName(path);
						LearningImage forecasted = _Learning.Forecast(LearningImage.LoadPng(path, _Color));
						forecasted.SavePng("../" + filename);
						Log.Instance.Info("forecasted: " + filename);
					}
				}
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
			}
		}
		private string GetNeuroPath(string dir) { return Path.Combine(dir, _Learning.Filename); }

		private void BackgroundWorkerMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			UpdateLog();
		}

		private void BackgroundWorkerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			UpdateLog();
			EnableButtons(true);
		}

		private void UpdateLog()
		{
			if (!Log.Instance.Updated) return;
			TextBoxLog.Text = Log.Instance.Get();
			TextBoxLog.Select(TextBoxLog.Text.Length, 0);
			TextBoxLog.ScrollToCaret();
		}

		private void EnableButtons(bool enable)
		{
			ButtonRunTraining.Enabled = enable;
			ButtonRunForecast.Enabled = enable;
		}

		enum IconTaskType { None, Training, Forecast }
		class IconTask
		{
			public IconTaskType Type = IconTaskType.None;
			public string NeuroDirectory;
			public List<string> Inputs = new List<string>();
			public List<string> Outputs = new List<string>();
			public List<int> Details = new List<int>();
		}

		private void TimerMain_Tick(object sender, EventArgs e)
		{
			UpdateLog();
		}
	}
}
