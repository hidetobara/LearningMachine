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

		protected LearningManager _Learning_Color = new LearningIPCA_Slicing_3to16();
		public int ColorHeight { get { return _Learning_Color.FrameIn.Height; } }
		public int ColorWidth { get { return _Learning_Color.FrameIn.Width; } }
		public int ColorPlane { get { return _Learning_Color.FrameOut.Plane; } }
		protected LearningManager _Learning_Components16 = new LearningIPCA_Slicing_16to32();
		public int ComponentPlane { get { return _Learning_Components16.FrameOut.Plane; } }

		public override string Filename { get { return "./"; } }

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			if (!_Learning_Color.Load(Path.Combine(path, _Learning_Color.Filename))) _Learning_Color.Initialize();
			if (!_Learning_Components16.Load(Path.Combine(path, _Learning_Components16.Filename))) _Learning_Components16.Initialize();
			return true;
		}
		public override void Save(string path)
		{
			_Learning_Color.Save(Path.Combine(path, _Learning_Color.Filename));
			_Learning_Components16.Save(Path.Combine(path, _Learning_Components16.Filename));
		}

		public override void Learn(List<LearningImage> images)
		{
			// 3to32の学習
			_Learning_Color.Learn(images);
			// 32to32の学習

			List<LearningImage> compresses = new List<LearningImage>();
			foreach (LearningImage image in images) compresses.Add(CompressColorToComponents(image));
			_Learning_Components16.Learn(compresses);
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
			for(int h = 0; h <= i.Height - ColorHeight; h += Scale)
			{
				scaledW = 0;
				for(int w = 0; w <= i.Width - ColorWidth; w += Scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, ColorWidth, ColorHeight));
					var projected = _Learning_Color.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, ColorPlane, results.ToArray());
		}

		public LearningImage CompressComponents(LearningImage i)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for (int h = 0; h <= i.Height - ColorHeight; h += Scale)
			{
				scaledW = 0;
				for (int w = 0; w <= i.Width - ColorWidth; w += Scale)
				{
					var trimed = i.Trim(new Rectangle(w, h, ColorWidth, ColorHeight));
					var projected = _Learning_Components16.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, ComponentPlane, results.ToArray());
		}

		public LearningImage ExpandComponents(LearningImage i)
		{
			List<Likelihood> list = MakeLikelihoods(i);

			LearningImage o = new LearningImage((i.Height - 1) * Scale + ColorHeight, (i.Width - 1) * Scale + ColorWidth, _Learning_Components16.FrameIn.Plane);
			foreach(var l in list)
			{
				LearningImage pasting = _Learning_Components16.BackProject(l.Image);
				o.Paste(l.W * Scale, l.H * Scale, pasting);
			}
			return o;
		}

		public LearningImage ExpandComponentsToColor(LearningImage i)
		{
			List<Likelihood> list = MakeLikelihoods(i);

			LearningImage o = new LearningImage((i.Height - 1) * Scale + ColorHeight, (i.Width - 1) * Scale + ColorWidth, _Learning_Color.FrameIn.Plane);
			foreach (var l in list)
			{
				LearningImage pasting = _Learning_Color.BackProject(l.Image);
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
				Value = LearningImage.ParticularEuclideanLength(Image);
			}
		}
	}
}
