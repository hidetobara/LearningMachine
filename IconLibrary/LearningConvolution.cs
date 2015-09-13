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
		const int Scale = 2;

		protected LearningManager _Learning_3to32 = new LearningIPCA_Slicing_3to32();
		public int BlockHeight { get { return _Learning_3to32.FrameIn.Height; } }
		public int BlockWidth { get { return _Learning_3to32.FrameIn.Width; } }
		public int BlockPlane { get { return _Learning_3to32.FrameOut.Plane; } }
		protected LearningManager _Learning_32to32 = new LearningIPCA_Slicing_32to32();

		public override string Filename { get { return "./"; } }

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			if (!_Learning_3to32.Load(Path.Combine(path, _Learning_3to32.Filename))) _Learning_3to32.Initialize();
			if (!_Learning_32to32.Load(Path.Combine(path, _Learning_32to32.Filename))) _Learning_32to32.Initialize();
			return true;
		}
		public override void Save(string path)
		{
			_Learning_3to32.Save(Path.Combine(path, _Learning_3to32.Filename));
			_Learning_32to32.Save(Path.Combine(path, _Learning_32to32.Filename));
		}

		public override void Learn(List<LearningImage> images)
		{
			// 3to32の学習
			_Learning_3to32.Learn(images);
			// 32to32の学習

			List<LearningImage> compresses = new List<LearningImage>();
			foreach (LearningImage image in images) compresses.Add(CompressColorToComponents(image));
			_Learning_32to32.Learn(compresses);
		}

		public LearningImage Forecast1(LearningImage i)
		{
			var io = CompressColorToComponents(i); io.SavePngAdjusted("../i1.png");
			var o = ExpandComponentsToColor(io); o.SavePngAdjusted("../io1.png");
			io = Forecast2(io);
			o = ExpandComponentsToColor(io); o.SavePngAdjusted("../o1.png");
			return o;
		}

		public LearningImage Forecast2(LearningImage i)
		{
			var io = CompressComponents(i); io.SavePngAdjusted("../i2.png");
			var o = ExpandComponents(io); o.SavePngAdjusted("../o2.png");
			return o;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			return Forecast1(image);
		}

		public LearningImage CompressColorToComponents(LearningImage i)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for(int h = 0; h <= i.Height - BlockHeight; h += Scale)
			{
				scaledW = 0;
				for(int w = 0; w <= i.Width - BlockWidth; w += Scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, BlockWidth, BlockHeight));
					var projected = _Learning_3to32.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, BlockPlane, results.ToArray());
		}

		public LearningImage CompressComponents(LearningImage i)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for (int h = 0; h <= i.Height - BlockHeight; h += Scale)
			{
				scaledW = 0;
				for (int w = 0; w <= i.Width - BlockWidth; w += Scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, BlockWidth, BlockHeight));
					var projected = _Learning_32to32.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, BlockPlane, results.ToArray());
		}

		public LearningImage ExpandComponents(LearningImage i)
		{
			List<Likelihood> list = MakeLikelihoods(i);

			LearningImage o = new LearningImage((i.Height - 1) * Scale + BlockHeight, (i.Width - 1) * Scale + BlockWidth, i.Plane);
			foreach(var l in list)
			{
				LearningImage pasting = _Learning_32to32.BackProject(l.Image);
				o.Paste(l.W * Scale, l.H * Scale, pasting);
			}
			return o;
		}

		public LearningImage ExpandComponentsToColor(LearningImage i)
		{
			List<Likelihood> list = MakeLikelihoods(i);

			LearningImage o = new LearningImage((i.Height - 1) * Scale + BlockHeight, (i.Width - 1) * Scale + BlockWidth, 3);
			foreach (var l in list)
			{
				LearningImage pasting = _Learning_3to32.BackProject(l.Image);
				o.Blend(l.W * Scale, l.H * Scale, pasting);
			}
			return o;
		}

		private List<Likelihood> MakeLikelihoods(LearningImage i)
		{
			List<Likelihood> list = new List<Likelihood>();
			for (int h = 0; h < i.Height; h++)
			{
				for (int w = 0; w < i.Width; w++)
				{
					LearningImage trimed = i.Trim(new Rectangle(w, h, 1, 1));
					list.Add(new Likelihood(trimed, h, w));
				}
			}
			return list.OrderBy(v => v.Value).ToList();
		}

		class Likelihood
		{
			public double Value { private set; get; }
			public LearningImage Image;
			public int W, H;
			public Likelihood(LearningImage i, int h, int w)
			{
				Image = i;
				H = h;
				W = w;
				Value = LearningImage.EuclideanLength(Image);
			}
		}
	}
}
