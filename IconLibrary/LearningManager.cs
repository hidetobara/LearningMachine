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

		public virtual int Width { get { return 100; } }
		public virtual int Height { get { return 100; } }
		public virtual int Plane { get { return 3; } }
		public int Length { get { return Width * Height * Plane; } }

		public virtual string Filename { get { return "Learning.bin"; } }
		public virtual void Initialize() { }
		public virtual bool Load(string path) { return false; }
		public virtual void Save(string path) { }
		public virtual void Learn(List<LearningImage> images) { }
		public virtual LearningImage Forecast(LearningImage image) { return null; }
	}
}
