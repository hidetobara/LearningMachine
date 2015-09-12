using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace IconLibrary
{
	public class LearningConvolution : LearningManager
	{
		protected LearningManager _Learning_3to16 = new LearningIPCA_Slicing_3to16();
		public int BlockHeight { get { return _Learning_3to16.FrameIn.Height; } }
		public int BlockWidth { get { return _Learning_3to16.FrameIn.Width; } }
		public int BlockPlane { get { return _Learning_3to16.FrameOut.Plane; } }
		protected LearningManager _Learning_16to16 = new LearningIPCA_Slicing_16to16();

		public override string Filename { get { return "./"; } }

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			Initialize();

			if (_Learning_3to16.Load(Path.Combine(path, _Learning_3to16.Filename))
				&& _Learning_16to16.Load(Path.Combine(path, _Learning_16to16.Filename))) return true;
			return false;
		}
		public override void Save(string path)
		{
			//_Learning_3to16.Save(Path.Combine(path, _Learning_3to16.Filename));
			_Learning_16to16.Save(Path.Combine(path, _Learning_16to16.Filename));
		}

		public override void Learn(List<LearningImage> images)
		{
			// 3to16の学習
			//_Learning_3to16.Learn(images);
			// 16to16の学習
			List<LearningImage> compresses = new List<LearningImage>();
			foreach (LearningImage image in images) compresses.Add(Compress3to16(image));
			_Learning_16to16.Learn(compresses);
		}

		public override LearningImage Project(LearningImage i)
		{
			var i1 = Compress3to16(i); i1.SavePngAdjusted("../i1.png");
			var i2 = Compress16to16(i1); i2.SavePngAdjusted("../i2.png");
			return i2;
		}

		public override LearningImage BackProject(LearningImage i)
		{
			var o1 = Expand16to16(i); o1.SavePngAdjusted("../o1.png");
			var o2 = Expand16to3(o1); o2.SavePngAdjusted("../o2.png");
			return o2;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			var tmp = Project(image);
			return BackProject(tmp);
		}

		public LearningImage Compress3to16(LearningImage i, int scale = 4)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for(int h = 0; h <= i.Height - BlockHeight; h += scale)
			{
				scaledW = 0;
				for(int w = 0; w <= i.Width - BlockWidth; w += scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, BlockWidth, BlockHeight));
					var projected = _Learning_3to16.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, BlockPlane, results.ToArray());
		}

		public LearningImage Compress16to16(LearningImage i, int scale = 4)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for (int h = 0; h <= i.Height - BlockHeight; h += scale)
			{
				scaledW = 0;
				for (int w = 0; w <= i.Width - BlockWidth; w += scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, BlockWidth, BlockHeight));
					var projected = _Learning_16to16.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, BlockPlane, results.ToArray());
		}

		public LearningImage Expand16to16(LearningImage i, int scale = 4)
		{
			LearningImage o = new LearningImage((i.Height - 1) * scale + BlockHeight, (i.Width - 1) * scale + BlockWidth, i.Plane);
			for(int h = 0; h < i.Height; h++)
			{
				for(int w = 0; w < i.Width; w++)
				{
					LearningImage trimed = i.Trim(new Rectangle(w, h, 1, 1));
					LearningImage pasting = _Learning_16to16.BackProject(trimed);
					o.Paste(w * scale, h * scale, pasting);
				}
			}
			return o;
		}

		public LearningImage Expand16to3(LearningImage i, int scale = 4)
		{
			LearningImage o = new LearningImage((i.Height - 1) * scale + BlockHeight, (i.Width - 1) * scale + BlockWidth, 3);
			for (int h = 0; h < i.Height; h++)
			{
				for (int w = 0; w < i.Width; w++)
				{
					LearningImage trimed = i.Trim(new Rectangle(w, h, 1, 1));
					LearningImage pasting = _Learning_3to16.BackProject(trimed);
					o.Paste(w * scale, h * scale, pasting);
				}
			}
			return o;
		}
	}
}
