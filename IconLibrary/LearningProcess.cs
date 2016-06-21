using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace IconLibrary
{
	/*
	 * LearningUnitを組み合わせて学習
	 */
	public class LearningProcess : LearningUnit
	{
		protected LearningUnit[] _Units;

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			foreach(var unit in _Units)
			{
				if (!unit.Load(Path.Combine(path, unit.Filename))) unit.Initialize();
			}
			return true;
		}
		public override void Save(string path)
		{
			foreach(var unit in _Units)
			{
				unit.Save(Path.Combine(path, unit.Filename));
			}
		}

		public override void Learn(List<LearningImagePair> pairs)
		{
			List<LearningImage> samples = new List<LearningImage>();
			List<LearningImage> compresses = new List<LearningImage>();
			foreach (var pair in pairs) samples.Add(pair.In);

			if (!_Units[0].IsEnoughToLearn) _Units[0].Learn(samples);

			for (int i = 1; i < _Units.Length; i++)
			{
				Log.Instance.Info("[Process.Learn] " + i + " " + samples.Count);
				for (int j = 0; j < samples.Count; j++)
				{
					compresses.Add(_Units[i - 1].Project(samples[j]));
					if (j % 1000 == 0) Log.Instance.Info("[Process.Learn] " + i + "." + j);
				}
				switch(_Units[i].Style)
				{
					case LearningStyle.InputOnly:
						if (!_Units[i].IsEnoughToLearn) _Units[i].Learn(compresses);
						break;
					case LearningStyle.InputOutput:
						List<LearningImagePair> list = new List<LearningImagePair>();
						for (int j = 0; j < compresses.Count; j++) list.Add(new LearningImagePair(compresses[j], pairs[j].Out));
						_Units[i].Learn(list);
						break;
				}

				samples.Clear();
				GC.Collect();
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
			//var e = _Units[index].BackProject(o);
			//e.SavePngAdjusted("../e" + index + ".png");
			return o;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			return ForecastUnit(0, image);
		}
	}
}
