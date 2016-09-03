using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IconLibrary
{
	public class LearningPseudoCNN : LearningProcess
	{
		private const int IMAGE_SIZE = 64;

		public override LearningFrame FrameOut { get { return new LearningFrame(4, 4, 1); } }

		public LearningPseudoCNN()
		{
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(3, 16));	// 64,64,3
			_Units.Add(new LearningPool(4));				// 64,64,16
			_Units.Add(new LearningNormalize());			// 16,16,16
			_Units.Add(new LearningIPCA_Slicing(16, 32));	// 16,16,16
			_Units.Add(new LearningPool(4));				// 16,16,32
			_Units.Add(new LearningNormalize());			// 4,4,32
			_Units.Add(new LearningDNN(4, 32, 4, 1));		// 4,4,32 > 4,4,1
		}

		public override void Learn(List<string> paths)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			foreach (string path in paths)
			{
				LearningImage image = PrepareImage(path);
				LearningImage result = MakeOutimage(FrameOut.Height, FrameOut.Width, path);
				if (result == null) continue;
				LearningImagePair pair = new LearningImagePair(image, result);
				pairs.Add(pair);
			}
			Learn(pairs);
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
			return CvImage.Load(path).Zoom(IMAGE_SIZE).ToLearningImage();
		}
	}
}
