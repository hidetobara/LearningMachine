using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace IconLibrary
{
	public class LearningProcess : LearningUnit
	{
		LearningUnit[] _Units;
		protected LearningUnit _Learning_Color { get { return _Units[0]; } }
		public int ColorHeight { get { return _Learning_Color.FrameIn.Height; } }
		public int ColorWidth { get { return _Learning_Color.FrameIn.Width; } }

		public override string Filename { get { return "./"; } }

		public LearningProcess()
		{
			_Units = new LearningUnit[2];
			_Units[0] = new LearningIPCA_Slicing_3to32();
			_Units[1] = new LearningPool();
//			_Units[2] = new LearningIPCA_Slicing_32to64();
		}

		public void ChangeMainMax(int ipca0, int ipca1, int ipca2)
		{
			if(_Learning_Color is LearningIPCA) (_Learning_Color as LearningIPCA).TemporaryMainMax = ipca0;
		}

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			foreach(var unit in _Units)
			{
				if (!unit.Load(Path.Combine(path, _Learning_Color.Filename))) unit.Initialize();
			}
			return true;
		}
		public override void Save(string path)
		{
			foreach(var unit in _Units)
			{
				unit.Save(Path.Combine(path, _Learning_Color.Filename));
			}
		}

		public override void Learn(List<LearningImage> images)
		{
			// 初期
			List<LearningImage> samples = images;
			List<LearningImage> compresses = new List<LearningImage>();

			_Units[0].Learn(samples);

			for (int i = 1; i < _Units.Length; i++ )
			{
				foreach (var image in samples) compresses.Add(_Units[i - 1].Project(image));
				_Units[i].Learn(compresses);

				samples = compresses;
				compresses = new List<LearningImage>();
			}
		}

		private LearningImage ForecastUnit(int index, LearningImage i)
		{
			if(index >= _Units.Length) return i;

			var c = _Units[index].Project(i);
			c.SavePngAdjusted("../c" + index + ".png");
			var o = ForecastUnit(index + 1, c);
			var e = _Units[index].BackProject(o);
			e.SavePngAdjusted("../e" + index + ".png");
			return e;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			return ForecastUnit(0, image);
		}
	}
}
