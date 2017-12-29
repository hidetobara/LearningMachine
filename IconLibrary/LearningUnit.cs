using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class LearningUnit : LearningNode
	{
		public enum LearningStyle { None, Input, Output, InputOutput };

		public int Height { get { return FrameIn.Height; } }
		public int Width { get { return FrameIn.Width; } }
		public int Plane { get { return FrameIn.Plane; } }
		public int Length { get { return FrameIn.Length; } }

		#region 旧
		public virtual LearningStyle Style { get { return LearningStyle.None; } }
		public virtual bool IsEnoughToLearn { get { return false; } }
		public virtual void Learn(List<LearningImagePair> pairs, LearningStyle style = LearningStyle.InputOutput) { }

		public virtual LearningImage Project(LearningImage image) { return image; }
		public virtual LearningImage BackProject(LearningImage image) { return image; }
		public virtual LearningImage Forecast(string path) { return null; }
		protected virtual void Forecast(string path, string outdir) { }

		public virtual void ParallelLearn(List<string> paths) { }
		public virtual void ParallelProject(List<LearningImage> inputs, out List<LearningImage> outputs)
		{
			LearningImage[] array = new LearningImage[inputs.Count];
			Parallel.For(0, inputs.Count, GetParallelOptions(), i => array[i] = Project(inputs[i]));
			outputs = new List<LearningImage>(array);
		}
		public virtual void ParallelForecast(List<string> paths, string outdir){ }

		public virtual LearningImage PrepareImage(string path) { return LearningImage.Load(path); }
		#endregion

		#region 新
		public override LearningImage Forecast(LearningImage i) { return Project(i); }
		public override LearningImage BackForecast(LearningImage i) { return BackProject(i); }
		#endregion
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
