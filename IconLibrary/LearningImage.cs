using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Accord.Math;

namespace IconLibrary
{
    public class LearningImage
    {
		public enum ColorType { Color, Gray };

		public string Name;
		private LearningFrame _Frame;
		public int Height { get { return _Frame.Height; } }
		public int Width { get { return _Frame.Width; } }
		public int Area { get { return _Frame.Area; } }
		public int Plane { get { return _Frame.Plane; } }
		public int Length { get { return _Frame.Length; } }
		public double[] Data;

		public LearningPlane GetPlane(int h, int w)
		{
			if (h < 0) h = 0;
			if (h >= Height) h = Height - 1;
			if (w < 0) w = 0;
			if (w >= Width) w = Width - 1;
			double[] p = new double[Plane];
			Array.Copy(Data, (Width * h + w) * Plane, p, 0, Plane);
			return new LearningPlane(p);
		}
		public LearningPlane GetPlane(double h, double w)
		{
			int h0 = (int)h;
			int h1 = (int)h + 1;
			int w0 = (int)w;
			int w1 = (int)w + 1;
			double w00 = (h1 - h) + (w1 - w);
			double w10 = (h - h0) + (w1 - w);
			double w01 = (h1 - h) + (w - w0);
			double w11 = (h - h0) + (w - w0);
			double sum = w00 + w10 + w01 + w11;
			LearningPlane p = new LearningPlane(Plane);
			p.Add(GetPlane(h0, w0).Scale(w00 / sum), GetPlane(h1, w0).Scale(w10 / sum), GetPlane(h0, w1).Scale(w01 / sum), GetPlane(h1, w1).Scale(w11 / sum));
			return p;
		}
		public LearningPlane GetPlaneStrict(double h, double w)
		{
			int h0 = (int)h;
			int w0 = (int)w;
			double d = Math.Abs(h - h0) + Math.Abs(w - w0);
			if (d != 0) return new LearningPlane(this.Plane);	// 本当に厳密に
			return GetPlane(h0, w0).Scale(0.001 / (d + 0.001));
		}
		public void SetPlane(int h, int w, LearningPlane p)
		{
			if (p == null) { Array.Clear(Data, (Width * h + w) * Plane, Plane); return; }
			Array.Copy(p.Data, 0, Data, (Width * h + w) * Plane, Plane);
		}

		public LearningImage(LearningFrame f, double[] data = null)
		{
			_Frame = f;
			Data = new double[Length];
			if (data != null) Array.Copy(data, Data, Math.Min(data.Length, Length));
		}
		public LearningImage(int height, int width, int plane = 3, double[] data = null)
		{
			_Frame = new LearningFrame() { Height = height, Width = width, Plane = plane };
			Data = new double[Length];
			if (data != null) Array.Copy(data, Data, Math.Min(data.Length, Length));
		}
		public LearningImage(LearningImage image) : this(image._Frame, image.Data) { }

		public static LearningImage Load(string path, ColorType color = ColorType.Color)
		{
			if (!File.Exists(path)) return null;
			return FromBitmap(new Bitmap(path), color);
		}
		public unsafe static LearningImage FromBitmap(Bitmap src, ColorType color = ColorType.Color)
		{
			try
			{
				PixelFormat format = PixelFormat.Format24bppRgb;
				int plane = 3;
				if (color == ColorType.Gray)
				{
					format = PixelFormat.Format8bppIndexed;
					plane = 1;
				}
				BitmapData srcData = src.LockBits(new Rectangle(Point.Empty, src.Size), ImageLockMode.ReadOnly, format);
				LearningImage i = new LearningImage(src.Height, src.Width, plane);
				for (int h = 0; h < srcData.Height; h++)
				{
					byte* ps = (byte*)srcData.Scan0 + srcData.Stride * h;
					for (int w = 0; w < srcData.Width; w++, ps += i.Plane)
					{
						int postion = (i.Width * h + w) * i.Plane;
						for (int l = 0; l < i.Plane; l++)
							i.Data[postion + l] = (double)ps[l] / 255;
					}
				}
				src.UnlockBits(srcData);
				return i;
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				return null;
			}
		}

		public static LearningImage LoadByZoom(string path, int block_size)
		{
			if (!File.Exists(path)) return null;
			Bitmap src = new Bitmap(path);
			Bitmap dest = Zoom(src, block_size);
			LearningImage img = FromBitmap(dest);
			img.Name = Path.GetFileNameWithoutExtension(path);
			return img;
		}

		private static Bitmap Zoom(Bitmap src, int block_size)
		{
			try
			{
				float zoom = 1, x = 0, y = 0, h = 0, w = 0;
				if(src.Width > src.Height)
				{
					zoom = (float)block_size / (float)src.Height;
					x = -(src.Width - src.Height) / 2.0f * zoom;
					h = block_size;
					w = src.Width * zoom;
				}
				else
				{
					zoom = (float)block_size / (float)src.Width;
					y = -(src.Height - src.Width) / 2.0f * zoom;
					h = src.Height * zoom;
					w = block_size;
				}
				Bitmap dest = new Bitmap(block_size, block_size);
				Graphics g = Graphics.FromImage(dest);
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.DrawImage(src, x, y, w, h);
				return dest;
			}
			catch (Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				return null;
			}
		}

		public LearningImage ScaleBitmap(int size)
		{
			Bitmap src = ToBitmap();
			Bitmap dest = Zoom(src, size);
			return FromBitmap(dest);
		}

		unsafe public void SavePng(string path, double low = 0, double high = 1, int start = 0)
		{
			Bitmap b = ToBitmap(low, high, start);
			string dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
			b.Save(path, ImageFormat.Png);
		}
		unsafe public Bitmap ToBitmap(double low = 0, double high = 1, int start = 0)
		{
			//Bitmap b = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
			Bitmap b = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
			BitmapData d = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			for (int h = 0; h < d.Height; h++)
			{
				byte* p = (byte*)d.Scan0 + d.Stride * h;
				for (int w = 0; w < d.Width; w++, p += 3)
				{
					int position = (this.Width * h + w) * Plane;
					for (int l = 0; l < 3; l++)
					{
						int sl = start + l;
						if (sl< Plane) p[sl] = Step(Data[position + sl], low, high);
						else p[l] = 0;
					}
				}
			}
			b.UnlockBits(d);
			return b;
		}
		private byte Step(double v, double low = 0, double high = 1)
		{
			if (v < low) return 0;
			if (v > high) return 255;
			return (byte)((v - low) / (high - low) * 255.0);
		}

		public void SavePngAdjusted(string path)
		{
			var list = LearningImage.HighLow(this);
			SavePng(path, list[1], list[0]);
		}

		public static void Multiply(LearningImage i, LearningImage o, double rate = 1, double bias = 0)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = i.Data[l] * rate + bias;
		}
		public static void Add(LearningImage a, LearningImage b, LearningImage o)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = a.Data[l] + b.Data[l];
		}
		public static void Add(LearningImage a, double b, LearningImage o)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = a.Data[l] + b;
		}
		public static void Sub(LearningImage a, LearningImage b, LearningImage o)
		{
			for (int l = 0; l < o.Length; l++) o.Data[l] = a.Data[l] - b.Data[l];
		}
		public static double DotProduct(LearningImage a, LearningImage b)
		{
			return Matrix.InnerProduct(a.Data, b.Data);
		}
		public static double EuclideanLength(LearningImage a)
		{
			return Norm.Euclidean(a.Data);
		}
		public static double ParticularEuclideanLength(LearningImage a)
		{
			double amount = 0;
			for (int i = 1; i < a.Data.Length; i++) amount += a.Data[i] * a.Data[i];
			return Math.Sqrt(amount);
		}
		public static List<double> HighLow(LearningImage a)
		{
			return new List<double>() { a.Data.Max(), a.Data.Min() };
		}

		public void SaveBin(string path)
		{
			BinaryWriter writer = null;
			try
			{
				string dir = Path.GetDirectoryName(path);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				List<byte> buffer = new List<byte>();
				buffer.AddRange(BitConverter.GetBytes(Height));
				buffer.AddRange(BitConverter.GetBytes(Width));
				buffer.AddRange(BitConverter.GetBytes(Plane));
				for (int l = 0; l < Length; l++) buffer.AddRange(BitConverter.GetBytes(Data[l]));
				writer = new BinaryWriter(File.OpenWrite(path));
				writer.Write(buffer.ToArray());
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
			}
			finally
			{
				if (writer != null) writer.Close();
				writer = null;
			}
		}

		public static LearningImage LoadBin(string path)
		{
			BinaryReader reader = null;
			try
			{
				if (!File.Exists(path)) return null;

				byte[] bytes = File.ReadAllBytes(path);
				int shift = 0;
				int height = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				int width = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				int plane = BitConverter.ToInt32(bytes, shift); shift += sizeof(int);
				LearningImage image = new LearningImage(height, width, plane);
				for (int l = 0; l < image.Length; l++) image.Data[l] = BitConverter.ToDouble(bytes, l * sizeof(double) + shift);
				return image;
			}
			catch(Exception ex)
			{
				Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				return null;
			}
			finally
			{
				if (reader != null) reader.Close();
				reader = null;
			}
		}

		public List<LearningPlane> GetPlanes(Rectangle r)
		{
			List<LearningPlane> list = new List<LearningPlane>();
			for (int h = r.Top; h < r.Bottom; h++)
			{
				for (int w = r.Left; w < r.Right; w++)
				{
					if (h < 0 || h >= this.Height || w < 0 || w >= this.Width) continue;	// 無い画素
					list.Add(GetPlane(h, w));
				}
			}
			return list;
		}

		public LearningImage Trim(Rectangle r, bool isClamped = true)
		{
			LearningImage i = new LearningImage(r.Height, r.Width, Plane);
			for(int h = r.Top; h < r.Bottom; h++)
			{
				for(int w = r.Left; w < r.Right; w++)
				{
					bool isOut = false;
					int sh = h;
					int sw = w;
					if (sh < 0) { sh = 0; isOut = true; }
					if (sh >= this.Height) { sh = this.Height - 1; isOut = true; }
					if (sw < 0) { sw = 0; isOut = true; }
					if (sw >= this.Width) { sw = this.Width - 1; isOut = true; }

					int d = i.Width * (h - r.Top) + (w - r.Left);
					int s = sh * this.Width + sw;
					if (isOut && !isClamped) Array.Clear(i.Data, d * Plane, Plane);	// Clampしないなら0で埋める
					else Array.Copy(this.Data, s * Plane, i.Data, d * Plane, Plane);
				}
			}
			return i;
		}
		public List<LearningImage> MakeSlices(Size s, bool focus = true)
		{
			List<LearningImage> list = new List<LearningImage>();
			for (int h = 0; h < Height - s.Height; h++)
				for (int w = 0; w < Width - s.Width; w++)
				{
					var trimed = Trim(new Rectangle() { X = w, Y = h, Size = s });
					if (focus) trimed = trimed.Focus();
					list.Add(trimed);
				}
			return list;
		}

		public void Blend(int x, int y, LearningImage image, double rate = 0.75)
		{
			for (int h = 0; h < image.Height; h++)
			{
				for (int w = 0; w < image.Width; w++)
				{
					if (h + y >= this.Height || w + x >= this.Width) continue;
					int pt = (this.Width * (h + y) + (w + x)) * Plane;
					int pi = (image.Width * h + w) * Plane;
					for (int l = 0; l < Plane; l++) this.Data[pt + l] = this.Data[pt + l] * (1 - rate) + image.Data[pi + l] * rate;
				}
			}
		}

		// ドロップアウト
		public LearningImage DropOut(double rate)
		{
			LearningImage i = new LearningImage(this);
			Random random = new Random();
			for(int j = 0; j < i.Length; j++)
			{
				if (random.NextDouble() < rate) i.Data[j] = 0;
			}
			return i;
		}

		// ノイズ追加、
		public LearningImage AddNoise(double range)
		{
			LearningImage i = new LearningImage(this);
			Random random = new Random();
			for (int j = 0; j < i.Length; j++)
			{
				i.Data[j] += (random.NextDouble() - 0.5) * range;
			}
			return i;
		}

		// 斉次座標化
		public double[] Homogenize()
		{
			double[] data = new double[Length + 1];
			Array.Copy(this.Data, 0, data, 0, Length);
			data[Length] = 1.0;
			return data;
		}

		// 正規化
		public LearningImage Normalize()
		{
			double amount_x = 0;
			double amount_xx = 0;
			for(int j = 0; j < this.Length; j++)
			{
				double x = this.Data[j];
				amount_x += x;
				amount_xx += x * x;
			}
			double average = amount_x / this.Length;
			double deviation = Math.Sqrt(amount_xx / this.Length - average * average);
			if (deviation == 0) deviation = 1.0;

			LearningImage i = new LearningImage(this);
			for (int j = 0; j < Length; j++) i.Data[j] = (i.Data[j] - average) / deviation;
			return i;
		}

		// 等倍する
		public LearningImage ScaleImage(int scale, bool isStrict = true)
		{
			LearningImage scaled = new LearningImage(Height * scale, Width * scale, Plane);
			for(int h = 0; h < scaled.Height; h++)
			{
				double hh = (double)h / (double)scale;
				for(int w = 0; w < scaled.Width; w++)
				{
					double ww = (double)w / (double)scale;
					var plane = isStrict ? GetPlaneStrict(hh, ww) : GetPlane(hh, ww); // 厳密にするか、平均をとるか要調整
					scaled.SetPlane(h, w, plane);
				}
			}
			return scaled;
		}

		// 中心の値を扱うようにする
		public LearningImage Focus()
		{
			LearningImage focused = new LearningImage(this);
			for(int h = 0; h < focused.Height; h++)
			{
				double sh = (Math.Abs(focused.Height / 2 - 0.5 - h)) / (focused.Height / 2);
				double ah = Math.Sqrt(1 - sh * sh);
				for(int w = 0; w < focused.Width; w++)
				{
					double sw = (Math.Abs(focused.Width / 2 - 0.5 - w)) / (focused.Width / 2);
					double aw = Math.Sqrt(1 - sw * sw);
					int index = h * focused.Width + w;
					focused.Data[index] *= ah * aw; 
				}
			}
			return focused;
		}

		// 和を返す
		public double Sum()
		{
			return Matrix.Sum(Data);
		}
	}

	public class LearningPlane
	{
		public double[] Data;
		public int Length { get { return Data.Length; } }
		public LearningPlane(int plane) { Data = new double[plane]; }
		public LearningPlane(double[] vs) { Data = vs; }
		public double Euclidean() { return Accord.Math.Norm.Euclidean(Data); }
		public double SpecialEuclidean()
		{
			double sum = Accord.Math.Norm.SquareEuclidean(Data) - Data[0] * Data[0];
			return sum > 0 ? Math.Sqrt(sum) : 0;
		}

		public void Add(params LearningPlane[] planes)
		{
			foreach (var plane in planes) this.Data = Accord.Math.Matrix.Add(this.Data, plane.Data);
		}

		public LearningPlane Scale(double scale)
		{
			return new LearningPlane(Accord.Math.Matrix.Multiply(Data, scale));
		}
	}

	public class LearningImagePair
	{
		public LearningImage In;
		public LearningImage Out;
		public LearningImagePair(LearningImage i, LearningImage o) { In = i; Out = o; }
	}
}
