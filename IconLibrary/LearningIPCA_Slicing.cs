using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace IconLibrary
{
	/*
	 * 畳み込み用のIPCA
	 */
	public class LearningConvolutionIPCA : LearningIPCA
	{
		public override void Learn(List<LearningImage> images)
		{
			Size s = new Size(Width, Height);
			foreach (var i in images)
			{
				List<LearningImage> list = new List<LearningImage>();
				list.AddRange(i.MakeSlices(s, true));
				base.Learn(list);
			}
		}

		public override LearningImage Project(LearningImage i)
		{
			List<double> results = new List<double>();
			int scaledH = 0;
			int scaledW = 0;
			for (int h = -Height / 2; h < i.Height - Height / 2; h += 1)
			{
				scaledW = 0;
				for (int w = -Width / 2; w < i.Width - Width / 2; w += 1)
				{
					var trimed = i.Trim(new Rectangle(w, h, Width, Height));
					var projected = base.Project(trimed);
					results.AddRange(projected.Data);
					scaledW++;
				}
				scaledH++;
			}
			return new LearningImage(scaledH, scaledW, this.FrameOut.Plane, results.ToArray());
		}

		public override LearningImage BackProject(LearningImage i)
		{
			int Scale = 1;
			ListImage li = new ListImage(i.Height * Scale + Height, i.Width * Scale + Width);
			for (int h = 0; h < i.Height; h++)
			{
				for (int w = 0; w < i.Width; w++)
				{
					LearningImage trimed = i.Trim(new Rectangle(w, h, 1, 1));
					LearningImage pasting = base.BackProject(trimed);
					for (int hh = 0; hh < pasting.Height; hh++)
						for (int ww = 0; ww < pasting.Width; ww++)
							li.Add(h * Scale + hh, w * Scale + ww, pasting.GetPlane(hh, ww));
				}
			}

			LearningImage o = new LearningImage(i.Height * Scale, i.Width * Scale, this.FrameIn.Plane);
			for (int h = 0; h < o.Height; h++)
				for (int w = 0; w < o.Width; w++)
					o.SetPlane(h, w, li.Median(h + Height / 2, w + Width / 2));
			return o;
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
			public void Add(int h, int w, LearningPlane p)
			{
				if (h < 0 || h >= Height || w < 0 || w >= Width)
				{
					return;
				}
				_Data[Width * h + w].Add(p);
			}
			public LearningPlane Median(int h, int w)
			{
				return _Data[Width * h + w].Median();
			}
		}
		class Planes
		{
			List<LearningPlane> _Data = new List<LearningPlane>();
			public void Add(double[] vs) { _Data.Add(new LearningPlane(vs)); }
			public void Add(LearningPlane p) { _Data.Add(p); }
			public LearningPlane Median()
			{
				_Data = _Data.OrderBy(d => d.Euclidean()).ToList();
				if(_Data.Count < 4) return _Data[_Data.Count / 2];

				LearningPlane p = new LearningPlane(_Data[0].Length);
				int count = 0;
				for (int i = 1; i < _Data.Count - 1; i++)
				{
					p.Add(_Data[i]);
					count++;
				}
				return p.Scale(1.0 / count);
			}
		}
		#endregion
	}

	public class LearningIPCA_Slicing : LearningConvolutionIPCA
	{
		private LearningFrame _FrameIn;
		private LearningFrame _FrameOut;
		public override LearningFrame FrameIn { get { return _FrameIn; } }
		public override LearningFrame FrameOut { get { return _FrameOut; } }
		public override string Filename { get { return "IPCA_" + _FrameIn.Plane + "-" + _FrameOut.Plane + "/"; } }

		public LearningIPCA_Slicing(int start, int end, int block = 8)
		{
			_FrameIn = new LearningFrame(block, block, start);
			_FrameOut = new LearningFrame(1, 1, end);
		}
	}
}
