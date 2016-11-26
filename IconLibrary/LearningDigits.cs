using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IconLibrary
{
	/*
	 * 数字の認識に使った
	 */
	public class LearningDigits : LearningProcess
	{
		public LearningDigits()
		{
			_Units = new List<LearningUnit>();
			_Units.Add(new LearningIPCA_Slicing(1, 16));	// (28,28,1)
			_Units.Add(new LearningPool(4));		// (7,7,16)
			_Units.Add(new LearningNormalize());	// (7,7,16)
			_Units.Add(new LearningDNN(7, 16, 4, 1));
		}

		public override void ParallelLearn(List<string> paths)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			foreach (string path in paths)
			{
				LearningImage image = LearningImage.Load(path, LearningImage.ColorType.Gray);
				LearningImage result = MakeOutimageByDirectory(4, 4, path);
				if (result == null) continue;
				pairs.Add(new LearningImagePair(image, result));
			}
			if (pairs.Count > 0) Learn(pairs, LearningStyle.InputOutput);
		}
	}
}
