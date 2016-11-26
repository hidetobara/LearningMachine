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

			foreach (MainMethod m in Enum.GetValues(typeof(MainMethod)))
				ComboBoxMethod.Items.Add(m.ToString());
			ComboBoxMethod.SelectedIndex = 0;
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
					LearningUnit.Instance = ParseUnit(task.Method);
					if (LearningUnit.Instance == null) throw new Exception("No executable method !");

					LearningUnit.Instance.Initialize();
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
					_Learning.ParallelLearn(task.Inputs);
					_Learning.Save(GetNeuroPath(task.NeuroDirectory));
				}
				else if (task.Type == MainTaskType.Forecast && task.Inputs.Count > 0)
				{
					_Learning.ParallelForecast(task.Inputs, task.Outputs.Count > 0 ? task.Outputs[0] : "../");
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
		enum MainMethod { FILTER, CNN_L3, CNN_L4, CNN_L2, CNN_L2_B16, CNN_L2_D48, CNN_L3_D48, CNN_L3_O2, CNN_L3_I128, CNN_L3_I128_B16, DIGITS }

		private LearningUnit ParseUnit(MainMethod method)
		{
			switch (method)
			{
				case MainMethod.DIGITS: return new LearningDigits();
				case MainMethod.FILTER: return new LearningPseudoCNNFilter();
				case MainMethod.CNN_L2: return new LearningPseudoCNN();
				case MainMethod.CNN_L2_B16: return new LearningPseudoCNN_B16();
				case MainMethod.CNN_L2_D48: return new LearningPseudoCNN_D48();
				case MainMethod.CNN_L3: return new LearningPseudoCNN_L3();
				case MainMethod.CNN_L3_D48: return new LearningPseudoCNN_L3_D48();
				case MainMethod.CNN_L3_O2: return new LearningPseudoCNN_L3_O2();
				case MainMethod.CNN_L3_I128: return new LearningPseudoCNN_L3_I128();
				case MainMethod.CNN_L3_I128_B16: return new LearningPseudoCNN_L3_I128_B16();
				case MainMethod.CNN_L4: return new LearningPseudoCNN_L4();
			}
			return null;
		}

		class MainTask
		{
			public MainTaskType Type = MainTaskType.None;
			public string NeuroDirectory;
			public List<string> Inputs = new List<string>();
			public List<string> Outputs = new List<string>();
			public int ImageLimit;
			public MainMethod Method = MainMethod.CNN_L2;

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
