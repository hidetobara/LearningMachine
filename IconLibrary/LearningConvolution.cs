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

		protected LearningManager _Learning_Color = new LearningIPCA_Slicing_3to32();
		public int ColorHeight { get { return _Learning_Color.FrameIn.Height; } }
		public int ColorWidth { get { return _Learning_Color.FrameIn.Width; } }
		protected LearningManager _Learning_Components32 = new LearningIPCA_Slicing_32to64();
		protected LearningManager _Learning_Components64 = new LearningIPCA_Slicing_64to64();

		public override string Filename { get { return "./"; } }

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			if (!_Learning_Color.Load(Path.Combine(path, _Learning_Color.Filename))) _Learning_Color.Initialize();
			if (!_Learning_Components32.Load(Path.Combine(path, _Learning_Components32.Filename))) _Learning_Components32.Initialize();
			if (!_Learning_Components64.Load(Path.Combine(path, _Learning_Components64.Filename))) _Learning_Components64.Initialize();
			return true;
		}
		public override void Save(string path)
		{
			_Learning_Color.Save(Path.Combine(path, _Learning_Color.Filename));
			_Learning_Components32.Save(Path.Combine(path, _Learning_Components32.Filename));
			_Learning_Components64.Save(Path.Combine(path, _Learning_Components64.Filename));
		}

		public override void Learn(List<LearningImage> images)
		{
			_Learning_Color.Learn(images);

			List<LearningImage> compresses1 = new List<LearningImage>();
			foreach (LearningImage image in images) compresses1.Add(CompressColorToComponents(image));
			_Learning_Components32.Learn(compresses1);

			List<LearningImage> compresses2 = new List<LearningImage>();
			foreach (LearningImage image in compresses1) compresses2.Add(CompressComponents32(image));
			_Learning_Components64.Learn(compresses2);
		}

		public LearningImage ForecastColor(LearningImage i)
		{
			var io = CompressColorToComponents(i); io.SavePngAdjusted("../i1.png");
//			io = Forecast32(io);
			var o = ExpandComponentsToColor(io); o.SavePngAdjusted("../o1.png");
			return o;
		}

		public LearningImage Forecast32(LearningImage i)
		{
			var io = CompressComponents32(i); io.SavePngAdjusted("../i2.png");
//			io = Forecast64(io);
			var o = ExpandComponents32(io); o.SavePngAdjusted("../o2.png");
			return o;
		}

		public LearningImage Forecast64(LearningImage i)
		{
			var io = CompressComponents64(i); io.SavePngAdjusted("../i3.png");
			var o = ExpandComponents64(io); o.SavePngAdjusted("../o3.png");
			return o;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			return ForecastColor(image);
		}

		private LearningImage Compress(LearningManager manager, LearningImage i)
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
					var projected = manager.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, manager.FrameOut.Plane, results.ToArray());
		}
		public LearningImage CompressColorToComponents(LearningImage i)
		{
			return Compress(_Learning_Color, i);
		}
		public LearningImage CompressComponents32(LearningImage i)
		{
			return Compress(_Learning_Components32, i);
		}
		public LearningImage CompressComponents64(LearningImage i)
		{
			return Compress(_Learning_Components64, i);
		}

		private LearningImage Expand(LearningManager manager, LearningImage i)
		{
			LearningImage o = new LearningImage((i.Height - 1) * Scale + ColorHeight, (i.Width - 1) * Scale + ColorWidth, manager.FrameIn.Plane);
			ListImage li = new ListImage(o.Height, o.Width);
			for (int h = 0; h < i.Height; h++)
			{
				for (int w = 0; w < i.Width; w++)
				{
					LearningImage trimed = i.Trim(new Rectangle(w, h, 1, 1));
					LearningImage pasting = manager.BackProject(trimed);
					for (int hh = 0; hh < pasting.Height; hh++)
						for (int ww = 0; ww < pasting.Width; ww++)
							li.Add(h * Scale + hh, w * Scale + ww, pasting.GetPlane(hh, ww));
				}
			}

			for (int h = 0; h < o.Height; h++)
				for (int w = 0; w < o.Width; w++)
					o.SetPlane(h, w, li.Median(h, w).Data);
			return o;
		}
		public LearningImage ExpandComponents64(LearningImage i)
		{
			return Expand(_Learning_Components64, i);
		}
		public LearningImage ExpandComponents32(LearningImage i)
		{
			return Expand(_Learning_Components32, i);
		}
		public LearningImage ExpandComponentsToColor(LearningImage i)
		{
			return Expand(_Learning_Color, i);
		}

		#region 解凍用クラス
		class ListImage
		{
			public int Height { get; private set; }
			public int Width { get; private set; }
			int _Lenght { get { return Width * Height; } }
			Planes[] _Data;
			public ListImage(int height, int width)
			{
				Height = height;
				Width = width;
				_Data = new Planes[Height * Width];
				for (int i = 0; i < _Lenght; i++) _Data[i] = new Planes();
			}
			public void Add(int h, int w, double[] vs)
			{
				_Data[Width * h + w].Add(vs);
			}
			public Plane Median(int h, int w)
			{
				return _Data[Width * h + w].Median();
			}
		}
		class Planes
		{
			List<Plane> _Data = new List<Plane>();
			public void Add(double[] vs) { _Data.Add(new Plane(vs)); }
			public Plane Median()
			{
				_Data.OrderBy(d => d.Euclidean());
				return _Data[_Data.Count / 2];
			}
		}
		class Plane
		{
			public double[] Data;
			public Plane(double[] vs) { Data = vs; }
			public double Euclidean() { return Accord.Math.Norm.Euclidean(Data); }
		}
		#endregion
	}
}
