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

namespace TwitterDesktop
{
	public partial class FormMain : Form
	{
		const int LearnImageLimit = 15;

		TwitterManager _Twitter;
		LearningUnit _Learning;

		TaskType _CurrentTask ;
		bool IsRunning { get { return _CurrentTask != TaskType.None; } }

		void EnableButtonLearn(bool enable)
		{
			ButtonLearn.ForeColor = enable ? Color.Red : Color.Black;
		}

		public FormMain()
		{
			InitializeComponent();

			_Learning = new LearningPseudoCNN();
			_Learning.Initialize();
			_Twitter = new TwitterManager();
		}

        private void ButtonLearn_Click(object sender, EventArgs e)
        {
			if(IsRunning)
			{
				_CurrentTask = TaskType.Stop;
				ButtonLearn.ForeColor = Color.Blue;
			}
			else
			{
				TaskLearn task = new TaskLearn();
				task.Type = TaskType.Learn;
				task.RootPath = TextBoxRootPath.Text;
				BackgroundWorkerMain.RunWorkerAsync(task);
				EnableButtonLearn(true);
				Properties.Settings.Default.Save();
				TimerMain.Enabled = true;
			}
        }

		private void ButtonRootPath_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialogMain.ShowDialog() != DialogResult.OK) return;
			string path = FolderBrowserDialogMain.SelectedPath;

			if (sender == ButtonRootPath) TextBoxRootPath.Text = path;
		}

		private void BackgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				TaskLearn task = e.Argument as TaskLearn;
				if (task == null) return;

				_CurrentTask = task.Type;
				if (task.Type == TaskType.Learn)
				{
					string dirNeuro = Path.Combine(task.RootPath, "neuro");
					_Learning.Load(dirNeuro);
					string dir = Path.Combine(task.RootPath, "images\\0");
					if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
					_Twitter.StartStream(dir);

					while (true)
					{
						if (_CurrentTask == TaskType.Stop) break;

						List<string> paths = new List<string>();
						while (true)
						{
							if (_CurrentTask == TaskType.Stop) break;

							System.Threading.Thread.Sleep(1);
							var t = _Twitter.Get();
							if (t != null) paths.Add(t.User.IconPath);
							if (paths.Count > LearnImageLimit) break;
						}
						_Learning.ParallelLearn(paths);
					}
					_Twitter.EndStream();
					_Learning.Save(dirNeuro);
					TimerMain.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
			}
		}

		private void BackgroundWorkerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			EnableButtonLearn(false);
		}

		private void TimerMain_Tick(object sender, EventArgs e)
		{
			string log = "[" + DateTime.Now.ToString() + "]";
			log += "tweets=" + _Twitter.Count() + " ";
			log += Log.Instance.Get();
			Log.Instance.Clear();
			TextBoxLog.AppendText(log + Environment.NewLine);
		}
	}

	enum TaskType { None, Learn, Forecast, Stop }
	class TaskLearn
	{
		public string RootPath;
		public TaskType Type;
	}
}
