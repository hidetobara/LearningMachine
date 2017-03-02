using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace IconLibrary
{
	public class LearningNodeConnection : LearningNode
	{
		protected List<LearningNode> _Nodes;

		public virtual int IMAGE_SIZE { get { return 32; } }    // 画像サイズ

		public override void Initialize()
		{
			foreach (var unit in _Nodes) unit.Initialize();
		}

		public override bool Load(string path)
		{
			foreach (var unit in _Nodes)
			{
				if (!unit.Load(Path.Combine(path, unit.Filename))) unit.Initialize();
			}
			return true;
		}
		public override bool Save(string path)
		{
			foreach (var unit in _Nodes)
			{
				unit.Save(Path.Combine(path, unit.Filename));
			}
			return true;
		}

		public override void Learn(LearningNodeGroup group)
		{
			LearningNodeGroup temporaryGroup = group;
			_Nodes[0].Learn(temporaryGroup);

			for (int n = 1; n < _Nodes.Count; n++)
			{
				temporaryGroup = _Nodes[n - 1].Forecast(temporaryGroup);
				Log.Instance.Info(group.Name + " Learning...[" + n + "]");
				_Nodes[n].Learn(temporaryGroup);
				GC.Collect();
			}
		}

		public override LearningImage Forecast(LearningImage image)
		{
			LearningNodeGroup group = new LearningNodeGroup() { Name = "Forecast" };
			group.Slots[0] = new List<LearningImage>() { image };

			for (int n = 0; n < _Nodes.Count; n++)
			{
				group = _Nodes[n].Forecast(group);
			}
			return group.Slots[0][0];
		}

		#region 追加の関数
		public virtual void LearnByFiles(List<string> paths)
		{
			List<LearningImage> list = new List<LearningImage>();
			foreach(string path in paths)
			{
				list.Add(LearningImage.LoadByZoom(path, IMAGE_SIZE));
			}
			LearningNodeGroup group = new LearningNodeGroup() { Name = "Main" };
			group.Slots[0] = list;
			Learn(group);
		}

		public virtual void ForecastByFiles(List<string> paths, string outDir)
		{
			if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
			Parallel.ForEach(paths, GetParallelOptions(), path =>
				{
					var forecasted = Forecast(LearningImage.LoadByZoom(path, IMAGE_SIZE));
					string name = Path.GetFileNameWithoutExtension(path);
					string outpath = Path.Combine(outDir, name + ".png");
					forecasted.SavePng(outpath);
					Log.Instance.Info("forecasted: " + name);
				});
		}
		#endregion
	}
}
