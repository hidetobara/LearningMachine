using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace IconLibrary
{
	public class LearningPseudoCNNFilter : LearningProcess
	{
		public virtual int IMAGE_IN_SIZE { get { return 64; } }		// 画像入力サイズ
		public virtual int IMAGE_OUT_SIZE { get { return 16; } }	// 画像出力サイズ

		public override LearningFrame FrameOut { get { return new LearningFrame(IMAGE_OUT_SIZE, IMAGE_OUT_SIZE, 3); } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN Filter is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32));    // 64,64,3
			_Units.Add(new LearningPool(4));                // 64,64,32
			_Units.Add(new LearningNormalize());            // 16,16,32
			_Units.Add(new LearningIPCA_Slicing(32, 64));   // 16,16,32
			_Units.Add(new LearningPool(4));                // 16,16,64
			_Units.Add(new LearningNormalize());            // 4,4,64
			var dnn = new LearningDNN(4, 64, IMAGE_OUT_SIZE, 3, 64);
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);        // 4,4,64 > 16,16,3
		}

		public override void ParallelLearn(List<string> paths)
		{
			if (LearningLimit == 0) LearningLimit = 1000;
			int divide = paths.Count / LearningLimit + 1;
			int limit = paths.Count / divide + 1;
			Log.Instance.Info("[PCNN-F.Learn] paths.Count=" + paths.Count + " path=" + paths[0]);
			// 部分学習、分割が1より大きい時に有効
			for (int start = 0, end = limit; start < paths.Count; start += limit, end += limit)
			{
				Log.Instance.Info("[PCNN-F.Learn] start=" + start + " end=" + end);
				Learn(MakeLearningPairs(paths, start, end), LearningStyle.InputOutput);
			}
		}

		private List<LearningImagePair> MakeLearningPairs(List<string> paths, int start, int end)
		{
			List<LearningImagePair> list = new List<LearningImagePair>();
			for (int i = start; i < end; i++)
			{
				if (i >= paths.Count) break;
				var pair = MakeLearningPair(paths[i]);
				if (pair != null) list.Add(pair);
			}
			return list;
		}

		private LearningImagePair MakeLearningPair(string path)
		{
			LearningImage image = PrepareImage(path);
			LearningImage result = image.Shrink(IMAGE_OUT_SIZE);
			if (result == null) return null;
			return new LearningImagePair(image, result);
		}

		public override void ParallelForecast(List<string> paths, string outdir)
		{
			Parallel.ForEach(paths, path => Forecast(path, outdir));
		}

		protected override void Forecast(string path, string outdir)
		{
			LearningImage forecasted = this.Forecast(path);
			string filename = Path.GetFileNameWithoutExtension(path) + ".png";
			forecasted.SavePng(Path.Combine(outdir, filename));
			Log.Instance.Info("filtered: " + filename);
		}

		public override LearningImage PrepareImage(string path)
		{
			return LearningImage.LoadByZoom(path, IMAGE_IN_SIZE);
		}
	}

}
