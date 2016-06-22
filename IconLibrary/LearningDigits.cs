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
			_Units = new LearningUnit[4];
			_Units[0] = new LearningIPCA_Slicing(1, 16);	// (28,28,1)
			_Units[1] = new LearningPool(4);	// (7,7,16)
			_Units[2] = new LearningNormalize();	// (7,7,16)
			_Units[3] = new LearningDNN(7, 16, 4, 1);
		}

		public override void Learn(List<string> paths)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			foreach (string path in paths)
			{
				LearningImage image = LearningImage.LoadPng(path, LearningImage.ColorType.Gray);
				string dir = Path.GetDirectoryName(path);
				int index = dir.LastIndexOf('\\');
				string group = dir.Substring(index + 1);
				int number = -1;
				if (!int.TryParse(group, out number)) continue;
				LearningImage result = new LearningImage(4, 4, 1);
				if (number < 16) result.Data[number] = 1;
				pairs.Add(new LearningImagePair(image, result));
			}
			if (pairs.Count > 0) Learn(pairs);
		}
	}
}
