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
		protected LearningIPCA _Learning_Color = new LearningIPCA_Slicing_3to32();
		public int ColorHeight { get { return _Learning_Color.FrameIn.Height; } }
		public int ColorWidth { get { return _Learning_Color.FrameIn.Width; } }
		protected LearningIPCA _Learning_Components1 = new LearningIPCA_Slicing_32to64();
		protected LearningIPCA _Learning_Components2 = new LearningIPCA_Slicing_64to64();

		public override string Filename { get { return "./"; } }

		public void ChangeMainMax(int ipca0, int ipca1, int ipca2)
		{
			_Learning_Color.TemporaryMainMax = ipca0;
			_Learning_Components1.TemporaryMainMax = ipca1;
			_Learning_Components2.TemporaryMainMax = ipca2;
		}

		public override void Initialize()
		{
		}
		public override bool Load(string path)
		{
			if (!_Learning_Color.Load(Path.Combine(path, _Learning_Color.Filename))) _Learning_Color.Initialize();
			if (!_Learning_Components1.Load(Path.Combine(path, _Learning_Components1.Filename))) _Learning_Components1.Initialize();
			if (!_Learning_Components2.Load(Path.Combine(path, _Learning_Components2.Filename))) _Learning_Components2.Initialize();
			return true;
		}
		public override void Save(string path)
		{
			_Learning_Color.Save(Path.Combine(path, _Learning_Color.Filename));
			_Learning_Components1.Save(Path.Combine(path, _Learning_Components1.Filename));
			_Learning_Components2.Save(Path.Combine(path, _Learning_Components2.Filename));
		}

		public override void Learn(List<LearningImage> images)
		{
			_Learning_Color.Learn(images);

			List<LearningImage> compresses1 = new List<LearningImage>();
			foreach (LearningImage image in images) compresses1.Add(CompressColorToComponents(image));
			_Learning_Components1.Learn(compresses1);
		
			List<LearningImage> compresses2 = new List<LearningImage>();
			foreach (LearningImage image in compresses1) compresses2.Add(CompressComponents1(image));
			_Learning_Components2.Learn(compresses2);
		}

		public LearningImage ForecastColor(LearningImage i)
		{
			var io = CompressColorToComponents(i); io.SavePngAdjusted("../ic.png");
			io = Forecast1(io);
			var o = ExpandComponentsToColor(io); o.SavePngAdjusted("../oc.png");
			return o;
		}

		public LearningImage Forecast1(LearningImage i)
		{
			var io = CompressComponents1(i); io.SavePngAdjusted("../i1.png");
			io = Forecast2(io);
			var o = ExpandComponents1(io); o.SavePngAdjusted("../o1.png");
			return o;
		}

		public LearningImage Forecast2(LearningImage i)
		{
			var io = CompressComponents2(i); io.SavePngAdjusted("../i2.png");
			var o = ExpandComponents2(io); o.SavePngAdjusted("../o2.png");
			return o;
		}

		public override LearningImage Forecast(LearningImage image)
		{
			return ForecastColor(image);
		}

		private LearningImage Compress(LearningIPCA manager, LearningImage i)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for (int h = -ColorHeight / 2; h < i.Height - ColorHeight / 2; h += manager.Scale)
			{
				scaledW = 0;
				for (int w = -ColorWidth / 2; w < i.Width - ColorWidth / 2; w += manager.Scale)
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
		public LearningImage CompressComponents1(LearningImage i)
		{
			return Compress(_Learning_Components1, i);
		}
		public LearningImage CompressComponents2(LearningImage i)
		{
			return Compress(_Learning_Components2, i);
		}

		private LearningImage Expand(LearningIPCA manager, LearningImage i)
		{
			int Scale = manager.Scale;
			ListImage li = new ListImage(i.Height * Scale + ColorHeight, i.Width * Scale + ColorWidth);
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

			LearningImage o = new LearningImage(i.Height * Scale, i.Width * Scale, manager.FrameIn.Plane);
			for (int h = 0; h < o.Height; h++)
				for (int w = 0; w < o.Width; w++)
					o.SetPlane(h, w, li.Median(h + ColorHeight / 2, w + ColorWidth / 2).Data);
			return o;
		}
		public LearningImage ExpandComponents2(LearningImage i)
		{
			return Expand(_Learning_Components2, i);
		}
		public LearningImage ExpandComponents1(LearningImage i)
		{
			return Expand(_Learning_Components1, i);
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
				if(h < 0 || h >= Height || w < 0 || w>= Width)
				{
					return;
				}
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
