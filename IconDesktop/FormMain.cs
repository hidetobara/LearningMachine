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
			MainTask task = new MainTask() { NeuroDirectory = TextBoxNeuroDirectory.Text };
			task.ImageLimit = (int)NumericUpDownImageLimit.Value;
			task.ParseMethod(ComboBoxMethod.Text);
			if (sender == ButtonRunTraining)
			{
				task.Type = MainTaskType.Training;
				task.Inputs = GetFiles(TextBoxInputDirectory.Text);
			}
			else if(sender == ButtonRunForecast)
			{
				task.Type = MainTaskType.Forecast;
				task.Inputs = GetFiles(TextBoxForecast.Text);
				task.Outputs.Add(TextBoxForecastOutput.Text);
			}
			Properties.Settings.Default.Save();
			if (task.Type == MainTaskType.None) return;

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
				MainTask task = e.Argument as MainTask;
				if (task == null) return;

				if (LearningUnit.Instance == null)
				{
					if (task.Method == MainMethod.DIGITS) LearningUnit.Instance = new LearningDigits();
					if (task.Method == MainMethod.CNN2) LearningUnit.Instance = new LearningPseudo2CNN();
					if (task.Method == MainMethod.CNN3) LearningUnit.Instance = new LearningPseudo3CNN();
					if (LearningUnit.Instance == null) throw new Exception("No executable method !");

					LearningUnit.Instance.LearningLimit = task.ImageLimit;
				}

				if (_Learning == null)
				{
					_Learning = LearningUnit.Instance;
					if (!_Learning.Load(GetNeuroPath(task.NeuroDirectory))) _Learning.Initialize();
				}

				if (task.Type == MainTaskType.Training)
				{
					task.Inputs = task.Inputs.OrderBy(i => Guid.NewGuid()).ToList();
#if DEBUG
					task.Inputs = task.Inputs.Take(15).ToList();
#endif
					_Learning.Learn(task.Inputs);
					_Learning.Save(GetNeuroPath(task.NeuroDirectory));
				}
				else if (task.Type == MainTaskType.Forecast && task.Inputs.Count > 0)
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

		enum MainTaskType { None, Training, Forecast }
		enum MainMethod { DIGITS, CNN2, CNN3 }
		class MainTask
		{
			public MainTaskType Type = MainTaskType.None;
			public string NeuroDirectory;
			public List<string> Inputs = new List<string>();
			public List<string> Outputs = new List<string>();
			public int ImageLimit;
			public MainMethod Method = MainMethod.CNN2;

			public void ParseMethod(string name)
			{
				foreach(MainMethod m in Enum.GetValues(typeof(MainMethod)))
				{
					if (m.ToString() == name) Method = m;
				}
			}
		}

		private void TimerMain_Tick(object sender, EventArgs e)
		{
			UpdateLog();
		}
	}
}
