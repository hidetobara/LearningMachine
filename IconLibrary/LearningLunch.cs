using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class LearningLunch : LearningProcess
	{
		private const int IMAGE_SIZE = 64;

		public LearningLunch()
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
				LearningImage image = CvImage.Load(path).Zoom(IMAGE_SIZE).ToLearningImage();
				LearningImage result = MakeOutimage(4, 4, path);
				if (result == null) continue;
				LearningImagePair pair = new LearningImagePair(image, result);
				pairs.Add(pair);
			}
			Learn(pairs);
		}
	}
}
