using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IconLibrary
{
	/*
	 * 最大値を選択
	 */
	public class LearningPool : LearningUnit
	{
		private LearningFrame _Frame;
		public override LearningFrame FrameIn { get { return _Frame; } }
		public override LearningFrame FrameOut { get { return new LearningFrame() { Height = 1, Width = 1 }; } }

		public override string Filename { get { return "Pool.bin"; } }
		public override void Initialize() { }
		public override bool Load(string path) { return true; }
		public override void Save(string path) { }

		public LearningPool(int step = 2)
		{
			_Frame = new LearningFrame() { Height = step, Width = step };
		}

		public override LearningImage Project(LearningImage image)
		{
			int step = FrameIn.Height;
			int height = image.Height / step;
			int width = image.Width / step;
			LearningImage projected = new LearningImage(height, width, image.Plane);
			for (int h = 0; h < image.Height; h += step)
			{
				for(int w = 0; w < image.Width; w += step)
				{
					var list = image.GetPlanes(new Rectangle(w, h, step, step));
					var best = list.OrderByDescending(x => x.SpecialEuclidean()).First();
					projected.SetPlane(h / step, w / step, best);
				}
			}
			return projected;
		}
		public override LearningImage BackProject(LearningImage image)
		{
			int step = FrameIn.Height;
			int height = image.Height * step;
			int width = image.Width * step;
			LearningImage backed = new LearningImage(height, width, image.Plane);
			for (int h = 0; h < height; h++)
			{
				for (int w = 0; w < width; w++)
				{
					var p = image.GetPlane((double)h / (double)step, (double)w / (double)step);
					backed.SetPlane(h, w, p);
				}
			}
			return backed;
		}
		public override LearningImage Forecast(LearningImage image) { return image; }
	}
}
