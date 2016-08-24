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
			_Units = new LearningUnit[5];
			_Units[0] = new LearningIPCA_Slicing(3, 16);
			_Units[1] = new LearningPool(4);
			_Units[2] = new LearningNormalize();
			_Units[3] = new LearningRecord("pca1");
			_Units[4] = new LearningIPCA_Slicing(16, 24);
		}

		public override void Learn(List<string> paths)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			foreach (string path in paths)
			{
				LearningImage image = CvImage.Load(path).Zoom(IMAGE_SIZE).ToLearningImage();
				LearningImagePair pair = new LearningImagePair(image, null);
				pairs.Add(pair);
			}
			Learn(pairs);
		}
	}
}
