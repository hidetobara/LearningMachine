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

		public virtual LearningFrame FrameIn { get { return new LearningFrame(); } }
		public virtual LearningFrame FrameOut { get { return new LearningFrame(); } }

		public virtual string Filename { get { return "Learning.bin"; } }
		public virtual void Initialize() { }
		public virtual bool Load(string path) { return false; }
		public virtual void Save(string path) { }
		public virtual void Learn(List<LearningImage> images) { }

		public virtual LearningImage Project(LearningImage image) { return null; }
		public virtual LearningImage BackProject(LearningImage image) { return null; }
		public virtual LearningImage Forecast(LearningImage image) { return null; }
	}

	public struct LearningFrame
	{
		public int Width;
		public int Height;
		public int Plane;
		public int Area { get { return Width * Height; } }
		public int Length { get { return Width * Height * Plane; } }
	}
}
