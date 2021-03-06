﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IconLibrary
{
	public class LearningPseudoCNN : LearningProcess
	{
		public virtual int IMAGE_SIZE { get { return 64; } }	// 画像サイズ
		public virtual double ForecastThreshold { get { return 0.3; } }	// 推定の閾値

		public override LearningFrame FrameOut { get { return new LearningFrame(4, 4, 1); } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32));	// 64,64,3
			_Units.Add(new LearningPool(4));				// 64,64,32
			_Units.Add(new LearningNormalize());			// 16,16,32
			_Units.Add(new LearningIPCA_Slicing(32, 64));	// 16,16,32
			_Units.Add(new LearningPool(4));				// 16,16,64
			_Units.Add(new LearningNormalize());			// 4,4,64
			var dnn = new LearningDNN(4, 64, 4, 1, 64);
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);		// 4,4,64 > 4,4,1
		}

		public override void ParallelLearn(List<string> paths)
		{
			if (LearningLimit > 0)
			{
				int divide = paths.Count / LearningLimit + 1;
				int limit = paths.Count / divide + 1;
				Log.Instance.Info("[PCNN.Learn] paths.Count=" + paths.Count + " path=" + paths[0]);
				// 部分学習、分割が1より大きい時に有効
				for (int start = 0, end = limit; start < paths.Count; start += limit, end += limit)
				{
					Log.Instance.Info("[PCNN.Learn] start=" + start + " end=" + end);
					Learn(MakeLearningPairs(paths, start, end), LearningStyle.Input);
				}
			}
			// 全体
			Log.Instance.Info("[PCNN.Learn] LAST");
			Learn(MakeLearningPairs(paths, 0, paths.Count), LearningStyle.InputOutput);
		}

		private List<LearningImagePair> MakeLearningPairs(List<string> paths, int start, int end)
		{
			List<LearningImagePair> list = new List<LearningImagePair>();
			for(int i = start; i < end; i++)
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
			LearningImage result = MakeOutimageByDirectory(FrameOut.Height, FrameOut.Width, path);
			if (result == null) return null;
			return new LearningImagePair(image, result);
		}

		public override void ParallelForecast(List<string> paths, string outdir)
		{
			for (int i = 0; i < FrameOut.Area; i++)
			{
				string dir = Path.Combine(outdir, i.ToString());
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			}

			Parallel.ForEach(paths, path => Forecast(path, outdir));
		}

		protected override void Forecast(string path, string outdir)
		{
			LearningImage forecasted = this.Forecast(path);
			double count = ForecastThreshold;
			int index = -1;
			for(int i = 0; i < FrameOut.Area; i++)
			{
				if (count < forecasted.Data[i]) { count = forecasted.Data[i]; index = i; }
			}
			// 推定されたグループにコピー
			if (index >= 0)
			{
				string dest = Path.Combine(outdir, index.ToString(), Path.GetFileName(path));
				File.Copy(path, dest, true);
			}
			string log = "forecasted: " + Path.GetFileName(path) + ">";
			for (int i = 0; i < forecasted.Data.Length; i++)
			{
				log += (i != 0 ? "," : "") + i + "=" + String.Format("{0:F2}", forecasted.Data[i]);
			}
			Log.Instance.Info(log);
		}

		public override LearningImage PrepareImage(string path)
		{
			return LearningImage.LoadByZoom(path, IMAGE_SIZE).Normalize();
		}
	}

	public class LearningPseudoCNN_B16 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }
		public override void Initialize()
		{
			Log.Instance.Info("PCNN-B16 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 16));// 64,64,3
			_Units.Add(new LearningPool(4));				// 64,64,32
			_Units.Add(new LearningNormalize());			// 16,16,32
			_Units.Add(new LearningIPCA_Slicing(32, 64));	// 16,16,32
			_Units.Add(new LearningPool(4));				// 16,16,64
			_Units.Add(new LearningNormalize());			// 4,4,64
			var dnn = new LearningDNN(4, 64, 4, 1, 64);
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);		// 4,4,64 > 4,4,1
		}
	}

	public class LearningPseudoCNN_D48 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }
		public override void Initialize()
		{
			Log.Instance.Info("PCNN-B16 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 48));	// 64,64,3
			_Units.Add(new LearningPool(4));				// 64,64,32
			_Units.Add(new LearningNormalize());			// 16,16,32
			_Units.Add(new LearningIPCA_Slicing(48, 96));	// 16,16,32
			_Units.Add(new LearningPool(4));				// 16,16,64
			_Units.Add(new LearningNormalize());			// 4,4,64
			var dnn = new LearningDNN(4, 96, 4, 1, 96);
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);		// 4,4,96 > 4,4,1
		}
	}

	public class LearningPseudoCNN_L3 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L3 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 8));		// 64x,3
			_Units.Add(new LearningPool(4));					// 64x,32
			_Units.Add(new LearningNormalize());				// 16x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64, 8));	// 16x,32
			_Units.Add(new LearningPool(2));					// 16x,64
			_Units.Add(new LearningNormalize());				// 8x,64
			_Units.Add(new LearningIPCA_Slicing(64, 128, 4));	// 8x,64
			_Units.Add(new LearningPool(2));					// 8x,128
			_Units.Add(new LearningNormalize());				// 4,4,128
			var dnn = new LearningDNN(4, 128, 4, 1, 128);		// 4,4,128 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}

	public class LearningPseudoCNN_L3_D48 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L3-D48 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 48, 8));		// 64x,3
			_Units.Add(new LearningPool(4));					// 64x,48
			_Units.Add(new LearningNormalize());				// 16x,48
			_Units.Add(new LearningIPCA_Slicing(48, 96, 8));	// 16x,48
			_Units.Add(new LearningPool(2));					// 16x,96
			_Units.Add(new LearningNormalize());				// 8x,96
			_Units.Add(new LearningIPCA_Slicing(96, 144, 4));	// 8x,96
			_Units.Add(new LearningPool(2));					// 8x,144
			_Units.Add(new LearningNormalize());				// 4,4,144
			var dnn = new LearningDNN(4, 144, 4, 1, 144);		// 4,4,144 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}

	public class LearningPseudoCNN_L3_O2 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L3-O2 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 8));		// 64x,3
			_Units.Add(new LearningPool(4));					// 64x,32
			_Units.Add(new LearningNormalize());				// 16x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64, 8));	// 16x,32
			_Units.Add(new LearningPool(2));					// 16x,64
			_Units.Add(new LearningNormalize());				// 8x,64
			_Units.Add(new LearningIPCA_Slicing(64, 128, 4));	// 8x,64
			_Units.Add(new LearningPool(4));					// 8x,128
			_Units.Add(new LearningNormalize());				// 2,2,128
			var dnn = new LearningDNN(2, 128, 4, 1, 128);		// 2,2,128 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}

	public class LearningPseudoCNN_L3_I128 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 128; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L3 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 8));		// 128x,3
			_Units.Add(new LearningPool(4));					// 128x,32
			_Units.Add(new LearningNormalize());				// 32x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64, 8));	// 32x,32
			_Units.Add(new LearningPool(4));					// 32x,64
			_Units.Add(new LearningNormalize());				// 8x,64
			_Units.Add(new LearningIPCA_Slicing(64, 128, 4));	// 8x,64
			_Units.Add(new LearningPool(2));					// 8x,128
			_Units.Add(new LearningNormalize());				// 4,4,128
			var dnn = new LearningDNN(4, 128, 4, 1, 128);		// 4,4,128 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}

	public class LearningPseudoCNN_L3_I128_B16 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 128; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L3 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 16));	// 128x,3
			_Units.Add(new LearningPool(4));					// 128x,32
			_Units.Add(new LearningNormalize());				// 32x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64, 16));	// 32x,32
			_Units.Add(new LearningPool(4));					// 32x,64
			_Units.Add(new LearningNormalize());				// 8x,64
			_Units.Add(new LearningIPCA_Slicing(64, 128, 4));	// 8x,64
			_Units.Add(new LearningPool(2));					// 8x,128
			_Units.Add(new LearningNormalize());				// 4,4,128
			var dnn = new LearningDNN(4, 128, 4, 1, 128);		// 4,4,128 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}

	public class LearningPseudoCNN_L4 : LearningPseudoCNN
	{
		public override int IMAGE_SIZE { get { return 64; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN-L4 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32, 8));		// 64x,3
			_Units.Add(new LearningPool(4));					// 64x,32
			_Units.Add(new LearningNormalize());				// 16x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64, 8));	// 16x,32
			_Units.Add(new LearningPool(2));					// 16x,64
			_Units.Add(new LearningNormalize());				// 8x,64
			_Units.Add(new LearningIPCA_Slicing(64, 128, 4));	// 8x,64
			_Units.Add(new LearningNormalize());				// 8x,128
			_Units.Add(new LearningIPCA_Slicing(128, 192, 4));	// 8x,128
			_Units.Add(new LearningPool(2));					// 8x,192
			_Units.Add(new LearningNormalize());				// 4x,192
			var dnn = new LearningDNN(4, 192, 4, 1, 192);		// 4x,192 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}
}
