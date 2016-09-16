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
		LearningUnit _Learning;

		public FormMain()
		{
			InitializeComponent();
			//LearningUnit.Instance = new LearningDigits();
			//LearningUnit.Instance = new LearningIPCA();
			LearningUnit.Instance = new LearningPseudo2CNN();
		}

		private void ButtonDirectory_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialogMain.ShowDialog() != DialogResult.OK) return;

			if (sender == ButtonNeuroDirectory) TextBoxNeuroDirectory.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonInputDirectory) TextBoxInputDirectory.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonForecast) TextBoxForecast.Text = FolderBrowserDialogMain.SelectedPath;
			if (sender == ButtonForecastOutput) TextBoxForecastOutput.Text = FolderBrowserDialogMain.SelectedPath;
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
				task.Inputs = GetFiles(TextBoxInputDirectory.Text);
			}
			else if(sender == ButtonRunForecast)
			{
				task.Type = IconTaskType.Forecast;
				task.Inputs = GetFiles(TextBoxForecast.Text);
				task.Outputs.Add(TextBoxForecastOutput.Text);
			}
			Properties.Settings.Default.Save();
			if (task.Type == IconTaskType.None) return;

			EnableButtons(false);
			BackgroundWorkerMain.RunWorkerAsync(task);
		}

		List<string> GetFiles(string directory)
		{
			List<string> files = new List<string>(Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories));
			files.AddRange(Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories));
			return files;
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
					task.Inputs = task.Inputs.OrderBy(i => Guid.NewGuid()).ToList();
#if DEBUG
					task.Inputs = task.Inputs.Take(10).ToList();
#endif
					_Learning.Learn(task.Inputs);
					_Learning.Save(GetNeuroPath(task.NeuroDirectory));
				}
				else if (task.Type == IconTaskType.Forecast && task.Inputs.Count > 0)
				{
					foreach (var path in task.Inputs)
					{
						string dir = "../";
						if(task.Outputs.Count > 0) dir = task.Outputs[0];
						_Learning.Forecast(path, dir);
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
		}

		private void TimerMain_Tick(object sender, EventArgs e)
		{
			UpdateLog();
		}
	}
}
