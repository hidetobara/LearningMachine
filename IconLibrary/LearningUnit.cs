using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class LearningUnit
	{
		public enum LearningStyle { None, Input, Output, InputOutput };

		private static LearningUnit _Instance;
		public static LearningUnit Instance
		{
			get { return _Instance; }
			set { if (_Instance == null) _Instance = value; }
		}

		public virtual LearningFrame FrameIn { get { return new LearningFrame(); } }
		public virtual LearningFrame FrameOut { get { return new LearningFrame(); } }
		public virtual int Scale { get { return 1; } }

		public int Height { get { return FrameIn.Height; } }
		public int Width { get { return FrameIn.Width; } }
		public int Plane { get { return FrameIn.Plane; } }
		public int Length { get { return FrameIn.Length; } }

		public int LearningLimit;

		public virtual string Filename { get { return "./"; } }
		public virtual void Initialize() { }
		public virtual bool Load(string path) { return false; }
		public virtual void Save(string path) { }

		public virtual LearningStyle Style { get { return LearningStyle.None; } }
		public virtual bool IsEnoughToLearn { get { return false; } }
		public virtual void Learn(List<LearningImage> images) { }
		public virtual void Learn(List<LearningImagePair> pairs, LearningStyle style = LearningStyle.InputOutput) { }
		public virtual void Learn(List<string> paths) { }

		public virtual LearningImage Project(LearningImage image) { return image; }
		public virtual LearningImage BackProject(LearningImage image) { return image; }
		public virtual LearningImage Forecast(LearningImage image) { return image; }
		public virtual LearningImage Forecast(string path) { return null; }
		public virtual void Forecast(string path, string outdir) { }
		public virtual LearningImage PrepareImage(string path) { return LearningImage.Load(path); }
	}

	public struct LearningFrame
	{
		public int Width;
		public int Height;
		public int Plane;
		public int Area { get { return Width * Height; } }
		public int Length { get { return Width * Height * Plane; } }
		public LearningFrame(int height, int width, int plane) { Height = height; Width = width; Plane = plane; }
	}
}
