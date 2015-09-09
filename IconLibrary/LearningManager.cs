using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class LearningManager
	{
		private static LearningManager _Instance;
		public static LearningManager Instance
		{
			get { if (_Instance == null) _Instance = new LearningManager(); return _Instance; }
			set { if (_Instance == null) _Instance = value; }
		}

		public int Width = 100;
		public int Height = 100;
		public int Plane = 3;
		public int Length { get { return Width * Height * Plane; } }

		public virtual string Filename { get { return "Learning.bin"; } }
		public virtual void Initialize() { }
		public virtual bool Load(string path) { return false; }
		public virtual void Save(string path) { }
		public virtual void Learn(List<LearningImage> images, int iterate = 1) { }
		public virtual LearningImage Forecast(LearningImage image) { return null; }
	}
}
