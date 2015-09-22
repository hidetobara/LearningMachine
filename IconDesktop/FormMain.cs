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
		const int SCALE = 1;
		LearningManager _Learning;

		public FormMain()
		{
			InitializeComponent();
			LearningManager.Instance = new LearningConvolution();
		}

		private void ButtonDirectory_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialogMain.ShowDialog() != DialogResult.OK) return;

			if (sender == ButtonNeuroDirectory) TextBoxNeuroDirectory.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonInputDirectory) TextBoxInputDirectory.Text = FolderBrowserDialogMain.SelectedPath;
		}

		private void ButtonPath_Click(object sender, EventArgs e)
		{
			if (OpenFileDialogMain.ShowDialog() != DialogResult.OK) return;

			if (sender == ButtonForecastPath) TextBoxForecast.Text = OpenFileDialogMain.FileName;
		}

		private void ButtonRun_Click(object sender, EventArgs e)
		{
			IconTask task = new IconTask() { NeuroDirectory = TextBoxNeuroDirectory.Text };
			if (sender == ButtonRunTraining)
			{
				task.Type = IconTaskType.Training;
				task.Inputs = new List<string>(Directory.GetFiles(TextBoxInputDirectory.Text, "*.png", SearchOption.AllDirectories));
			}
			else if(sender == ButtonRunForecast)
			{
				task.Type = IconTaskType.Forecast;
				task.Inputs.Add(TextBoxForecast.Text);
				task.Details = new List<int>() { (int)NumericUpDownPrimary0.Value, (int)NumericUpDownPrimary1.Value, (int)NumericUpDownPrimary2.Value };
			}
			Properties.Settings.Default.Save();
			if (task.Type == IconTaskType.None) return;

			EnableButtons(false);
			BackgroundWorkerMain.RunWorkerAsync(task);
		}

		private void BackgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
		{
			IconTask task = e.Argument as IconTask;
			if (task == null) return;

			if(_Learning == null)
			{
				_Learning = LearningManager.Instance;
				if (!_Learning.Load(GetNeuroPath(task.NeuroDirectory))) _Learning.Initialize();
			}

			if(task.Type == IconTaskType.Training)
			{
				const int Limit = 1000;
				List<LearningImage> images = new List<LearningImage>();
				int progress = 0;
				foreach (var i in task.Inputs)
				{
					images.Add(LearningImage.LoadPng(i).Shrink(SCALE));

					// 学習開始
					if(images.Count >= Limit)
					{
						_Learning.Learn(images);
						Log.Instance.Info("progress: " + progress);
						BackgroundWorkerMain.ReportProgress(progress);
						images.Clear();
						progress++;
						System.Threading.Thread.Sleep(1);
					}
				}
				if (images.Count > 0) _Learning.Learn(images);

				_Learning.Save(GetNeuroPath(task.NeuroDirectory));
			}
			else if(task.Type == IconTaskType.Forecast && task.Inputs.Count > 0)
			{
				string path = task.Inputs[0];
				string filename = Path.GetFileName(path);
				if (_Learning is LearningConvolution && task.Details.Count > 2) (_Learning as LearningConvolution).ChangeMainMax(task.Details[0], task.Details[1], task.Details[2]);
				LearningImage forecasted = _Learning.Forecast(LearningImage.LoadPng(path).Shrink(SCALE));
				forecasted.SavePng("../" + filename);
				Log.Instance.Info("forecasted: " + filename);
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
	}
}
