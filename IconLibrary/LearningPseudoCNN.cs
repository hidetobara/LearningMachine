﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IconLibrary
{
	public class LearningPseudo2CNN : LearningProcess
	{
		protected virtual int IMAGE_SIZE { get { return 64; } }

		public override LearningFrame FrameOut { get { return new LearningFrame(4, 4, 1); } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN2 is active");
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

		public override void Learn(List<string> paths)
		{
			int divide = paths.Count / LearningLimit + 1;
			int limit = paths.Count / divide + 1;
			Log.Instance.Info("[PCNN.Learn] paths.Count=" + paths.Count + " path=" + paths[0]);
			// 部分学習、分割が1より大きい時に有効
			if (divide > 1)
			{
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
			LearningImage result = MakeOutimage(FrameOut.Height, FrameOut.Width, path);
			if (result == null) return null;
			return new LearningImagePair(image, result);
		}

		public override void Forecast(string path, string outdir)
		{
			for(int i = 0; i < FrameOut.Area; i++)
			{
				string dir = Path.Combine(outdir, i.ToString());
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			}

			LearningImage forecasted = this.Forecast(path);
			double count = forecasted.Data[0];
			int index = 0;
			for(int i = 0; i < FrameOut.Area; i++)
			{
				if (count < forecasted.Data[i]) { count = forecasted.Data[i]; index = i; }
			}
			string dest = Path.Combine(outdir, index.ToString(), Path.GetFileName(path));
			File.Copy(path, dest, true);
			string log = "forecasted: " + Path.GetFileName(path) + ">";
			for (int i = 0; i < forecasted.Data.Length; i++)
			{
				log += (i != 0 ? "," : "") + i + "=" + String.Format("{0:F2}", forecasted.Data[i]);
			}
			Log.Instance.Info(log);
		}

		public override LearningImage PrepareImage(string path)
		{
			return LearningImage.LoadByZoom(path, IMAGE_SIZE);
			//return CvImage.Load(path).Zoom(IMAGE_SIZE).ToLearningImage();	// 古い
		}
	}

	public class LearningPseudo3CNN : LearningPseudo2CNN
	{
		protected override int IMAGE_SIZE { get { return 256; } }

		public override void Initialize()
		{
			Log.Instance.Info("PCNN3 is active");
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 32));		// 256x,3
			_Units.Add(new LearningPool(4));					// 256x,32
			_Units.Add(new LearningNormalize());				// 64x,32
			_Units.Add(new LearningIPCA_Slicing(32, 64));		// 64x,32
			_Units.Add(new LearningPool(4));					// 64x,64
			_Units.Add(new LearningNormalize());				// 16x,64
			_Units.Add(new LearningIPCA_Slicing(64, 96));		// 16x,64
			_Units.Add(new LearningPool(4));					// 16x,96
			_Units.Add(new LearningNormalize());				// 4,4,96
			var dnn = new LearningDNN(4, 64, 4, 1, 96);			// 4,4,96 > 4,4,1
			dnn.DropoutRate = 0.5;
			dnn.DropoutPadding = 15;
			_Units.Add(dnn);
		}
	}
}
