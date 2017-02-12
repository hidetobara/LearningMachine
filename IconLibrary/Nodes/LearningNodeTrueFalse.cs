using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace IconLibrary
{
	public class LearningNodeTrueFalse : LearningNodeConnection
	{
		public override string Filename { get { return "TrueFalse"; } }

		public override int IMAGE_SIZE { get { return 64; } }    // 画像サイズ
		public bool IsParent = false;	// 親を持つかどうか

		public LearningNodeTrueFalse()
		{
			_Nodes = new List<LearningNode>();
			_Nodes.Add(new LearningIPCA_Slicing(3, 16, 8));     // 64-3
			_Nodes.Add(new LearningPool(4));                    // 64-16
			_Nodes.Add(new LearningNormalize());                // 16-16

			var dnn = new LearningDNN(16, 16, 1, 1, 64);
			dnn.OutputReference = -1;
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 10;
			_Nodes.Add(dnn);
		}

		public override void LearnByFiles(List<string> paths)
		{
			List<LearningImage> inputs = new List<LearningImage>();
			List<LearningImage> outputs = new List<LearningImage>();
			foreach (string path in paths)
			{
				string dir = Path.GetDirectoryName(path);
				int index = dir.LastIndexOf('\\');
				string cell = dir.Substring(index + 1);
				index = cell.IndexOf('-');
				if (index == 0) continue;	// 頭に-がついているフォルダは無視
				if (index > 0) cell = cell.Substring(0, index);
				int number = 0;
				if (!int.TryParse(cell, out number)) continue;

				inputs.Add(LearningImage.LoadByZoom(path, IMAGE_SIZE));
				LearningImage image = new LearningImage(1, 1, 1);
				image.Data[0] = number;
				outputs.Add(image);
			}
			LearningNodeGroup group = new LearningNodeGroup();
			group.Slots[0] = inputs;
			group.Slots[-1] = outputs;
			Learn(group);
		}

		public override void ForecastByFiles(List<string> paths, string outDir)
		{
			ForecastByFiles(paths, true);
		}
		public double ForecastByFiles(List<string> paths, bool isDebug = false)
		{
			int trueCount = 0;
			int falseCount = 0;
			foreach (string path in paths)
			{
				var forecasted = Forecast(LearningImage.LoadByZoom(path, IMAGE_SIZE));
				string name = Path.GetFileNameWithoutExtension(path);
				if (isDebug) Log.Instance.Info(name + ": " + forecasted.Data[0]);
				if (forecasted.Data[0] < 0.5) falseCount++; else trueCount++;
			}
			if (isDebug) Log.Instance.Info("True:" + trueCount + " False:" + falseCount);
			return (double)trueCount / (trueCount + falseCount);
		}
	}
}
