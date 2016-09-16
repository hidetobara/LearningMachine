using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IconLibrary
{
	class LearningRecord : LearningUnit
	{
		private List<LearningImage> _Images;
		private string _Tag;

		public LearningRecord(string tag)
		{
			this._Tag = tag;
		}

		public override LearningStyle Style { get { return LearningStyle.Input; } }
		public override string Filename { get { return "Record-" + _Tag; } }
		public override bool Load(string path) { return true; }
		public override void Save(string path)
		{
			for (int i = 0; i < _Images.Count; i++)
			{
				string filename = Path.Combine(path, String.Format("{0:0000}", i) + ".png");
				var highlow = LearningImage.HighLow(_Images[i]);
				_Images[i].SavePng(filename, highlow[1], highlow[0], 1); 
			}
		}

		public override void Learn(List<LearningImage> images)
		{
			_Images = new List<LearningImage>(images);	// クローンする
		}
	}
}
